import { statusOptions } from '../../core/resources'

export const formatCurrency = (value) =>
  new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL',
  }).format(Number(value ?? 0))

export const formatDate = (value) => {
  if (!value) {
    return '—'
  }

  return new Intl.DateTimeFormat('pt-BR', {
    timeZone: 'UTC',
  }).format(new Date(value))
}

export const formatStatus = (key, value) =>
  statusOptions[key]?.find((option) => option.value === Number(value))?.label ?? value ?? '—'

// Cor semântica de cada rótulo de status exibido em tabelas.
const toneByLabel = {
  Ativo: 'ok',
  Confirmada: 'ok',
  Entrada: 'ok',
  Pendente: 'warn',
  'Aguardando entrega': 'warn',
  Inativo: 'neutral',
  Desabilitado: 'neutral',
  'Fora de estoque': 'bad',
  Cancelada: 'bad',
  Saída: 'bad',
}

export const statusTone = (label) => toneByLabel[label] ?? 'neutral'

export const isStatusColumn = (column) =>
  Boolean(column.format) && !['currency', 'date', 'image'].includes(column.format)

export const isNumericColumn = (column) =>
  column.format === 'currency' || /^(quantidade|qtd)/i.test(column.key) || /^(atual|m[íi]nima|m[áa]xima|anterior|posterior|qtd)/i.test(column.label)

export const formatCellValue = (column, row, lookups = {}) => {
  const value = row[column.key]

  if (column.lookup) {
    return lookups[column.lookup]?.labels.get(String(value)) ?? value ?? '—'
  }

  if (column.format === 'currency') {
    return formatCurrency(value)
  }

  if (column.format === 'date') {
    return formatDate(value)
  }

  if (column.format) {
    return formatStatus(column.format, value)
  }

  return value || value === 0 ? value : '—'
}
