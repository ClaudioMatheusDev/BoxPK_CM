import { useEffect, useMemo, useState } from 'react'
import { statusOptions } from '../../core/resources'
import { normalizePayload, toDateInputValue } from '../utils/payload'

const getInitialValues = (definition, editingItem) => {
  const defaults = { ...definition.createDefaults }

  if (!editingItem) {
    return defaults
  }

  const values = Object.keys(defaults).reduce((acc, key) => {
    acc[key] = key.toLowerCase().includes('data')
      ? toDateInputValue(editingItem[key])
      : (editingItem[key] ?? '')

    return acc
  }, {})

  if (definition.updateStatusKey) {
    values[definition.updateStatusKey] = editingItem[definition.updateStatusKey] ?? 1
  }

  return values
}

export function ResourceForm({
  definition,
  editingItem,
  lookupLoading,
  lookups,
  onCancel,
  onSubmit,
}) {
  const initialValues = useMemo(
    () => getInitialValues(definition, editingItem),
    [definition, editingItem],
  )
  const [values, setValues] = useState(initialValues)

  useEffect(() => {
    setValues(initialValues)
  }, [initialValues])

  const fields = editingItem && definition.updateStatusKey
    ? [
        ...definition.fields,
        {
          name: definition.updateStatusKey,
          label: 'Status',
          type: 'select',
          options: statusOptions[definition.updateStatusKey] ?? [],
        },
      ]
    : definition.fields

  const handleChange = (event) => {
    const { name, value } = event.target
    setValues((current) => ({ ...current, [name]: value }))
  }

  const handleSubmit = async (event) => {
    event.preventDefault()
    const ok = await onSubmit({
      id: editingItem?.[definition.idKey],
      payload: normalizePayload(values),
    })

    if (ok && !editingItem) {
      setValues(getInitialValues(definition))
    }
  }

  return (
    <form className="form-panel" onSubmit={handleSubmit}>
      <div className="panel-header">
        <div>
          <span>{editingItem ? 'Edicao' : 'Novo registro'}</span>
          <h2>{definition.title}</h2>
        </div>
        {editingItem ? (
          <button type="button" className="ghost-button" onClick={onCancel}>
            Cancelar
          </button>
        ) : null}
      </div>

      <div className="form-grid">
        {fields.map((field) => {
          const lookup = field.lookup ? lookups[field.lookup] : null
          const options = lookup?.options ?? field.options ?? []
          const isLookupSelect = Boolean(field.lookup)
          const hasNoLookupOptions = isLookupSelect && !lookupLoading && options.length === 0

          return (
          <label
            className={field.type === 'textarea' ? 'field field-wide' : 'field'}
            key={field.name}
          >
            <span>{field.label}</span>
            {field.type === 'textarea' ? (
              <textarea
                name={field.name}
                value={values[field.name] ?? ''}
                onChange={handleChange}
                required={field.required}
                rows={3}
              />
            ) : field.type === 'select' || isLookupSelect ? (
              <select
                name={field.name}
                value={values[field.name] ?? ''}
                onChange={handleChange}
                required={field.required}
                disabled={isLookupSelect && lookupLoading}
              >
                {isLookupSelect || !options.length ? (
                  <option value="">
                    {hasNoLookupOptions
                      ? 'Cadastre opcoes primeiro'
                      : isLookupSelect && lookupLoading
                        ? 'Carregando...'
                        : 'Selecione'}
                  </option>
                ) : null}
                {options.map((option) => (
                  <option value={option.value} key={option.value}>
                    {option.label}
                  </option>
                ))}
              </select>
            ) : (
              <input
                name={field.name}
                type={field.type ?? 'text'}
                step={field.step}
                value={values[field.name] ?? ''}
                onChange={handleChange}
                required={field.required}
              />
            )}
            {hasNoLookupOptions ? (
              <small className="field-help">Nenhum registro disponivel para selecionar.</small>
            ) : null}
          </label>
          )
        })}
      </div>

      <button type="submit" className="primary-button">
        {editingItem ? 'Salvar alteracoes' : 'Cadastrar'}
      </button>
    </form>
  )
}
