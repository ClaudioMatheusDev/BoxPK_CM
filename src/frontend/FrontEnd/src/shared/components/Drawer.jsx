import { useEffect } from 'react'
import { Icon } from './Icon'

export function Drawer({ open, title, subtitle, onClose, children }) {
  useEffect(() => {
    if (!open) {
      return undefined
    }

    const handleKey = (event) => {
      if (event.key === 'Escape') {
        onClose()
      }
    }

    document.addEventListener('keydown', handleKey)
    return () => document.removeEventListener('keydown', handleKey)
  }, [open, onClose])

  if (!open) {
    return null
  }

  return (
    <div className="overlay" onMouseDown={(event) => event.target === event.currentTarget && onClose()}>
      <aside className="drawer" role="dialog" aria-modal="true" aria-label={title}>
        <header className="drawer-header">
          <div>
            {subtitle ? <span className="eyebrow">{subtitle}</span> : null}
            <h2>{title}</h2>
          </div>
          <button type="button" className="icon-button" onClick={onClose} aria-label="Fechar">
            <Icon name="close" />
          </button>
        </header>
        {children}
      </aside>
    </div>
  )
}
