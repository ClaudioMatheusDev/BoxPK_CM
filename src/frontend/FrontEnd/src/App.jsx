import { useState } from 'react'
import './App.css'
import { resourceDefinitions } from './core/resources'
import { AuthPage } from './features/auth/AuthPage'
import { useAuth } from './features/auth/useAuth'
import { Dashboard } from './features/dashboard/Dashboard'
import { inventoryResources } from './features/inventory/inventoryResources'
import { purchaseResources } from './features/purchases/purchaseResources'
import { StorefrontPage } from './features/storefront/StorefrontPage'
import { ResourcePage } from './shared/components/ResourcePage'
import { Shell } from './shared/components/Shell'

const sections = [
  { id: 'dashboard', label: 'Visão geral', icon: 'dashboard', group: 'Geral' },
  { id: 'vitrine', label: 'Vitrine', icon: 'store', group: 'Geral' },
  { id: 'produtos', label: 'Produtos', icon: 'box', group: 'Catálogo', definition: resourceDefinitions.produtos },
  { id: 'categorias', label: 'Categorias', icon: 'tag', group: 'Catálogo', definition: resourceDefinitions.categorias },
  { id: 'jogos', label: 'Jogos', icon: 'layers', group: 'Catálogo', definition: resourceDefinitions.jogos },
  { id: 'colecoes', label: 'Coleções', icon: 'collection', group: 'Catálogo', definition: resourceDefinitions.colecoes },
  { id: 'fornecedores', label: 'Fornecedores', icon: 'truck', group: 'Operação', definition: resourceDefinitions.fornecedores },
  { id: 'estoque', label: 'Estoque', icon: 'warehouse', group: 'Operação', definition: inventoryResources.estoque },
  { id: 'movimentacoes', label: 'Movimentações', icon: 'transfer', group: 'Operação', definition: inventoryResources.movimentacoes },
  { id: 'compras', label: 'Compras', icon: 'receipt', group: 'Compras', definition: purchaseResources.compras },
  { id: 'itensCompra', label: 'Itens de compra', icon: 'list', group: 'Compras', definition: purchaseResources.itensCompra },
]

const resourcesByKey = {
  ...resourceDefinitions,
  ...inventoryResources,
  ...purchaseResources,
}

function App() {
  const { isAuthenticated, logout } = useAuth()
  const [activeSection, setActiveSection] = useState('dashboard')
  const currentSection = sections.find((section) => section.id === activeSection)

  if (!isAuthenticated) {
    return <AuthPage />
  }

  return (
    <Shell
      sections={sections}
      activeSection={activeSection}
      onLogout={logout}
      onSectionChange={setActiveSection}
    >
      {activeSection === 'dashboard' ? (
        <Dashboard onNavigate={setActiveSection} />
      ) : activeSection === 'vitrine' ? (
        <StorefrontPage />
      ) : (
        <ResourcePage
          key={activeSection}
          definition={currentSection.definition}
          resourcesByKey={resourcesByKey}
        />
      )}
    </Shell>
  )
}

export default App
