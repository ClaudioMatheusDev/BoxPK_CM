import { resourceDefinitions } from '../../core/resources'
import { inventoryResources } from '../inventory/inventoryResources'
import { purchaseResources } from '../purchases/purchaseResources'
import { MetricCard } from '../../shared/components/MetricCard'
import { useResource } from '../../shared/hooks/useResource'
import { formatCurrency } from '../../shared/utils/formatters'

export function Dashboard() {
  const produtos = useResource(resourceDefinitions.produtos)
  const categorias = useResource(resourceDefinitions.categorias)
  const jogos = useResource(resourceDefinitions.jogos)
  const estoque = useResource(inventoryResources.estoque)
  const compras = useResource(purchaseResources.compras)

  const estoqueBaixo = estoque.items.filter(
    (item) => Number(item.quantidadeAtual) <= Number(item.quantidadeMinima),
  ).length

  const totalCompras = compras.items.reduce(
    (total, compra) => total + Number(compra.valorTotal ?? 0),
    0,
  )
  const estoqueRegular = Math.max(estoque.items.length - estoqueBaixo, 0)
  const cadastroBase = produtos.items.length && categorias.items.length

  return (
    <section className="dashboard">
      <div className="page-title">
        <div>
          <span>Workspace</span>
          <h1>Dashboard</h1>
        </div>
      </div>

      <div className="metrics-grid">
        <MetricCard label="Produtos" value={produtos.items.length} detail="catalogados" />
        <MetricCard label="Categorias" value={categorias.items.length} detail="ativas e historicas" />
        <MetricCard label="Estoque baixo" value={estoqueBaixo} detail="itens no minimo" />
        <MetricCard label="Compras" value={formatCurrency(totalCompras)} detail="valor acumulado" />
      </div>

      <div className="dashboard-grid">
        <div className="dashboard-panel">
          <div className="panel-header">
            <div>
              <span>Estoque</span>
              <h2>Saude operacional</h2>
            </div>
          </div>
          <div className="status-stack">
            <div>
              <strong>{estoqueRegular}</strong>
              <span>Itens dentro do minimo</span>
            </div>
            <div className={estoqueBaixo ? 'attention' : ''}>
              <strong>{estoqueBaixo}</strong>
              <span>Itens pedindo atencao</span>
            </div>
          </div>
        </div>

        <div className="dashboard-panel">
          <div className="panel-header">
            <div>
              <span>Dados</span>
              <h2>Base de cadastro</h2>
            </div>
          </div>
          <div className="check-list">
            <span className={categorias.items.length ? 'done' : ''}>Categorias</span>
            <span className={jogos.items.length ? 'done' : ''}>Jogos</span>
            <span className={cadastroBase ? 'done' : ''}>Produtos relacionados</span>
          </div>
        </div>
      </div>
    </section>
  )
}
