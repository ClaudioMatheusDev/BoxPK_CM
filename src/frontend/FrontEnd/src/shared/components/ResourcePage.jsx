import { useMemo, useState } from 'react'
import { useResource } from '../hooks/useResource'
import { Alert } from './Alert'
import { DataTable } from './DataTable'
import { ResourceForm } from './ResourceForm'
import { useLookups } from '../hooks/useLookups'

export function ResourcePage({ definition, resourcesByKey }) {
  const [editingItem, setEditingItem] = useState(null)
  const resource = useResource(definition)
  const lookups = useLookups(definition, resourcesByKey)
  const totalLabel = useMemo(() => `${resource.items.length} registros`, [resource.items.length])

  const handleDelete = (id) => {
    const shouldDelete = window.confirm('Deseja remover este registro?')

    if (shouldDelete) {
      resource.deleteItem(id)
    }
  }

  const handleSubmit = async (params) => {
    const ok = await resource.saveItem(params)

    if (ok) {
      setEditingItem(null)
    }

    return ok
  }

  return (
    <section className="page-grid">
      <div className="resource-main">
        <div className="page-title">
          <div>
            <span>Modulo</span>
            <h1>{definition.title}</h1>
          </div>
          <button type="button" className="secondary-button" onClick={resource.reload}>
            Atualizar
          </button>
        </div>

        <Alert type="error">{resource.error}</Alert>
        <Alert type="error">{lookups.error}</Alert>
        <Alert type="success">{resource.message}</Alert>

        <div className="list-panel">
          <div className="panel-header">
            <div>
              <span>Lista</span>
              <h2>{totalLabel}</h2>
            </div>
          </div>

          <DataTable
            columns={definition.columns}
            emptyLabel={definition.emptyLabel}
            idKey={definition.idKey}
            loading={resource.loading}
            lookups={lookups.data}
            rows={resource.items}
            onDelete={handleDelete}
            onEdit={setEditingItem}
          />
        </div>
      </div>

      <ResourceForm
        definition={definition}
        editingItem={editingItem}
        lookupLoading={lookups.loading}
        lookups={lookups.data}
        onCancel={() => setEditingItem(null)}
        onSubmit={handleSubmit}
      />
    </section>
  )
}
