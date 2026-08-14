import { api } from "@/shared/api/client";
import type {
  CreateServiceRequest,
  Service,
  ServiceListResponse,
  ServiceQuery,
  UpdateServiceRequest,
} from "./types";

export const servicesApi = {
  async getAll(includeInactive = false): Promise<Service[]> {
    const { data } = await api.get<Service[]>("/services", {
      params: includeInactive ? { includeInactive: true } : undefined,
    });
    return data;
  },

  // Admin only. Filtering happens in the database, so every change of the search
  // box or the status chips is a request — the client never re-filters a list it
  // is already holding.
  async search(query: ServiceQuery): Promise<ServiceListResponse> {
    const { data } = await api.get<ServiceListResponse>("/services/admin", {
      params: {
        search: query.search?.trim() || undefined,
        status: query.status && query.status !== "all" ? query.status : undefined,
      },
    });
    return data;
  },

  async getById(id: number): Promise<Service> {
    const { data } = await api.get<Service>(`/services/${id}`);
    return data;
  },

  async create(req: CreateServiceRequest): Promise<Service> {
    const { data } = await api.post<Service>("/services", req);
    return data;
  },

  async update(id: number, req: UpdateServiceRequest): Promise<Service> {
    const { data } = await api.put<Service>(`/services/${id}`, req);
    return data;
  },

  async remove(id: number): Promise<void> {
    await api.delete(`/services/${id}`);
  },
};
