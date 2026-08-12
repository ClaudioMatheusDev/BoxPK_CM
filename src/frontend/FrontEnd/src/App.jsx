import { useMemo, useState } from 'react'
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
  { id: 'dashboard', label: 'Dashboard', icon: 'D', group: 'Principal' },
  { id: 'vitrine', label: 'Vitrine', icon: 'V', group: 'Principal' },
  { id: 'produtos', label: 'Produtos', icon: 'P', group: 'Catalogo', definition: resourceDefinitions.produtos },
  { id: 'categorias', label: 'Categorias', icon: 'C', group: 'Catalogo', definition: resourceDefinitions.categorias },
  { id: 'jogos', label: 'Jogos', icon: 'J', group: 'Catalogo', definition: resourceDefinitions.jogos },
  { id: 'colecoes', label: 'Colecoes', icon: 'S', group: 'Catalogo', definition: resourceDefinitions.colecoes },
  { id: 'fornecedores', label: 'Fornecedores', icon: 'F', group: 'Operacao', definition: resourceDefinitions.fornecedores },
  { id: 'estoque', label: 'Estoque', icon: 'E', group: 'Operacao', definition: inventoryResources.estoque },
  { id: 'movimentacoes', label: 'Movimentacoes', icon: 'M', group: 'Operacao', definition: inventoryResources.movimentacoes },
  { id: 'compras', label: 'Compras', icon: 'R', group: 'Compras', definition: purchaseResources.compras },
  { id: 'itensCompra', label: 'Itens Compra', icon: 'I', group: 'Compras', definition: purchaseResources.itensCompra },
]

const resourcesByKey = {
  ...resourceDefinitions,
  ...inventoryResources,
  ...purchaseResources,
}

function App() {
  const { isAuthenticated, logout } = useAuth()
  const [activeSection, setActiveSection] = useState('dashboard')
  const currentSection = useMemo(
    () => sections.find((section) => section.id === activeSection),
    [activeSection],
  )

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
        <Dashboard />
      ) : activeSection === 'vitrine' ? (
        <StorefrontPage />
      ) : (
        <ResourcePage definition={currentSection.definition} resourcesByKey={resourcesByKey} />
      )}
    </Shell>
  )
}

export default App
