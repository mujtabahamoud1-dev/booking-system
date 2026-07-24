import { ref } from 'vue'
import { defineStore } from 'pinia'
import { servicesApi } from './api'
import type { CreateServiceRequest, Service, UpdateServiceRequest } from './types'

export const useServicesStore = defineStore('services', () => {
  const items = ref<Service[]>([])
  const loading = ref(false)

  async function load(includeInactive = false): Promise<void> {
    loading.value = true
    try {
      items.value = await servicesApi.getAll(includeInactive)
    } finally {
      loading.value = false
    }
  }

  async function create(req: CreateServiceRequest): Promise<Service> {
    const created = await servicesApi.create(req)
    items.value = [...items.value, created]
    return created
  }

  async function update(id: number, req: UpdateServiceRequest): Promise<Service> {
    const updated = await servicesApi.update(id, req)
    items.value = items.value.map((s) => (s.id === id ? updated : s))
    return updated
  }

  async function remove(id: number): Promise<void> {
    await servicesApi.remove(id)
    items.value = items.value.filter((s) => s.id !== id)
  }

  return { items, loading, load, create, update, remove }
})
