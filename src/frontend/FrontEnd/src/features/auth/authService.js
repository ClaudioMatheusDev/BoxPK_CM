import { httpClient } from '../../core/httpClient'
import { authSession } from '../../core/authSession'

const saveSession = (response) => {
  if (response?.accessToken) {
    authSession.setAccessToken(response.accessToken)
  }

  return response
}

export const authService = {
  login: async (credentials) => {
    const response = await httpClient.post('/api/Auth/login', credentials, { skipAuth: true })
    return saveSession(response)
  },

  register: async (payload) => {
    await httpClient.post('/api/Auth/register', payload, { skipAuth: true })
  },

  refresh: async () => {
    const response = await httpClient.post('/api/Auth/refresh', {}, { skipAuth: true })
    return saveSession(response)
  },

  logout: async () => {
    try {
      await httpClient.post('/api/Auth/logout', {}, { skipAuth: true })
    } finally {
      authSession.clear()
    }
  },
}
