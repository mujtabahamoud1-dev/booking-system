import { api } from "@/shared/api/client";
import type { AuthResponse, LoginRequest, RegisterRequest } from "./types";

// Raw HTTP for the Auth slice — the frontend analogue of a backend Repository:
// it knows the endpoints and shapes, nothing about state.
export const authApi = {
  async login(req: LoginRequest): Promise<AuthResponse> {
    const { data } = await api.post<AuthResponse>("/auth/login", req);
    return data;
  },

  async register(req: RegisterRequest): Promise<AuthResponse> {
    const { data } = await api.post<AuthResponse>("/auth/register", req);
    return data;
  },

  async refresh(): Promise<AuthResponse> {
    const { data } = await api.post<AuthResponse>("/auth/refresh", null);
    return data;
  },

  async logout(): Promise<void> {
    await api.post("/auth/logout", null);
  },
};
