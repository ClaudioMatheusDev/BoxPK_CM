import { statusOptions } from '../../core/resources'

export const formatCurrency = (value) =>
  new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL',
  }).format(Number(value ?? 0))

export const formatDate = (value) => {
  if (!value) {
    return '-'
  }

  return new Intl.DateTimeFormat('pt-BR', {
    timeZone: 'UTC',
  }).format(new Date(value))
}

export const formatStatus = (key, value) =>
  statusOptions[key]?.find((option) => option.value === Number(value))?.label ?? value ?? '-'

export const formatCellValue = (column, row, lookups = {}) => {
  const value = row[column.key]

  if (column.lookup) {
    return lookups[column.lookup]?.labels.get(String(value)) ?? value ?? '-'
  }

  if (column.format === 'currency') {
    return formatCurrency(value)
  }

  if (column.format === 'date') {
    return formatDate(value)
  }

  if (column.format === 'image') {
    return value ? 'Com imagem' : 'Sem imagem'
  }

  if (column.format) {
    return formatStatus(column.format, value)
  }

  return value || value === 0 ? value : '-'
}
