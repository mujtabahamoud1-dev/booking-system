import { api } from '@/shared/api/client'
import type { Booking, CreateBookingRequest } from './types'

export const bookingsApi = {
  async create(req: CreateBookingRequest): Promise<Booking> {
    const { data } = await api.post<Booking>('/bookings', req)
    return data
  },

  async getMine(): Promise<Booking[]> {
    const { data } = await api.get<Booking[]>('/bookings/mine')
    return data
  },

  async getAll(): Promise<Booking[]> {
    const { data } = await api.get<Booking[]>('/bookings')
    return data
  },

  async cancel(id: number): Promise<Booking> {
    const { data } = await api.post<Booking>(`/bookings/${id}/cancel`, null)
    return data
  },

  async confirm(id: number): Promise<Booking> {
    const { data } = await api.post<Booking>(`/bookings/${id}/confirm`, null)
    return data
  },
}
