import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import { configureAuth, setAccessToken } from '@/shared/api/client'
import { authApi } from './api'
import type { AuthResponse, LoginRequest, RegisterRequest, Role } from './types'

const STORAGE_KEY = 'booking.auth'

interface PersistedAuth {
  accessToken: string
  role: Role
  name: string
}

function load(): PersistedAuth | null {
  const raw = localStorage.getItem(STORAGE_KEY)
  return raw ? (JSON.parse(raw) as PersistedAuth) : null
}

// Holds session state and the login/logout logic — the frontend analogue of a
// backend Service. Components never call authApi directly; they go through here.
export const useAuthStore = defineStore('auth', () => {
  const accessToken = ref<string | null>(null)
  const role = ref<Role | null>(null)
  const name = ref<string | null>(null)

  const isAuthenticated = computed(() => accessToken.value !== null)
  const isAdmin = computed(() => role.value === 'admin')

  function apply(auth: AuthResponse | PersistedAuth): void {
    accessToken.value = auth.accessToken
    role.value = auth.role
    name.value = auth.name
    setAccessToken(auth.accessToken)
    localStorage.setItem(
      STORAGE_KEY,
      JSON.stringify({ accessToken: auth.accessToken, role: auth.role, name: auth.name }),
    )
  }

  function clear(): void {
    accessToken.value = null
    role.value = null
    name.value = null
    setAccessToken(null)
    localStorage.removeItem(STORAGE_KEY)
  }

  // Rehydrate from a previous session and wire the client's refresh/failure
  // callbacks to this store. Called once at app startup.
  function init(): void {
    const persisted = load()
    if (persisted) apply(persisted)

    configureAuth({
      onTokenRefreshed: (raw) => apply(raw as AuthResponse),
      onAuthFailed: () => clear(),
    })
  }

  async function login(req: LoginRequest): Promise<void> {
    apply(await authApi.login(req))
  }

  async function register(req: RegisterRequest): Promise<void> {
    apply(await authApi.register(req))
  }

  async function logout(): Promise<void> {
    try {
      await authApi.logout()
    } finally {
      clear()
    }
  }

  return { accessToken, role, name, isAuthenticated, isAdmin, init, login, register, logout }
})
