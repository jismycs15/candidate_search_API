// Session persisted in localStorage so a reload/new tab resumes the logged-in state.
// (Per Task 11: "local storage or session storage" — localStorage chosen so the
// refresh-token flow can outlive a closed tab, not just the current session.)
const STORAGE_KEY = 'candidate-search.auth'

export type StoredAuth = {
  token: string
  refreshToken: string
  expiresAt: number // epoch ms, derived from the login/refresh response's expiresIn
}

export function loadAuth(): StoredAuth | null {
  try {
    const raw = localStorage.getItem(STORAGE_KEY)
    if (!raw) return null
    const parsed = JSON.parse(raw) as Partial<StoredAuth>
    if (!parsed.token || !parsed.refreshToken || !parsed.expiresAt) return null
    return parsed as StoredAuth
  } catch {
    return null
  }
}

export function saveAuth(auth: StoredAuth): void {
  localStorage.setItem(STORAGE_KEY, JSON.stringify(auth))
}

export function clearAuth(): void {
  localStorage.removeItem(STORAGE_KEY)
}
