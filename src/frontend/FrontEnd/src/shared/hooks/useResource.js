import { useCallback, useEffect, useMemo, useState } from 'react'
import { createResourceRepository } from '../../core/resourceRepository'

export const useResource = (definition) => {
  const repository = useMemo(() => createResourceRepository(definition), [definition])
  const [items, setItems] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [message, setMessage] = useState('')

  const loadItems = useCallback(async () => {
    setLoading(true)
    setError('')

    try {
      const data = await repository.list()
      setItems(Array.isArray(data) ? data : [])
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }, [repository])

  useEffect(() => {
    loadItems()
  }, [loadItems])

  const saveItem = async ({ id, payload }) => {
    setError('')
    setMessage('')

    try {
      if (id) {
        await repository.update(id, payload)
        setMessage('Registro atualizado com sucesso.')
      } else {
        await repository.create(payload)
        setMessage('Registro criado com sucesso.')
      }

      await loadItems()
      return true
    } catch (err) {
      setError(err.message)
      return false
    }
  }

  const deleteItem = async (id) => {
    setError('')
    setMessage('')

    try {
      await repository.remove(id)
      setMessage('Registro removido com sucesso.')
      await loadItems()
    } catch (err) {
      setError(err.message)
    }
  }

  return {
    items,
    loading,
    error,
    message,
    reload: loadItems,
    saveItem,
    deleteItem,
  }
}
