import { useCallback, useEffect, useMemo, useState } from 'react'
import { resourceDefinitions } from '../../core/resources'
import { Drawer } from '../../shared/components/Drawer'
import { Icon } from '../../shared/components/Icon'
import { useResource } from '../../shared/hooks/useResource'
import { formatCurrency } from '../../shared/utils/formatters'
import { inventoryResources } from '../inventory/inventoryResources'
import './storefront.css'

const LOW_STOCK_LIMIT = 3
const PIX_DISCOUNT = 0.05
const MAX_INSTALLMENTS = 10
const MIN_INSTALLMENT = 30
const CART_KEY = 'boxpk.cart'

const SORTS = [
  { value: 'relevance', label: 'Relevância' },
  { value: 'price-asc', label: 'Menor preço' },
  { value: 'price-desc', label: 'Maior preço' },
  { value: 'name', label: 'Nome (A–Z)' },
]

const normalizeText = (value) =>
  String(value ?? '')
    .normalize('NFD')
    .replace(/[̀-ͯ]/g, '')
    .toLowerCase()
    .trim()

const byId = (items, idKey) => new Map(items.map((item) => [String(item[idKey]), item]))

const hueFor = (text) => {
  let hash = 0
  for (const char of String(text)) hash = (hash * 31 + char.charCodeAt(0)) % 360
  return hash
}

const initialsOf = (name) =>
  String(name ?? '')
    .split(/\s+/)
    .filter((word) => /[\p{L}\d]/u.test(word[0]))
    .slice(0, 2)
    .map((word) => word[0].toUpperCase())
    .join('')

const installmentsFor = (price) => {
  const count = Math.min(MAX_INSTALLMENTS, Math.max(1, Math.floor(price / MIN_INSTALLMENT)))
  return { count, value: price / count }
}

const pixPrice = (price) => price * (1 - PIX_DISCOUNT)

const readCart = () => {
  try {
    const parsed = JSON.parse(localStorage.getItem(CART_KEY) ?? '{}')
    return parsed && typeof parsed === 'object' ? parsed : {}
  } catch {
    return {}
  }
}

function ProductImage({ product, size = 'card' }) {
  if (product.imagemUrl) {
    return <img src={product.imagemUrl} alt={product.nome} loading="lazy" />
  }

  const hue = hueFor(product.jogoNome)
  return (
    <div
      className={`sf-placeholder sf-placeholder-${size}`}
      style={{ '--h': hue }}
      role="img"
      aria-label={product.nome}
    >
      <span>{initialsOf(product.nome)}</span>
      <small>{product.jogoNome}</small>
    </div>
  )
}

function Price({ value, large = false }) {
  const parcel = installmentsFor(value)

  return (
    <div className={large ? 'sf-price sf-price-lg' : 'sf-price'}>
      <strong>{formatCurrency(value)}</strong>
      <span className="sf-pix">
        {formatCurrency(pixPrice(value))} <em>no Pix</em>
      </span>
      <span className="sf-parcel">
        {parcel.count > 1 ? `ou ${parcel.count}x de ${formatCurrency(parcel.value)} sem juros` : 'à vista no cartão'}
      </span>
    </div>
  )
}

function Stepper({ value, max, onChange }) {
  return (
    <div className="sf-stepper" role="group" aria-label="Quantidade">
      <button type="button" onClick={() => onChange(value - 1)} aria-label="Diminuir" disabled={value <= 1}>
        <Icon name="minus" size={14} />
      </button>
      <span aria-live="polite">{value}</span>
      <button type="button" onClick={() => onChange(value + 1)} aria-label="Aumentar" disabled={value >= max}>
        <Icon name="plus" size={14} />
      </button>
    </div>
  )
}

