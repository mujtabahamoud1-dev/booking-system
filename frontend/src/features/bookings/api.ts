import { api } from "@/shared/api/client";
import type {
  AdminBookingListResponse,
  AdminBookingQuery,
  Booking,
  CreateBookingRequest,
} from "./types";

export const bookingsApi = {
  async create(req: CreateBookingRequest): Promise<Booking> {
    const { data } = await api.post<Booking>("/bookings", req);
    return data;
  },

  async getMine(): Promise<Booking[]> {
    const { data } = await api.get<Booking[]>("/bookings/mine");
    return data;
  },

  // Admin only — the API rejects this for anyone else, and it is the one
  // response that carries patient details. Filtering happens in the database, so
  // every change of the search box, the date range or the status chips is a
  // request rather than a pass over a list already in memory.
  async getAll(query: AdminBookingQuery = {}): Promise<AdminBookingListResponse> {
    const { data } = await api.get<AdminBookingListResponse>("/bookings", {
      params: {
        search: query.search?.trim() || undefined,
        status: query.status && query.status !== "all" ? query.status : undefined,
        from: query.from,
        to: query.to,
      },
    });
    return data;
  },

  async cancel(id: number): Promise<Booking> {
    const { data } = await api.post<Booking>(`/bookings/${id}/cancel`, null);
    return data;
  },

  async confirm(id: number): Promise<Booking> {
    const { data } = await api.post<Booking>(`/bookings/${id}/confirm`, null);
    return data;
  },
};
