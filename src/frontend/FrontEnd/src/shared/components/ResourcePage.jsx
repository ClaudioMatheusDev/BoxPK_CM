import { useMemo, useState } from 'react'
import { useLookups } from '../hooks/useLookups'
import { useResource } from '../hooks/useResource'
import { formatCellValue } from '../utils/formatters'
import { Alert } from './Alert'
import { ConfirmDialog } from './ConfirmDialog'
import { DataTable } from './DataTable'
import { Drawer } from './Drawer'
import { Icon } from './Icon'
import { ResourceForm } from './ResourceForm'

const normalize = (value) =>
  String(value ?? '')
    .normalize('NFD')
    .replace(/[̀-ͯ]/g, '')
    .toLowerCase()

export function ResourcePage({ definition, resourcesByKey }) {
  // editor: null (fechado) | { item: null } (novo) | { item: {...} } (edição)
  const [editor, setEditor] = useState(null)
  const [pendingDelete, setPendingDelete] = useState(null)
  const [query, setQuery] = useState('')
  const [dismissed, setDismissed] = useState({})
  const resource = useResource(definition)
  const lookups = useLookups(definition, resourcesByKey)
  const errorText = resource.error || lookups.error

  const visibleRows = useMemo(() => {
    const term = normalize(query.trim())

    if (!term) {
      return resource.items
    }

    return resource.items.filter((row) =>
      definition.columns.some((column) =>
        normalize(formatCellValue(column, row, lookups.data)).includes(term),
      ),
    )
  }, [definition.columns, lookups.data, query, resource.items])

  const closeEditor = () => setEditor(null)

  const handleSubmit = async (params) => {
    setDismissed({})
    const ok = await resource.saveItem(params)

    if (ok) {
      closeEditor()
    }

    return ok
  }

  const confirmDelete = async () => {
    const id = pendingDelete
    setPendingDelete(null)
    setDismissed({})
    await resource.deleteItem(id)
  }

  const counter =
    visibleRows.length === resource.items.length
      ? `${resource.items.length} ${resource.items.length === 1 ? 'registro' : 'registros'}`
      : `${visibleRows.length} de ${resource.items.length} registros`

  return (
    <section className="page">
      <header className="page-header">
        <div>
          <h1>{definition.title}</h1>
          <p className="page-sub">{resource.loading ? 'Carregando…' : counter}</p>
        </div>
        <div className="page-actions">
          <button type="button" className="btn" onClick={resource.reload} disabled={resource.loading}>
            <Icon name="refresh" size={16} />
            Atualizar
          </button>
          <button type="button" className="btn btn-primary" onClick={() => setEditor({ item: null })}>
            <Icon name="plus" size={16} />
            Novo registro
          </button>
        </div>
      </header>

      <div className="stack">
        {errorText !== dismissed.error ? (
          <Alert type="error" onDismiss={() => setDismissed((d) => ({ ...d, error: errorText }))}>
            {errorText}
          </Alert>
        ) : null}
        {resource.message !== dismissed.message ? (
          <Alert type="success" onDismiss={() => setDismissed((d) => ({ ...d, message: resource.message }))}>
            {resource.message}
          </Alert>
        ) : null}
      </div>

      <div className="panel">
        <div className="panel-toolbar">
          <label className="search">
            <Icon name="search" size={16} />
            <input
              type="search"
              value={query}
              onChange={(event) => setQuery(event.target.value)}
              placeholder={`Buscar em ${definition.title.toLowerCase()}`}
              aria-label="Buscar"
            />
          </label>
        </div>

        <DataTable
          columns={definition.columns}
          emptyLabel={definition.emptyLabel}
          filtered={Boolean(query.trim())}
          idKey={definition.idKey}
          loading={resource.loading}
          lookups={lookups.data}
          rows={visibleRows}
          onDelete={setPendingDelete}
          onEdit={(item) => setEditor({ item })}
        />
      </div>

      <Drawer
        open={Boolean(editor)}
        onClose={closeEditor}
        subtitle={editor?.item ? 'Editar registro' : 'Novo registro'}
        title={definition.title}
      >
        {editor ? (
          <ResourceForm
            key={editor.item?.[definition.idKey] ?? 'novo'}
            definition={definition}
            editingItem={editor.item}
            lookupLoading={lookups.loading}
            lookups={lookups.data}
            serverError={resource.error}
            onCancel={closeEditor}
            onSubmit={handleSubmit}
          />
        ) : null}
      </Drawer>

      <ConfirmDialog
        open={pendingDelete !== null}
        title="Excluir registro"
        message="Esta ação não pode ser desfeita. Deseja realmente excluir este registro?"
        onCancel={() => setPendingDelete(null)}
        onConfirm={confirmDelete}
      />
    </section>
  )
}
