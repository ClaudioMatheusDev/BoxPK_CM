import { useMemo } from 'react'
import { resourceDefinitions } from '../../core/resources'
import { Badge } from '../../shared/components/Badge'
import { MetricCard } from '../../shared/components/MetricCard'
import { useResource } from '../../shared/hooks/useResource'
import { formatCurrency, formatDate, formatStatus, statusTone } from '../../shared/utils/formatters'
import { inventoryResources } from '../inventory/inventoryResources'
import { purchaseResources } from '../purchases/purchaseResources'

const STATUS_PENDENTE = 1
const STATUS_PRODUTO_ATIVO = 1

export function Dashboard({ onNavigate }) {
  const produtos = useResource(resourceDefinitions.produtos)
  const fornecedores = useResource(resourceDefinitions.fornecedores)
  const estoque = useResource(inventoryResources.estoque)
  const compras = useResource(purchaseResources.compras)

  const loading = produtos.loading || estoque.loading || compras.loading || fornecedores.loading
  const error = produtos.error || estoque.error || compras.error || fornecedores.error

  const produtoNome = useMemo(
    () => new Map(produtos.items.map((item) => [item.idProduto, item.nome])),
    [produtos.items],
  )
  const fornecedorNome = useMemo(
    () => new Map(fornecedores.items.map((item) => [item.idFornecedor, item.nome])),
    [fornecedores.items],
  )

  const resumo = useMemo(() => {
    const criticos = estoque.items
      .filter((item) => Number(item.quantidadeAtual) <= Number(item.quantidadeMinima))
      .sort(
        (a, b) =>
          Number(a.quantidadeAtual) - Number(a.quantidadeMinima) -
          (Number(b.quantidadeAtual) - Number(b.quantidadeMinima)),
      )

    const pendentes = compras.items.filter((compra) => compra.statusCompra === STATUS_PENDENTE)

    return {
      criticos,
      produtosAtivos: produtos.items.filter((item) => item.statusProduto === STATUS_PRODUTO_ATIVO).length,
      unidades: estoque.items.reduce((total, item) => total + Number(item.quantidadeAtual ?? 0), 0),
      pendentes: pendentes.length,
      valorPendente: pendentes.reduce((total, compra) => total + Number(compra.valorTotal ?? 0), 0),
      recentes: [...compras.items].sort((a, b) => b.idCompra - a.idCompra).slice(0, 6),
    }
  }, [compras.items, estoque.items, produtos.items])

  return (
    <section className="page">
      <header className="page-header">
        <div>
          <h1>Visão geral</h1>
          <p className="page-sub">Situação atual do catálogo, do estoque e das compras.</p>
        </div>
      </header>

      {error ? <div className="alert alert-error">{error}</div> : null}

      <div className="metrics">
        <MetricCard
          label="Produtos ativos"
          value={loading ? '—' : resumo.produtosAtivos}
          detail={loading ? '' : `de ${produtos.items.length} cadastrados`}
        />
        <MetricCard
          label="Unidades em estoque"
          value={loading ? '—' : resumo.unidades.toLocaleString('pt-BR')}
          detail={loading ? '' : `em ${estoque.items.length} itens`}
        />
        <MetricCard
          label="Abaixo do mínimo"
          value={loading ? '—' : resumo.criticos.length}
          detail="itens que precisam de reposição"
          tone={resumo.criticos.length ? 'bad' : undefined}
        />
        <MetricCard
          label="Compras pendentes"
          value={loading ? '—' : resumo.pendentes}
          detail={loading ? '' : `${formatCurrency(resumo.valorPendente)} a confirmar`}
        />
      </div>

      <div className="split">
        <div className="panel">
          <div className="panel-head">
            <h2>Reposição necessária</h2>
            <button type="button" className="link" onClick={() => onNavigate('estoque')}>
              Ver estoque
            </button>
          </div>

          {resumo.criticos.length ? (
            <div className="table-wrap">
              <table className="table">
                <thead>
                  <tr>
                    <th>Produto</th>
                    <th className="num">Atual</th>
                    <th className="num">Mínimo</th>
                  </tr>
                </thead>
                <tbody>
                  {resumo.criticos.slice(0, 8).map((item) => (
                    <tr key={item.idEstoque}>
                      <td>{produtoNome.get(item.idProduto) ?? `Produto #${item.idProduto}`}</td>
                      <td className="num">
                        <strong className={Number(item.quantidadeAtual) === 0 ? 'text-bad' : ''}>
                          {item.quantidadeAtual}
                        </strong>
                      </td>
                      <td className="num">{item.quantidadeMinima}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          ) : (
            <div className="empty-state">{loading ? 'Carregando…' : 'Nenhum item abaixo do estoque mínimo.'}</div>
          )}
        </div>

        <div className="panel">
          <div className="panel-head">
            <h2>Últimas compras</h2>
            <button type="button" className="link" onClick={() => onNavigate('compras')}>
              Ver compras
            </button>
          </div>

          {resumo.recentes.length ? (
            <div className="table-wrap">
              <table className="table">
                <thead>
                  <tr>
                    <th>Fornecedor</th>
                    <th>Data</th>
                    <th className="num">Total</th>
                    <th>Status</th>
                  </tr>
                </thead>
                <tbody>
                  {resumo.recentes.map((compra) => {
                    const status = formatStatus('statusCompra', compra.statusCompra)

                    return (
                      <tr key={compra.idCompra}>
                        <td>{fornecedorNome.get(compra.idFornecedor) ?? `Fornecedor #${compra.idFornecedor}`}</td>
                        <td>{formatDate(compra.dataCompra)}</td>
                        <td className="num">{formatCurrency(compra.valorTotal)}</td>
                        <td>
                          <Badge tone={statusTone(status)}>{status}</Badge>
                        </td>
                      </tr>
                    )
                  })}
                </tbody>
              </table>
            </div>
          ) : (
            <div className="empty-state">{loading ? 'Carregando…' : 'Nenhuma compra registrada.'}</div>
          )}
        </div>
      </div>
    </section>
  )
}
