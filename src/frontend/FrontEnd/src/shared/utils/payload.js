const numericFieldNames = new Set([
  'idCategoria',
  'idJogoTCG',
  'idColecao',
  'idProduto',
  'idFornecedor',
  'idCompra',
  'quantidadeAtual',
  'quantidadeMinima',
  'quantidadeMaxima',
  'quantidade',
  'precoCusto',
  'precoVenda',
  'valorTotal',
  'valorUnitario',
  'tipoMovimentacao',
  'statusCategoria',
  'statusFornecedor',
  'statusJogo',
  'statusColecao',
  'statusProduto',
  'statusCompra',
])

export const normalizePayload = (values) =>
  Object.entries(values).reduce((payload, [key, value]) => {
    if (value === '') {
      payload[key] = key.startsWith('id') && key !== 'idJogoTCG' ? null : ''
      return payload
    }

    payload[key] = numericFieldNames.has(key) ? Number(value) : value
    return payload
  }, {})

export const toDateInputValue = (value) => {
  if (!value) {
    return ''
  }

  return new Date(value).toISOString().slice(0, 10)
}
