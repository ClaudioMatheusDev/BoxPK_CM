import { useMemo, useState } from 'react'
import {
  formatCellValue,
  isNumericColumn,
  isStatusColumn,
  statusTone,
} from '../utils/formatters'
import { Badge } from './Badge'
import { Icon } from './Icon'

const collator = new Intl.Collator('pt-BR', { numeric: true, sensitivity: 'base' })

function Cell({ column, row, lookups }) {
  const value = formatCellValue(column, row, lookups)

  if (column.format === 'image') {
    return row[column.key] ? (
      <img className="thumb" src={row[column.key]} alt="" loading="lazy" />
    ) : (
      <span className="thumb thumb-empty">
        <Icon name="image" size={16} />
      </span>
    )
  }

  if (isStatusColumn(column)) {
    return <Badge tone={statusTone(value)}>{value}</Badge>
  }

  return value
}

export function DataTable({ columns, idKey, rows, loading, lookups, onEdit, onDelete, emptyLabel, filtered }) {
  const [sort, setSort] = useState({ key: null, dir: 1 })

  const sortedRows = useMemo(() => {
    if (!sort.key) {
      return rows
    }

    const column = columns.find((item) => item.key === sort.key)

    return [...rows].sort(
      (a, b) =>
        sort.dir * collator.compare(String(formatCellValue(column, a, lookups)), String(formatCellValue(column, b, lookups))),
    )
  }, [columns, lookups, rows, sort])

  const toggleSort = (key) =>
    setSort((current) => (current.key === key ? { key, dir: -current.dir } : { key, dir: 1 }))

  if (loading) {
    return <div className="empty-state">Carregando dados…</div>
  }

  if (!rows.length) {
    return (
      <div className="empty-state">
        {filtered ? 'Nenhum resultado para a busca.' : `Nenhum(a) ${emptyLabel} cadastrado(a) até o momento.`}
      </div>
    )
  }

  return (
    <div className="table-wrap">
      <table className="table">
        <thead>
          <tr>
            {columns.map((column) => {
              const active = sort.key === column.key

              return (
                <th
                  key={column.key}
                  className={isNumericColumn(column) ? 'num' : ''}
                  aria-sort={active ? (sort.dir === 1 ? 'ascending' : 'descending') : 'none'}
                >
                  <button type="button" className="th-button" onClick={() => toggleSort(column.key)}>
                    {column.label}
                    <span className={`sort-mark${active ? ' on' : ''}`}>{active && sort.dir === -1 ? '↓' : '↑'}</span>
                  </button>
                </th>
              )
            })}
            <th className="col-actions">
              <span className="sr-only">Ações</span>
            </th>
          </tr>
        </thead>
        <tbody>
          {sortedRows.map((row) => (
            <tr key={row[idKey]}>
              {columns.map((column) => (
                <td
                  key={column.key}
                  className={[
                    isNumericColumn(column) ? 'num' : '',
                    column.key === idKey ? 'cell-id' : '',
                  ]
                    .filter(Boolean)
                    .join(' ')}
                >
                  <Cell column={column} row={row} lookups={lookups} />
                </td>
              ))}
              <td className="col-actions">
                <button type="button" className="icon-button" onClick={() => onEdit(row)} title="Editar" aria-label="Editar">
                  <Icon name="edit" size={16} />
                </button>
                <button
                  type="button"
                  className="icon-button icon-danger"
                  onClick={() => onDelete(row[idKey])}
                  title="Excluir"
                  aria-label="Excluir"
                >
                  <Icon name="trash" size={16} />
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
