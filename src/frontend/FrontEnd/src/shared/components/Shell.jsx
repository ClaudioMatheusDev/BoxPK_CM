export function Shell({ sections, activeSection, onSectionChange, onLogout, children }) {
  const groupedSections = sections.reduce((groups, section) => {
    const group = section.group ?? 'Geral'
    groups[group] = [...(groups[group] ?? []), section]
    return groups
  }, {})

  const currentLabel = sections.find((section) => section.id === activeSection)?.label ?? 'Dashboard'

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="brand">
          <span className="brand-mark">.   Box</span>
          <div>
            <strong>BoxPK CM</strong>
            <span>Controle de cartas TCG</span>
          </div>
        </div>

        <nav className="nav-list" aria-label="Modulos">
          {Object.entries(groupedSections).map(([group, items]) => (
            <div className="nav-group" key={group}>
              <p>{group}</p>
              {items.map((section) => (
                <button
                  type="button"
                  key={section.id}
                  className={activeSection === section.id ? 'active' : ''}
                  onClick={() => onSectionChange(section.id)}
                  title={section.label}
                >
                  <span>{section.icon}</span>
                  {section.label}
                </button>
              ))}
            </div>
          ))}
        </nav>

        <button type="button" className="logout-button" onClick={onLogout}>
          Sair
        </button>
      </aside>

      <main className="content">
        <div className="content-shell">
          <header className="content-header">
            <div>
              <span>Operações</span>
              <h2>{currentLabel}</h2>
            </div>
            <div className="header-badges">
              <span className="status-pill success">Sistema ativo</span>
              <span className="status-pill">Cloud sync</span>
            </div>
          </header>
          {children}
        </div>
      </main>
    </div>
  )
}