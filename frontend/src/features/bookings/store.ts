import { ref } from 'vue'
import { defineStore } from 'pinia'
import { bookingsApi } from './api'
import type { Booking, CreateBookingRequest } from './types'

// One store serves both scopes: `mine` for clients, `all` for the admin view.
export const useBookingsStore = defineStore('bookings', () => {
  const items = ref<Booking[]>([])
  const loading = ref(false)

  async function loadMine(): Promise<void> {
    loading.value = true
    try {
      items.value = await bookingsApi.getMine()
    } finally {
      loading.value = false
    }
  }

  async function loadAll(): Promise<void> {
    loading.value = true
    try {
      items.value = await bookingsApi.getAll()
    } finally {
      loading.value = false
    }
  }

  async function create(req: CreateBookingRequest): Promise<Booking> {
    const created = await bookingsApi.create(req)
    items.value = [created, ...items.value]
    return created
  }

  function replace(updated: Booking): void {
    items.value = items.value.map((b) => (b.id === updated.id ? updated : b))
  }

  async function cancel(id: number): Promise<void> {
    replace(await bookingsApi.cancel(id))
  }

  async function confirm(id: number): Promise<void> {
    replace(await bookingsApi.confirm(id))
  }

  return { items, loading, loadMine, loadAll, create, cancel, confirm }
})
