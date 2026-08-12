export const API_BASE_URL = (
  import.meta.env.VITE_API_BASE_URL || 'http://localhost:5236'
).replace(/\/$/, '')

export const DEFAULT_HEADERS = {
  'Content-Type': 'application/json',
}
