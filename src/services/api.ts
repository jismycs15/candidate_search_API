import { clearAuth, loadAuth, saveAuth } from './authStorage'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL as string | undefined
if (!API_BASE_URL) {
  throw new Error('VITE_API_BASE_URL is not set. Copy .env.example to .env and configure it.')
}

export class UnauthorizedError extends Error {
  constructor(message = 'Your session has expired. Please log in again.') {
    super(message)
    this.name = 'UnauthorizedError'
  }
}

export type Candidate = {
  id: number
  name: string
  age: number
  education: string
  location: string
  createdAt: string
  lastLoginAt: string | null
}

export type SearchResponse = {
  page: number
  pageSize: number
  totalCount: number
  items: Candidate[]
}

export type CandidateFilters = {
  minAge: number | ''
  maxAge: number | ''
  gender: string
  location: string
  education: string[]
  createdFrom: string
  createdTo: string
  lastLoginFrom: string
  lastLoginTo: string
}

export const emptyFilters: CandidateFilters = {
  minAge: '',
  maxAge: '',
  gender: '',
  location: '',
  education: [],
  createdFrom: '',
  createdTo: '',
  lastLoginFrom: '',
  lastLoginTo: '',
}

export function filtersToSearchParams(filters: CandidateFilters, page: number, pageSize: number): URLSearchParams {
  const params = new URLSearchParams()
  if (filters.minAge !== '') params.set('minAge', String(filters.minAge))
  if (filters.maxAge !== '') params.set('maxAge', String(filters.maxAge))
  if (filters.gender) params.set('gender', filters.gender)
  if (filters.location) params.set('location', filters.location)
  if (filters.education.length > 0) params.set('education', filters.education.join(','))
  if (filters.createdFrom) params.set('createdFrom', filters.createdFrom)
  if (filters.createdTo) params.set('createdTo', filters.createdTo)
  if (filters.lastLoginFrom) params.set('lastLoginFrom', filters.lastLoginFrom)
  if (filters.lastLoginTo) params.set('lastLoginTo', filters.lastLoginTo)
  if (page > 1) params.set('page', String(page))
  if (pageSize !== 10) params.set('pageSize', String(pageSize))
  return params
}

export function searchParamsToFilters(params: URLSearchParams): { filters: CandidateFilters; page: number } {
  const num = (key: string): number | '' => {
    const raw = params.get(key)
    if (raw === null || raw === '') return ''
    const value = Number(raw)
    return Number.isFinite(value) ? value : ''
  }
  const filters: CandidateFilters = {
    minAge: num('minAge'),
    maxAge: num('maxAge'),
    gender: params.get('gender') ?? '',
    location: params.get('location') ?? '',
    education: params.get('education')?.split(',').filter(Boolean) ?? [],
    createdFrom: params.get('createdFrom') ?? '',
    createdTo: params.get('createdTo') ?? '',
    lastLoginFrom: params.get('lastLoginFrom') ?? '',
    lastLoginTo: params.get('lastLoginTo') ?? '',
  }
  const page = Math.max(1, Number(params.get('page')) || 1)
  return { filters, page }
}

async function parseErrorMessage(response: Response): Promise<string> {
  try {
    const body = await response.json()
    if (typeof body?.message === 'string') return body.message
    if (body?.errors && typeof body.errors === 'object') {
      const firstField = Object.values(body.errors as Record<string, unknown>)[0]
      if (Array.isArray(firstField) && typeof firstField[0] === 'string') return firstField[0]
    }
    if (typeof body?.title === 'string') return body.title
  } catch {
    // Body wasn't JSON (or was empty) — fall through to a generic message.
  }
  return `Request failed with status ${response.status}.`
}

function isAbortError(error: unknown): error is DOMException {
  return error instanceof DOMException && error.name === 'AbortError'
}

async function rawFetch(path: string, options: RequestInit): Promise<Response> {
  try {
    const headers = new Headers(options.headers)
    headers.set('Content-Type', 'application/json')
    return await fetch(`${API_BASE_URL}${path}`, { ...options, headers })
  } catch (error) {
    if (isAbortError(error)) throw error
    throw new Error('Unable to reach the server. Check your connection and try again.')
  }
}

// Only one refresh call in flight at a time — concurrent 401s (e.g. two searches racing)
// share the same refresh attempt instead of each rotating the refresh token themselves.
let refreshInFlight: Promise<boolean> | null = null

async function tryRefresh(): Promise<boolean> {
  const auth = loadAuth()
  if (!auth) return false

  refreshInFlight ??= (async () => {
    try {
      const response = await rawFetch('/auth/refresh', {
        method: 'POST',
        body: JSON.stringify({ refreshToken: auth.refreshToken }),
      })
      if (!response.ok) return false
      const data = await response.json()
      saveAuth({ token: data.token, refreshToken: data.refreshToken, expiresAt: Date.now() + data.expiresIn * 1000 })
      return true
    } catch {
      return false
    } finally {
      refreshInFlight = null
    }
  })()

  return refreshInFlight
}

/** For endpoints that don't require a token (register/login/refresh). */
async function publicFetch(path: string, options: RequestInit = {}): Promise<Response> {
  return rawFetch(path, options)
}

/** For candidate-facing endpoints: attaches the bearer token and retries once after a refresh. */
async function authFetch(path: string, options: RequestInit = {}, allowRetry = true): Promise<Response> {
  const auth = loadAuth()
  if (!auth) {
    throw new UnauthorizedError('You are not logged in.')
  }

  const response = await rawFetch(path, {
    ...options,
    headers: { ...options.headers, Authorization: `Bearer ${auth.token}` },
  })

  if (response.status === 401) {
    if (allowRetry && (await tryRefresh())) {
      return authFetch(path, options, false)
    }
    clearAuth()
    throw new UnauthorizedError()
  }

  return response
}

export async function login(email: string, password: string): Promise<void> {
  const response = await publicFetch('/auth/login', {
    method: 'POST',
    body: JSON.stringify({ email, password }),
  })
  if (!response.ok) throw new Error(await parseErrorMessage(response))
  const data = await response.json()
  saveAuth({ token: data.token, refreshToken: data.refreshToken, expiresAt: Date.now() + data.expiresIn * 1000 })
}

export async function logout(): Promise<void> {
  const auth = loadAuth()
  clearAuth() // Local session ends immediately regardless of whether the server call succeeds.
  if (!auth) return
  try {
    await rawFetch('/auth/logout', { method: 'POST', headers: { Authorization: `Bearer ${auth.token}` } })
  } catch {
    // Best-effort server-side revoke; nothing for the UI to react to either way.
  }
}

export function isLoggedIn(): boolean {
  return loadAuth() !== null
}

export async function searchCandidates(
  filters: CandidateFilters,
  page: number,
  pageSize: number,
  signal?: AbortSignal,
): Promise<SearchResponse> {
  const params = filtersToSearchParams(filters, page, pageSize)
  params.set('page', String(page))
  params.set('pageSize', String(pageSize))
  const response = await authFetch(`/candidates/search?${params.toString()}`, { method: 'GET', signal })
  if (!response.ok) throw new Error(await parseErrorMessage(response))
  return response.json()
}
