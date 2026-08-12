import { useMemo, useState } from 'react'
import { authSession } from '../../core/authSession'
import { authService } from './authService'
import { AuthContext } from './authContext'

export function AuthProvider({ children }) {
  const [accessToken, setAccessToken] = useState(() => authSession.getAccessToken())

  const value = useMemo(
    () => ({
      isAuthenticated: Boolean(accessToken),
      accessToken,
      login: async (credentials) => {
        const response = await authService.login(credentials)
        setAccessToken(response.accessToken)
      },
      register: authService.register,
      logout: async () => {
        await authService.logout()
        setAccessToken(null)
      },
    }),
    [accessToken],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}
