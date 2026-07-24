import { api } from '@/shared/api/client'
import type { CreateSlotRequest, Slot, UpdateSlotRequest } from './types'

export const slotsApi = {
  async getForService(serviceId: number): Promise<Slot[]> {
    const { data } = await api.get<Slot[]>(`/slots/service/${serviceId}`)
    return data
  },

  async create(req: CreateSlotRequest): Promise<Slot> {
    const { data } = await api.post<Slot>('/slots', req)
    return data
  },

  async update(id: number, req: UpdateSlotRequest): Promise<Slot> {
    const { data } = await api.put<Slot>(`/slots/${id}`, req)
    return data
  },

  async remove(id: number): Promise<void> {
    await api.delete(`/slots/${id}`)
  },
}
