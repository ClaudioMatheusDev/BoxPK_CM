import { useMemo, useState } from 'react'
import './App.css'
import { resourceDefinitions } from './core/resources'
import { Dashboard } from './features/dashboard/Dashboard'
import { inventoryResources } from './features/inventory/inventoryResources'
import { purchaseResources } from './features/purchases/purchaseResources'
import { ResourcePage } from './shared/components/ResourcePage'
import { Shell } from './shared/components/Shell'

const sections = [
  { id: 'dashboard', label: 'Dashboard', icon: 'D', group: 'Principal' },
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
  const [activeSection, setActiveSection] = useState('dashboard')
  const currentSection = useMemo(
    () => sections.find((section) => section.id === activeSection),
    [activeSection],
  )

  return (
    <Shell sections={sections} activeSection={activeSection} onSectionChange={setActiveSection}>
      {activeSection === 'dashboard' ? (
        <Dashboard />
      ) : (
        <ResourcePage definition={currentSection.definition} resourcesByKey={resourcesByKey} />
      )}
    </Shell>
  )
}

export default App
