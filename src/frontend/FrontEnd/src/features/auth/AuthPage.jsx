import { useState } from 'react'
import { Alert } from '../../shared/components/Alert'
import { useAuth } from './useAuth'

const initialValues = {
  email: '',
  password: '',
  confirmPassword: '',
}

export function AuthPage() {
  const { login, register } = useAuth()
  const [mode, setMode] = useState('login')
  const [values, setValues] = useState(initialValues)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')
  const [message, setMessage] = useState('')

  const isRegister = mode === 'register'

  const handleChange = (event) => {
    const { name, value } = event.target
    setValues((current) => ({ ...current, [name]: value }))
  }

  const changeMode = (nextMode) => {
    setMode(nextMode)
    setError('')
    setMessage('')
    setValues(initialValues)
  }

  const handleSubmit = async (event) => {
    event.preventDefault()
    setLoading(true)
    setError('')
    setMessage('')

    try {
      if (isRegister) {
        if (values.password !== values.confirmPassword) {
          throw new Error('As senhas informadas nao conferem.')
        }

        await register({ email: values.email, password: values.password })
        setMessage('Conta criada com sucesso. Voce ja pode entrar.')
        setMode('login')
        setValues({ ...initialValues, email: values.email })
      } else {
        await login({ email: values.email, password: values.password })
      }
    } catch (err) {
      setError(err.message || 'Nao foi possivel autenticar.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <main className="auth-page">
      <section className="auth-visual">
        <div className="auth-brand">
          <span className="brand-mark">B</span>
          <strong>BoxPK CM</strong>
        </div>

        <div className="auth-copy">
          <span>Gestao protegida</span>
          <h1>Controle seu catalogo, estoque e compras com seguranca.</h1>
          <p>
            Acesse o painel com JWT, refresh token em cookie seguro e rotas protegidas
            pela API.
          </p>
        </div>
      </section>

      <section className="auth-panel" aria-label={isRegister ? 'Registro' : 'Login'}>
        <div className="auth-card">
          <div className="auth-header">
            <span>{isRegister ? 'Criar acesso' : 'Bem-vindo'}</span>
            <h2>{isRegister ? 'Registrar conta' : 'Entrar no painel'}</h2>
          </div>

          <div className="auth-tabs">
            <button
              type="button"
              className={!isRegister ? 'active' : ''}
              onClick={() => changeMode('login')}
            >
              Login
            </button>
            <button
              type="button"
              className={isRegister ? 'active' : ''}
              onClick={() => changeMode('register')}
            >
              Registro
            </button>
          </div>

          <Alert type="error">{error}</Alert>
          <Alert type="success">{message}</Alert>

          <form className="auth-form" onSubmit={handleSubmit}>
            <label className="field">
              <span>E-mail</span>
              <input
                name="email"
                type="email"
                autoComplete="email"
                value={values.email}
                onChange={handleChange}
                required
              />
            </label>

            <label className="field">
              <span>Senha</span>
              <input
                name="password"
                type="password"
                autoComplete={isRegister ? 'new-password' : 'current-password'}
                minLength={6}
                value={values.password}
                onChange={handleChange}
                required
              />
            </label>

            {isRegister ? (
              <label className="field">
                <span>Confirmar senha</span>
                <input
                  name="confirmPassword"
                  type="password"
                  autoComplete="new-password"
                  minLength={6}
                  value={values.confirmPassword}
                  onChange={handleChange}
                  required
                />
              </label>
            ) : null}

            <button type="submit" className="primary-button" disabled={loading}>
              {loading ? 'Aguarde...' : isRegister ? 'Criar conta' : 'Entrar'}
            </button>
          </form>
        </div>
      </section>
    </main>
  )
}
