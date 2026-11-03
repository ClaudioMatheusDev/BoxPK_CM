import { useState } from 'react'
import { useAuth } from '../../features/auth/useAuth'
import { getEmailFromToken } from '../utils/token'
import { Icon } from './Icon'

export function Shell({ sections, activeSection, onSectionChange, onLogout, children }) {
  const { accessToken } = useAuth()
  const [menuOpen, setMenuOpen] = useState(false)

  const groupedSections = sections.reduce((groups, section) => {
    const group = section.group ?? 'Geral'
    groups[group] = [...(groups[group] ?? []), section]
    return groups
  }, {})

  const current = sections.find((section) => section.id === activeSection)
  const email = getEmailFromToken(accessToken)

  const select = (id) => {
    onSectionChange(id)
    setMenuOpen(false)
  }

  return (
    <div className="shell">
      <aside className={`sidebar${menuOpen ? ' open' : ''}`}>
        <div className="brand">
          <span className="brand-mark" aria-hidden="true">
            <Icon name="box" size={18} />
          </span>
          <div className="brand-text">
            <strong>BoxPK CM</strong>
            <span>Gestão TCG</span>
          </div>
        </div>

        <nav className="nav" aria-label="Módulos">
          {Object.entries(groupedSections).map(([group, items]) => (
            <div className="nav-group" key={group}>
              <p>{group}</p>
              {items.map((section) => (
                <button
                  type="button"
                  key={section.id}
                  className={activeSection === section.id ? 'nav-item active' : 'nav-item'}
                  aria-current={activeSection === section.id ? 'page' : undefined}
                  onClick={() => select(section.id)}
                >
                  <Icon name={section.icon} size={17} />
                  {section.label}
                </button>
              ))}
            </div>
          ))}
        </nav>

        <div className="sidebar-user">
          <span className="avatar" aria-hidden="true">
            {(email || 'U').slice(0, 1).toUpperCase()}
          </span>
          <span className="user-email" title={email}>
            {email || 'Usuário'}
          </span>
          <button type="button" className="icon-button on-dark" onClick={onLogout} title="Sair" aria-label="Sair">
            <Icon name="logout" size={17} />
          </button>
        </div>
      </aside>

      {menuOpen ? <div className="scrim" onClick={() => setMenuOpen(false)} /> : null}

      <div className="main">
        <header className="topbar">
          <button
            type="button"
            className="icon-button menu-toggle"
            onClick={() => setMenuOpen(true)}
            aria-label="Abrir menu"
          >
            <Icon name="menu" />
          </button>
          <nav className="crumbs" aria-label="Localização">
            <span>{current?.group}</span>
            <span className="sep">/</span>
            <strong>{current?.label}</strong>
          </nav>
        </header>

        <main className="content">{children}</main>
      </div>
    </div>
  )
}
