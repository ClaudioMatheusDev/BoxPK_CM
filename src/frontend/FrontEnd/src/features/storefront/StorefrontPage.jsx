import { useMemo, useState } from 'react'
import { resourceDefinitions } from '../../core/resources'
import { inventoryResources } from '../inventory/inventoryResources'
import { useResource } from '../../shared/hooks/useResource'
import { formatCurrency } from '../../shared/utils/formatters'

const normalizeText = (value) => String(value ?? '').toLowerCase().trim()

const getById = (items, idKey) =>
  new Map(items.map((item) => [String(item[idKey]), item]))

const getProductImageUrl = (produto) =>
  produto.imagemUrl ?? produto.ImagemUrl ?? produto.imageUrl ?? produto.urlImagem ?? ''

export function StorefrontPage() {
  const [search, setSearch] = useState('')
  const [category, setCategory] = useState('')
  const [stockMode, setStockMode] = useState('available')

  const produtos = useResource(resourceDefinitions.produtos)
  const categorias = useResource(resourceDefinitions.categorias)
  const jogos = useResource(resourceDefinitions.jogos)
  const estoque = useResource(inventoryResources.estoque)

  const categoriaById = useMemo(
    () => getById(categorias.items, 'idCategoria'),
    [categorias.items],
  )
  const jogoById = useMemo(() => getById(jogos.items, 'idJogoTCG'), [jogos.items])
  const estoqueByProduto = useMemo(
    () => getById(estoque.items, 'idProduto'),
    [estoque.items],
  )

  const products = useMemo(() => {
    const term = normalizeText(search)

    return produtos.items
      .map((produto) => {
        const stock = estoqueByProduto.get(String(produto.idProduto))
        const categoriaProduto = categoriaById.get(String(produto.idCategoria))
        const jogoProduto = jogoById.get(String(produto.idJogoTCG))
        const quantidadeAtual = Number(stock?.quantidadeAtual ?? 0)

        return {
          ...produto,
          categoriaNome: categoriaProduto?.nome ?? 'Sem categoria',
          jogoNome: jogoProduto?.nome ?? 'Sem jogo',
          imagemUrl: getProductImageUrl(produto),
          quantidadeAtual,
        }
      })
      .filter((produto) => {
        const matchesSearch = !term || normalizeText(
          `${produto.nome} ${produto.descricao} ${produto.categoriaNome} ${produto.jogoNome}`,
        ).includes(term)
        const matchesCategory = !category || String(produto.idCategoria) === category
        const matchesStock =
          stockMode === 'all' ||
          (stockMode === 'available' && produto.quantidadeAtual > 0) ||
          (stockMode === 'low' && produto.quantidadeAtual > 0 && produto.quantidadeAtual <= 3) ||
          (stockMode === 'empty' && produto.quantidadeAtual <= 0)

        return matchesSearch && matchesCategory && matchesStock
      })
  }, [categoriaById, category, estoqueByProduto, jogoById, produtos.items, search, stockMode])

  const loading = produtos.loading || categorias.loading || jogos.loading || estoque.loading
  const error = produtos.error || categorias.error || jogos.error || estoque.error

  return (
    <section className="storefront">
      <div className="store-hero">
        <div>
          <span>Vitrine</span>
          <h1>Produtos em estoque</h1>
        </div>
        <strong>{products.length} produtos</strong>
      </div>

      <div className="store-toolbar">
        <label className="field">
          <span>Buscar</span>
          <input
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder="Nome, categoria ou jogo"
          />
        </label>
        <label className="field">
          <span>Categoria</span>
          <select value={category} onChange={(event) => setCategory(event.target.value)}>
            <option value="">Todas</option>
            {categorias.items.map((item) => (
              <option value={item.idCategoria} key={item.idCategoria}>
                {item.nome}
              </option>
            ))}
          </select>
        </label>
        <label className="field">
          <span>Estoque</span>
          <select value={stockMode} onChange={(event) => setStockMode(event.target.value)}>
            <option value="available">Disponiveis</option>
            <option value="low">Baixo estoque</option>
            <option value="empty">Sem estoque</option>
            <option value="all">Todos</option>
          </select>
        </label>
      </div>

      {error ? <div className="empty-state">{error}</div> : null}
      {loading ? <div className="empty-state">Carregando vitrine...</div> : null}

      {!loading && !products.length ? (
        <div className="empty-state">Nenhum produto encontrado.</div>
      ) : null}

      <div className="product-grid">
        {products.map((produto) => (
          <article className="product-card" key={produto.idProduto}>
            <div className="product-media">
              {produto.imagemUrl ? (
                <img src={produto.imagemUrl} alt={produto.nome} loading="lazy" />
              ) : (
                <div className="product-placeholder">{produto.nome?.slice(0, 2) ?? 'PK'}</div>
              )}
            </div>
            <div className="product-body">
              <div>
                <span>{produto.categoriaNome}</span>
                <h2>{produto.nome}</h2>
              </div>
              <p>{produto.descricao || produto.jogoNome}</p>
              <div className="product-footer">
                <strong>{formatCurrency(produto.precoVenda)}</strong>
                <span className={produto.quantidadeAtual > 0 ? 'stock-pill' : 'stock-pill empty'}>
                  {produto.quantidadeAtual} em estoque
                </span>
              </div>
            </div>
          </article>
        ))}
      </div>
    </section>
  )
}
