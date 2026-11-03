import { useMemo, useState } from 'react'
import { statusOptions } from '../../core/resources'
import { normalizePayload, toDateInputValue } from '../utils/payload'
import { Alert } from './Alert'

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
  serverError,
  onCancel,
  onSubmit,
}) {
  const initialValues = useMemo(
    () => getInitialValues(definition, editingItem),
    [definition, editingItem],
  )
  const [values, setValues] = useState(initialValues)
  const [saving, setSaving] = useState(false)
  const [failed, setFailed] = useState(false)

  const fields =
    editingItem && definition.updateStatusKey
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
    setSaving(true)
    setFailed(false)

    const ok = await onSubmit({
      id: editingItem?.[definition.idKey],
      payload: normalizePayload(values),
    })

    if (!ok) {
      setSaving(false)
      setFailed(true)
    }
  }

  return (
    <form className="drawer-form" onSubmit={handleSubmit}>
      <div className="drawer-body">
        {failed ? <Alert type="error">{serverError || 'Não foi possível salvar o registro.'}</Alert> : null}

        <div className="form-grid">
          {fields.map((field) => {
            const lookup = field.lookup ? lookups[field.lookup] : null
            const options = lookup?.options ?? field.options ?? []
            const isLookupSelect = Boolean(field.lookup)
            const hasNoLookupOptions = isLookupSelect && !lookupLoading && options.length === 0
            // Somente campos numéricos e de data ficam lado a lado; os demais ocupam a linha toda.
            const wide = !['number', 'date'].includes(field.type)

            return (
              <label className={wide ? 'field field-wide' : 'field'} key={field.name}>
                <span className="field-label">
                  {field.label}
                  {field.required ? <i className="req" title="Obrigatório"> *</i> : null}
                </span>

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
                          ? 'Cadastre opções primeiro'
                          : isLookupSelect && lookupLoading
                            ? 'Carregando…'
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
                    maxLength={field.maxLength}
                    placeholder={field.placeholder}
                    value={values[field.name] ?? ''}
                    onChange={handleChange}
                    required={field.required}
                  />
                )}

                {hasNoLookupOptions ? (
                  <small className="field-help field-help-warn">Nenhum registro disponível para seleção.</small>
                ) : null}
                {field.help ? <small className="field-help">{field.help}</small> : null}
              </label>
            )
          })}
        </div>
      </div>

      <footer className="drawer-footer">
        <button type="button" className="btn" onClick={onCancel} disabled={saving}>
          Cancelar
        </button>
        <button type="submit" className="btn btn-primary" disabled={saving}>
          {saving ? 'Salvando…' : editingItem ? 'Salvar alterações' : 'Cadastrar'}
        </button>
      </footer>
    </form>
  )
}
