import { formatCellValue } from '../utils/formatters'

export function DataTable({ columns, idKey, rows, loading, lookups, onEdit, onDelete, emptyLabel }) {
  if (loading) {
    return <div className="empty-state">Carregando dados...</div>
  }

  if (!rows.length) {
    return <div className="empty-state">Nenhum {emptyLabel} cadastrado.</div>
  }

  return (
    <div className="table-wrap">
      <table>
        <thead>
          <tr>
            {columns.map((column) => (
              <th key={column.key}>{column.label}</th>
            ))}
            <th>Acoes</th>
          </tr>
        </thead>
        <tbody>
          {rows.map((row) => (
            <tr key={row[idKey]}>
              {columns.map((column) => (
                <td key={column.key}>{formatCellValue(column, row, lookups)}</td>
              ))}
              <td className="actions-cell">
                <button type="button" className="icon-button" onClick={() => onEdit(row)} title="Editar">
                  E
                </button>
                <button
                  type="button"
                  className="icon-button danger"
                  onClick={() => onDelete(row[idKey])}
                  title="Excluir"
                >
                  X
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
