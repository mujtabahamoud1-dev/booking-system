import { ref } from 'vue'
import { defineStore } from 'pinia'
import { slotsApi } from './api'
import type { CreateSlotRequest, Slot, UpdateSlotRequest } from './types'

// Slots are always scoped to a service, so the store keys its cache by serviceId.
export const useSlotsStore = defineStore('slots', () => {
  const byService = ref<Record<number, Slot[]>>({})
  const loading = ref(false)

  function forService(serviceId: number): Slot[] {
    return byService.value[serviceId] ?? []
  }

  async function load(serviceId: number): Promise<void> {
    loading.value = true
    try {
      byService.value = { ...byService.value, [serviceId]: await slotsApi.getForService(serviceId) }
    } finally {
      loading.value = false
    }
  }

  async function create(req: CreateSlotRequest): Promise<Slot> {
    const created = await slotsApi.create(req)
    const existing = byService.value[req.serviceId] ?? []
    byService.value = { ...byService.value, [req.serviceId]: [...existing, created] }
    return created
  }

  async function update(id: number, serviceId: number, req: UpdateSlotRequest): Promise<Slot> {
    const updated = await slotsApi.update(id, req)
    const existing = byService.value[serviceId] ?? []
    byService.value = {
      ...byService.value,
      [serviceId]: existing.map((s) => (s.id === id ? updated : s)),
    }
    return updated
  }

  async function remove(id: number, serviceId: number): Promise<void> {
    await slotsApi.remove(id)
    const existing = byService.value[serviceId] ?? []
    byService.value = { ...byService.value, [serviceId]: existing.filter((s) => s.id !== id) }
  }

  return { byService, loading, forService, load, create, update, remove }
})
