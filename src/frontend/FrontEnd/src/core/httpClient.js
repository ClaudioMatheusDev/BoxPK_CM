import { API_BASE_URL, DEFAULT_HEADERS } from './apiConfig'
import { authSession } from './authSession'

const parseResponse = async (response) => {
  const text = await response.text()

  if (!text) {
    return null
  }

  try {
    return JSON.parse(text)
  } catch {
    return text
  }
}

const buildHeaders = (options) => {
  const token = authSession.getAccessToken()
  const authHeader = token && !options.skipAuth ? { Authorization: `Bearer ${token}` } : {}

  return {
    ...DEFAULT_HEADERS,
    ...authHeader,
    ...options.headers,
  }
}

const refreshAccessToken = async () => {
  const response = await fetch(`${API_BASE_URL}/api/Auth/refresh`, {
    method: 'POST',
    credentials: 'include',
    headers: DEFAULT_HEADERS,
    body: JSON.stringify({}),
  })

  if (!response.ok) {
    authSession.clear()
    return false
  }

  const data = await parseResponse(response)

  if (data?.accessToken) {
    authSession.setAccessToken(data.accessToken)
    return true
  }

  return false
}

const request = async (path, options = {}) => {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...options,
    credentials: 'include',
    headers: buildHeaders(options),
  })

  const data = await parseResponse(response)

  if (response.status === 401 && !options.skipAuth && !options.retry) {
    const refreshed = await refreshAccessToken()

    if (refreshed) {
      return request(path, { ...options, retry: true })
    }
  }

  if (!response.ok) {
    const message =
      Array.isArray(data)
        ? data.map((item) => item.description ?? item.code ?? item).join(' ')
        : typeof data === 'string'
        ? data
        : data?.message ?? data?.title ?? 'Nao foi possivel concluir a operacao.'

    throw new Error(message)
  }

  return data
}

export const httpClient = {
  get: (path, options) => request(path, options),
  post: (path, body, options) =>
    request(path, {
      ...options,
      method: 'POST',
      body: JSON.stringify(body),
    }),
  put: (path, body, options) =>
    request(path, {
      ...options,
      method: 'PUT',
      body: JSON.stringify(body),
    }),
  delete: (path, options) =>
    request(path, {
      ...options,
      method: 'DELETE',
    }),
}
