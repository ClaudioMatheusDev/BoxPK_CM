import { httpClient } from './httpClient'

export const createResourceRepository = (definition) => ({
  list: () => httpClient.get(definition.endpoint),
  create: (payload) => httpClient.post(definition.endpoint, payload),
  update: (id, payload) => httpClient.put(`${definition.endpoint}/${id}`, payload),
  remove: (id) => httpClient.delete(`${definition.endpoint}/${id}`),
})
