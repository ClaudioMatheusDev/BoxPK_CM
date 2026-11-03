import { useState } from 'react'
import { Alert } from '../../shared/components/Alert'
import { Icon } from '../../shared/components/Icon'
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
          throw new Error('As senhas informadas não conferem.')
        }

        await register({ email: values.email, password: values.password })
        setMessage('Conta criada. Entre com seu e-mail e senha.')
        setMode('login')
        setValues({ ...initialValues, email: values.email })
      } else {
        await login({ email: values.email, password: values.password })
      }
    } catch (err) {
      setError(err.message || 'Não foi possível autenticar.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <main className="auth">
      <aside className="auth-side">
        <div className="brand">
          <span className="brand-mark" aria-hidden="true">
            <Icon name="box" size={18} />
          </span>
          <div className="brand-text">
            <strong>BoxPK CM</strong>
            <span>Gestão TCG</span>
          </div>
        </div>

        <div className="auth-statement">
          <h1>Catálogo, estoque e compras de cartas em um só lugar.</h1>
          <ul>
            <li>Cadastro de produtos, jogos e coleções</li>
            <li>Saldo de estoque com histórico de movimentações</li>
            <li>Compras com entrada automática no estoque</li>
          </ul>
        </div>
      </aside>

      <section className="auth-main" aria-label={isRegister ? 'Criar conta' : 'Entrar'}>
        <div className="auth-box">
          <h2>{isRegister ? 'Criar conta' : 'Entrar'}</h2>
          <p className="auth-lead">
            {isRegister ? 'Cada conta tem seus próprios dados, isolados dos demais usuários.' : 'Acesse o painel com seu e-mail e senha.'}
          </p>

          <div className="stack">
            <Alert type="error">{error}</Alert>
            <Alert type="success">{message}</Alert>
          </div>

          <form className="form-stack" onSubmit={handleSubmit}>
            <label className="field">
              <span className="field-label">E-mail</span>
              <input
                name="email"
                type="email"
                autoComplete="email"
                value={values.email}
                onChange={handleChange}
                required
                autoFocus
              />
            </label>

            <label className="field">
              <span className="field-label">Senha</span>
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
                <span className="field-label">Confirmar senha</span>
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

            <button type="submit" className="btn btn-primary btn-block" disabled={loading}>
              {loading ? 'Aguarde…' : isRegister ? 'Criar conta' : 'Entrar'}
            </button>
          </form>

          <p className="auth-switch">
            {isRegister ? 'Já possui conta?' : 'Ainda não tem conta?'}{' '}
            <button type="button" className="link" onClick={() => changeMode(isRegister ? 'login' : 'register')}>
              {isRegister ? 'Entrar' : 'Criar conta'}
            </button>
          </p>
        </div>
      </section>
    </main>
  )
}
