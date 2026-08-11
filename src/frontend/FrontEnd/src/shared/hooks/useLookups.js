import { useEffect, useMemo, useState } from 'react'
import { createResourceRepository } from '../../core/resourceRepository'

const collectLookups = (definition) => {
  const fields = definition.fields ?? []
  const columns = definition.columns ?? []
  const names = new Set()

  for (const item of [...fields, ...columns]) {
    if (item.lookup) {
      names.add(item.lookup)
    }
  }

  return [...names]
}

const getOptionLabel = (item, definition) => {
  const labelKeys = definition.lookupLabelKeys ?? ['nome', 'descricao', definition.idKey]
  const label = labelKeys
    .map((key) => item[key])
    .filter((value) => value || value === 0)
    .join(' - ')

  return label || `Registro ${item[definition.idKey]}`
}

export const useLookups = (definition, resourcesByKey) => {
  const lookupKeys = useMemo(() => collectLookups(definition), [definition])
  const [state, setState] = useState({ loading: false, error: '', data: {} })

  useEffect(() => {
    let active = true

    const load = async () => {
      if (!lookupKeys.length) {
        setState({ loading: false, error: '', data: {} })
        return
      }

      setState((current) => ({ ...current, loading: true, error: '' }))

      try {
        const entries = await Promise.all(
          lookupKeys.map(async (key) => {
            const lookupDefinition = resourcesByKey[key]

            if (!lookupDefinition) {
              return [key, { options: [], labels: new Map() }]
            }

            const items = await createResourceRepository(lookupDefinition).list()
            const options = items.map((item) => ({
              value: item[lookupDefinition.idKey],
              label: getOptionLabel(item, lookupDefinition),
            }))
            const labels = new Map(options.map((option) => [String(option.value), option.label]))

            return [key, { options, labels }]
          }),
        )

        if (active) {
          setState({ loading: false, error: '', data: Object.fromEntries(entries) })
        }
      } catch (error) {
        if (active) {
          setState({
            loading: false,
            error: error.message ?? 'Nao foi possivel carregar as opcoes relacionadas.',
            data: {},
          })
        }
      }
    }

    load()

    return () => {
      active = false
    }
  }, [lookupKeys, resourcesByKey])

  return state
}
