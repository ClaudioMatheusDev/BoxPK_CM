export function Shell({ sections, activeSection, onSectionChange, onLogout, children }) {
  const groupedSections = sections.reduce((groups, section) => {
    const group = section.group ?? 'Geral'
    groups[group] = [...(groups[group] ?? []), section]
    return groups
  }, {})

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="brand">
          <span className="brand-mark">B</span>
          <div>
            <strong>BoxPK CM</strong>
            <span>Controle de cartas</span>
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

      <main className="content">{children}</main>
    </div>
  )
}
