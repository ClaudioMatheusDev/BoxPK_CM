import { Icon } from './Icon'

export function Alert({ type = 'info', children, onDismiss }) {
  if (!children) {
    return null
  }

  return (
    <div className={`alert alert-${type}`} role={type === 'error' ? 'alert' : 'status'}>
      <Icon name={type === 'error' ? 'alert' : 'check'} size={16} />
      <span>{children}</span>
      {onDismiss ? (
        <button type="button" className="alert-close" onClick={onDismiss} aria-label="Fechar aviso">
          <Icon name="close" size={14} />
        </button>
      ) : null}
    </div>
  )
}
