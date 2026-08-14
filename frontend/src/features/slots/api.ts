import { api } from "@/shared/api/client";
import type {
  CreateSlotRequest,
  Slot,
  SlotListResponse,
  SlotQuery,
  UpdateSlotRequest,
} from "./types";

export const slotsApi = {
  async getForService(serviceId: number): Promise<Slot[]> {
    const { data } = await api.get<Slot[]>(`/slots/service/${serviceId}`);
    return data;
  },

  // Admin only, because leaving serviceId off spans every service. Filtering
  // happens in the database, so changing the service or the day is a request.
  async search(query: SlotQuery): Promise<SlotListResponse> {
    const { data } = await api.get<SlotListResponse>("/slots", {
      params: { serviceId: query.serviceId, dayOfWeek: query.dayOfWeek },
    });
    return data;
  },

  async create(req: CreateSlotRequest): Promise<Slot> {
    const { data } = await api.post<Slot>("/slots", req);
    return data;
  },

  async update(id: number, req: UpdateSlotRequest): Promise<Slot> {
    const { data } = await api.put<Slot>(`/slots/${id}`, req);
    return data;
  },

  async remove(id: number): Promise<void> {
    await api.delete(`/slots/${id}`);
  },
};
