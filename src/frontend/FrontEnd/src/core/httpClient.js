import { API_BASE_URL, DEFAULT_HEADERS } from './apiConfig'

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

const request = async (path, options = {}) => {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...options,
    headers: {
      ...DEFAULT_HEADERS,
      ...options.headers,
    },
  })

  const data = await parseResponse(response)

  if (!response.ok) {
    const message =
      typeof data === 'string'
        ? data
        : data?.message ?? data?.title ?? 'Nao foi possivel concluir a operacao.'

    throw new Error(message)
  }

  return data
}

export const httpClient = {
  get: (path) => request(path),
  post: (path, body) =>
    request(path, {
      method: 'POST',
      body: JSON.stringify(body),
    }),
  put: (path, body) =>
    request(path, {
      method: 'PUT',
      body: JSON.stringify(body),
    }),
  delete: (path) =>
    request(path, {
      method: 'DELETE',
    }),
}
