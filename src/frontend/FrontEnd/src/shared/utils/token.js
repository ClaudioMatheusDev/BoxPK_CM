const EMAIL_CLAIMS = ['email', 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress']

// Lê o e-mail do payload do JWT apenas para exibição; a validação do token é sempre feita pela API.
export const getEmailFromToken = (token) => {
  try {
    const payload = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/')
    const claims = JSON.parse(atob(payload))
    const key = EMAIL_CLAIMS.find((claim) => claims[claim])

    return key ? claims[key] : ''
  } catch {
    return ''
  }
}
