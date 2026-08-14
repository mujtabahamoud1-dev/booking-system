import axios, { AxiosError, type InternalAxiosRequestConfig } from "axios";

// The one axios instance every feature's api.ts talks through — the frontend
// analogue of the backend's DbSession: a single shared connection to the API.
// withCredentials is required so the httpOnly refresh_token cookie rides along.
const baseURL = import.meta.env.VITE_API_URL ?? "http://localhost:5007/api";

export const api = axios.create({
  baseURL,
  withCredentials: true,
});

// --- access token state -----------------------------------------------------
// The access token is the single source of truth here; the auth store mirrors
// it (and persists it) via the callbacks below. Kept in this module so the
// request/response interceptors can read it without importing the store,
// which would create a circular dependency.
let accessToken: string | null = null;
let onTokenRefreshed: ((raw: unknown) => void) | null = null;
let onAuthFailed: (() => void) | null = null;

export function setAccessToken(token: string | null): void {
  accessToken = token;
}

// Called once from the auth store so the interceptors can push a refreshed
// token back into the store, and trigger a logout when refresh finally fails.
export function configureAuth(handlers: {
  onTokenRefreshed: (raw: unknown) => void;
  onAuthFailed: () => void;
}): void {
  onTokenRefreshed = handlers.onTokenRefreshed;
  onAuthFailed = handlers.onAuthFailed;
}

const isAuthPath = (url = ""): boolean =>
  ["/auth/login", "/auth/register", "/auth/refresh"].some((p) => url.includes(p));

api.interceptors.request.use((config) => {
  if (accessToken) config.headers.Authorization = `Bearer ${accessToken}`;
  return config;
});

// On a 401 for a normal request, try the refresh cookie exactly once, then
// replay the original request. If refresh itself fails, surface it to the store.
api.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const original = error.config as
      (InternalAxiosRequestConfig & { _retry?: boolean }) | undefined;

    if (
      error.response?.status === 401 &&
      original &&
      !original._retry &&
      !isAuthPath(original.url)
    ) {
      original._retry = true;
      try {
        const { data } = await axios.post(`${baseURL}/auth/refresh`, null, {
          withCredentials: true,
        });
        setAccessToken((data as { accessToken: string }).accessToken);
        onTokenRefreshed?.(data);
        return api(original);
      } catch {
        onAuthFailed?.();
      }
    }

    return Promise.reject(error);
  },
);

// Pull the API's `{ message }` body out of an error for display; fall back to
// the axios message otherwise.
export function apiErrorMessage(error: unknown, fallback = "Something went wrong."): string {
  if (error instanceof AxiosError) {
    const data = error.response?.data as { message?: string } | undefined;
    return data?.message ?? error.message ?? fallback;
  }
  return fallback;
}