export function StorefrontPage() {
  const [search, setSearch] = useState('')
  const [jogo, setJogo] = useState('')
  const [category, setCategory] = useState('')
  const [colecao, setColecao] = useState('')
  const [onlyAvailable, setOnlyAvailable] = useState(true)
  const [sort, setSort] = useState('relevance')
  const [filtersOpen, setFiltersOpen] = useState(false)
  const [selected, setSelected] = useState(null)
  const [detailQty, setDetailQty] = useState(1)
  const [cartOpen, setCartOpen] = useState(false)
  const [cart, setCart] = useState(readCart)
  const [copied, setCopied] = useState(false)

  const produtos = useResource(resourceDefinitions.produtos)
  const categorias = useResource(resourceDefinitions.categorias)
  const jogos = useResource(resourceDefinitions.jogos)
  const colecoes = useResource(resourceDefinitions.colecoes)
  const estoque = useResource(inventoryResources.estoque)

  useEffect(() => {
    try {
      localStorage.setItem(CART_KEY, JSON.stringify(cart))
    } catch {
      /* armazenamento indisponível: o carrinho vale só para esta sessão */
    }
  }, [cart])

  const categoriaById = useMemo(() => byId(categorias.items, 'idCategoria'), [categorias.items])
  const jogoById = useMemo(() => byId(jogos.items, 'idJogoTCG'), [jogos.items])
  const colecaoById = useMemo(() => byId(colecoes.items, 'idColecao'), [colecoes.items])
  const estoqueByProduto = useMemo(() => byId(estoque.items, 'idProduto'), [estoque.items])

  const catalog = useMemo(
    () =>
      produtos.items
        .filter((produto) => produto.statusProduto === undefined || Number(produto.statusProduto) === 1)
        .map((produto) => ({
          ...produto,
          preco: Number(produto.precoVenda ?? 0),
          categoriaNome: categoriaById.get(String(produto.idCategoria))?.nome ?? 'Sem categoria',
          jogoNome: jogoById.get(String(produto.idJogoTCG))?.nome ?? 'Sem jogo',
          colecaoNome: colecaoById.get(String(produto.idColecao))?.nome ?? '',
          quantidadeAtual: Number(estoqueByProduto.get(String(produto.idProduto))?.quantidadeAtual ?? 0),
        })),
    [categoriaById, colecaoById, estoqueByProduto, jogoById, produtos.items],
  )

  const catalogById = useMemo(() => byId(catalog, 'idProduto'), [catalog])

  const matches = useCallback(
    (produto, ignore = '') => {
      const term = normalizeText(search)
      return (
        (!term ||
          normalizeText(
            `${produto.nome} ${produto.descricao} ${produto.categoriaNome} ${produto.jogoNome} ${produto.colecaoNome}`,
          ).includes(term)) &&
        (ignore === 'jogo' || !jogo || String(produto.idJogoTCG) === jogo) &&
        (ignore === 'category' || !category || String(produto.idCategoria) === category) &&
        (ignore === 'colecao' || !colecao || String(produto.idColecao) === colecao) &&
        (!onlyAvailable || produto.quantidadeAtual > 0)
      )
    },
    [category, colecao, jogo, onlyAvailable, search],
  )

  const products = useMemo(() => {
    const list = catalog.filter((produto) => matches(produto))

    switch (sort) {
      case 'price-asc':
        return list.sort((a, b) => a.preco - b.preco)
      case 'price-desc':
        return list.sort((a, b) => b.preco - a.preco)
      case 'name':
        return list.sort((a, b) => a.nome.localeCompare(b.nome, 'pt-BR'))
      default:
        // Relevância: o que tem estoque primeiro, depois o mais recente cadastrado.
        return list.sort(
          (a, b) => Number(b.quantidadeAtual > 0) - Number(a.quantidadeAtual > 0) || b.idProduto - a.idProduto,
        )
    }
  }, [catalog, matches, sort])

  // Contagens de cada filtro consideram os demais filtros, como em lojas de verdade.
  const countBy = useCallback(
    (ignore, key) => {
      const counts = new Map()
      catalog.filter((produto) => matches(produto, ignore)).forEach((produto) => {
        const id = String(produto[key] ?? '')
        counts.set(id, (counts.get(id) ?? 0) + 1)
      })
      return counts
    },
    [catalog, matches],
  )

  const jogoCounts = useMemo(() => countBy('jogo', 'idJogoTCG'), [countBy])
  const categoryCounts = useMemo(() => countBy('category', 'idCategoria'), [countBy])
  const colecaoCounts = useMemo(() => countBy('colecao', 'idColecao'), [countBy])
  const totalForGames = useMemo(
    () => catalog.filter((produto) => matches(produto, 'jogo')).length,
    [catalog, matches],
  )

  const cartLines = useMemo(
    () =>
      Object.entries(cart)
        .map(([id, quantity]) => ({ product: catalogById.get(id), quantity: Number(quantity) }))
        .filter((line) => line.product && line.quantity > 0),
    [cart, catalogById],
  )
  const cartCount = cartLines.reduce((sum, line) => sum + line.quantity, 0)
  const cartTotal = cartLines.reduce((sum, line) => sum + line.quantity * line.product.preco, 0)

  const setQuantity = (product, quantity) => {
    const next = Math.max(0, Math.min(quantity, product.quantidadeAtual))
    setCart((current) => {
      const copy = { ...current }
      if (next <= 0) delete copy[product.idProduto]
      else copy[product.idProduto] = next
      return copy
    })
  }

  const addToCart = (product, amount = 1) => {
    setQuantity(product, (Number(cart[product.idProduto]) || 0) + amount)
  }

  const openDetail = (product) => {
    setSelected(product)
    setDetailQty(1)
  }

  const copySummary = async () => {
    const lines = cartLines.map(
      ({ product, quantity }) => `${quantity}x ${product.nome} — ${formatCurrency(product.preco * quantity)}`,
    )
    const text = `Pedido BoxPK\n${lines.join('\n')}\nTotal: ${formatCurrency(cartTotal)} (Pix: ${formatCurrency(pixPrice(cartTotal))})`

    try {
      await navigator.clipboard.writeText(text)
      setCopied(true)
      setTimeout(() => setCopied(false), 2200)
    } catch {
      window.prompt('Copie o resumo do pedido:', text)
    }
  }

  const clearFilters = () => {
    setSearch('')
    setJogo('')
    setCategory('')
    setColecao('')
    setOnlyAvailable(true)
  }

  const loading = produtos.loading || categorias.loading || jogos.loading || estoque.loading
  const error = produtos.error || categorias.error || jogos.error || estoque.error
  const hasFilters = search || jogo || category || colecao || !onlyAvailable
  const selectedStock = selected ? selected.quantidadeAtual : 0

  const filterBlocks = (
    <>
      <section className="sf-filter">
        <h3>Categorias</h3>
        <ul>
          <li>
            <button type="button" className={!category ? 'is-active' : ''} onClick={() => setCategory('')}>
              Todas <span>{[...categoryCounts.values()].reduce((a, b) => a + b, 0)}</span>
            </button>
          </li>
          {categorias.items.map((item) => (
            <li key={item.idCategoria}>
              <button
                type="button"
                className={category === String(item.idCategoria) ? 'is-active' : ''}
                onClick={() => setCategory(String(item.idCategoria))}
                disabled={!categoryCounts.get(String(item.idCategoria))}
              >
                {item.nome} <span>{categoryCounts.get(String(item.idCategoria)) ?? 0}</span>
              </button>
            </li>
          ))}
        </ul>
      </section>

      <section className="sf-filter">
        <h3>Coleção</h3>
        <select value={colecao} onChange={(event) => setColecao(event.target.value)} aria-label="Coleção">
          <option value="">Todas as coleções</option>
          {colecoes.items
            .filter((item) => colecaoCounts.get(String(item.idColecao)))
            .map((item) => (
              <option value={item.idColecao} key={item.idColecao}>
                {item.nome} ({colecaoCounts.get(String(item.idColecao))})
              </option>
            ))}
        </select>
      </section>

      <section className="sf-filter">
        <label className="sf-check">
          <input type="checkbox" checked={onlyAvailable} onChange={(event) => setOnlyAvailable(event.target.checked)} />
          Mostrar somente itens em estoque
        </label>
      </section>

      {hasFilters ? (
        <button type="button" className="btn btn-block" onClick={clearFilters}>
          Limpar filtros
        </button>
      ) : null}
    </>
  )

  return (
    <section className="sf">
      <div className="sf-hero">
        <div className="sf-hero-text">
          <span className="eyebrow">Loja BoxPK</span>
          <h1>Boxes, boosters e acessórios</h1>
          <p>Produtos selados, pronto para jogar ou colecionar. Pix com {PIX_DISCOUNT * 100}% de desconto.</p>
        </div>
        <div className="sf-hero-tools">
          <label className="sf-search">
            <Icon name="search" size={18} />
            <input
              type="search"
              value={search}
              onChange={(event) => setSearch(event.target.value)}
              placeholder="Buscar por produto, jogo ou coleção"
              aria-label="Buscar produto"
            />
          </label>
          <button type="button" className="sf-cart-btn" onClick={() => setCartOpen(true)}>
            <Icon name="cart" size={18} />
            <span>Carrinho</span>
            {cartCount ? <b>{cartCount}</b> : null}
          </button>
        </div>
      </div>

      <nav className="sf-tabs" aria-label="Jogos">
        <button type="button" className={!jogo ? 'is-active' : ''} onClick={() => setJogo('')}>
          Todos os jogos <span>{totalForGames}</span>
        </button>
        {jogos.items.map((item) => (
          <button
            type="button"
            key={item.idJogoTCG}
            className={jogo === String(item.idJogoTCG) ? 'is-active' : ''}
            onClick={() => setJogo(String(item.idJogoTCG))}
          >
            {item.nome} <span>{jogoCounts.get(String(item.idJogoTCG)) ?? 0}</span>
          </button>
        ))}
      </nav>

      {error ? <div className="alert alert-error">{error}</div> : null}

      <div className="sf-layout">
        <aside className={filtersOpen ? 'sf-sidebar is-open' : 'sf-sidebar'}>{filterBlocks}</aside>

        <div className="sf-results">
          <div className="sf-bar">
            <button type="button" className="btn sf-filter-toggle" onClick={() => setFiltersOpen((open) => !open)}>
              <Icon name="list" size={16} /> Filtros
            </button>
            <span className="sf-count">
              {loading ? 'Carregando…' : `${products.length} ${products.length === 1 ? 'produto' : 'produtos'}`}
            </span>
            <label className="sf-sort">
              <span>Ordenar por</span>
              <select value={sort} onChange={(event) => setSort(event.target.value)}>
                {SORTS.map((option) => (
                  <option value={option.value} key={option.value}>
                    {option.label}
                  </option>
                ))}
              </select>
            </label>
          </div>

          {!loading && !products.length ? (
            <div className="sf-empty">
              <Icon name="search" size={28} />
              <strong>Nenhum produto encontrado</strong>
              <p>Tente outro termo ou remova alguns filtros.</p>
              {hasFilters ? (
                <button type="button" className="btn" onClick={clearFilters}>
                  Limpar filtros
                </button>
              ) : null}
            </div>
          ) : null}

          <div className="sf-grid">
            {products.map((produto) => {
              const soldOut = produto.quantidadeAtual <= 0
              const low = !soldOut && produto.quantidadeAtual <= LOW_STOCK_LIMIT
              const inCart = Number(cart[produto.idProduto]) || 0

              return (
                <article className={soldOut ? 'sf-card is-soldout' : 'sf-card'} key={produto.idProduto}>
                  <button type="button" className="sf-card-open" onClick={() => openDetail(produto)}>
                    <div className="sf-media">
                      <ProductImage product={produto} />
                      {soldOut ? <span className="sf-tag sf-tag-dark">Esgotado</span> : null}
                      {low ? <span className="sf-tag sf-tag-warn">Últimas {produto.quantidadeAtual} un.</span> : null}
                    </div>
                    <div className="sf-info">
                      <span className="sf-kicker">{produto.colecaoNome || produto.categoriaNome}</span>
                      <h2>{produto.nome}</h2>
                      <Price value={produto.preco} />
                    </div>
                  </button>
                  <div className="sf-card-foot">
                    <button
                      type="button"
                      className="btn btn-primary btn-block"
                      onClick={() => addToCart(produto)}
                      disabled={soldOut || inCart >= produto.quantidadeAtual}
                    >
                      <Icon name="cart" size={16} />
                      {soldOut ? 'Indisponível' : inCart >= produto.quantidadeAtual ? 'Máximo no carrinho' : 'Adicionar'}
                    </button>
                  </div>
                </article>
              )
            })}
          </div>
        </div>
      </div>

      <Drawer
        open={Boolean(selected)}
        title={selected?.nome ?? ''}
        subtitle={selected ? `${selected.jogoNome} · ${selected.categoriaNome}` : ''}
        onClose={() => setSelected(null)}
      >
        {selected ? (
          <>
            <div className="drawer-body sf-detail">
              <div className="sf-detail-media">
                <ProductImage product={selected} size="large" />
              </div>
              <Price value={selected.preco} large />
              <p className={selectedStock <= 0 ? 'sf-avail is-out' : selectedStock <= LOW_STOCK_LIMIT ? 'sf-avail is-low' : 'sf-avail'}>
                {selectedStock <= 0
                  ? 'Produto esgotado'
                  : selectedStock <= LOW_STOCK_LIMIT
                    ? `Restam apenas ${selectedStock} unidades`
                    : 'Em estoque'}
              </p>
              {selected.descricao ? <p className="sf-desc">{selected.descricao}</p> : null}
              <dl className="sf-specs">
                <div>
                  <dt>Jogo</dt>
                  <dd>{selected.jogoNome}</dd>
                </div>
                <div>
                  <dt>Categoria</dt>
                  <dd>{selected.categoriaNome}</dd>
                </div>
                {selected.colecaoNome ? (
                  <div>
                    <dt>Coleção</dt>
                    <dd>{selected.colecaoNome}</dd>
                  </div>
                ) : null}
                <div>
                  <dt>Código</dt>
                  <dd>#{String(selected.idProduto).padStart(5, '0')}</dd>
                </div>
              </dl>
            </div>
            <footer className="drawer-footer sf-detail-foot">
              {selectedStock > 0 ? <Stepper value={detailQty} max={selectedStock} onChange={setDetailQty} /> : null}
              <button
                type="button"
                className="btn btn-primary"
                disabled={selectedStock <= 0}
                onClick={() => {
                  addToCart(selected, detailQty)
                  setSelected(null)
                  setCartOpen(true)
                }}
              >
                <Icon name="cart" size={16} /> Adicionar ao carrinho
              </button>
            </footer>
          </>
        ) : null}
      </Drawer>

      <Drawer
        open={cartOpen}
        title="Carrinho"
        subtitle={cartCount ? `${cartCount} ${cartCount === 1 ? 'item' : 'itens'}` : ''}
        onClose={() => setCartOpen(false)}
      >
        {cartLines.length ? (
          <>
            <ul className="drawer-body sf-cart">
              {cartLines.map(({ product, quantity }) => (
                <li key={product.idProduto}>
                  <div className="sf-cart-thumb">
                    <ProductImage product={product} size="thumb" />
                  </div>
                  <div className="sf-cart-info">
                    <strong>{product.nome}</strong>
                    <span>{formatCurrency(product.preco)} cada</span>
                    <Stepper value={quantity} max={product.quantidadeAtual} onChange={(value) => setQuantity(product, value)} />
                  </div>
                  <div className="sf-cart-side">
                    <b>{formatCurrency(product.preco * quantity)}</b>
                    <button type="button" className="icon-button" onClick={() => setQuantity(product, 0)} aria-label={`Remover ${product.nome}`}>
                      <Icon name="trash" size={16} />
                    </button>
                  </div>
                </li>
              ))}
            </ul>
            <footer className="drawer-footer sf-cart-foot">
              <dl>
                <div>
                  <dt>Total</dt>
                  <dd>{formatCurrency(cartTotal)}</dd>
                </div>
                <div className="sf-cart-pix">
                  <dt>No Pix ({PIX_DISCOUNT * 100}% off)</dt>
                  <dd>{formatCurrency(pixPrice(cartTotal))}</dd>
                </div>
              </dl>
              <button type="button" className="btn btn-primary btn-block" onClick={copySummary}>
                <Icon name={copied ? 'check' : 'receipt'} size={16} />
                {copied ? 'Resumo copiado' : 'Copiar resumo do pedido'}
              </button>
              <button type="button" className="btn btn-block" onClick={() => setCart({})}>
                Esvaziar carrinho
              </button>
            </footer>
          </>
        ) : (
          <div className="drawer-body sf-empty">
            <Icon name="cart" size={28} />
            <strong>Seu carrinho está vazio</strong>
            <p>Adicione produtos da vitrine para montar o pedido.</p>
          </div>
        )}
      </Drawer>
    </section>
  )
}
