// Mirrors the backend Auth DTOs (Features/Auth/AuthDTOs.cs).
export type Role = 'admin' | 'client'

export interface RegisterRequest {
  name: string
  email: string
  password: string
  phone?: string | null
}

export interface LoginRequest {
  email: string
  password: string
}

export interface AuthResponse {
  accessToken: string
  role: Role
  name: string
}
