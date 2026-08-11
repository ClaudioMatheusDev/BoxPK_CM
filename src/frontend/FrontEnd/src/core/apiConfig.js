const apiBaseUrl = import.meta.env.VITE_API_BASE_URL

if (!apiBaseUrl) {
  throw new Error('VITE_API_BASE_URL nao configurada.')
}

export const API_BASE_URL = apiBaseUrl.replace(/\/$/, '')

export const DEFAULT_HEADERS = {
  'Content-Type': 'application/json',
}
