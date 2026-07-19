const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5080/api'

export class ApiError extends Error {
  readonly status: number

  constructor(message: string, status: number) {
    super(message)
    this.name = 'ApiError'
    this.status = status
  }
}

async function request<TRespuesta>(path: string, init?: RequestInit): Promise<TRespuesta> {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...init,
    headers: {
      'Content-Type': 'application/json',
      ...init?.headers,
    },
  })

  if (!response.ok) {
    throw new ApiError(`Error en la solicitud a ${path}`, response.status)
  }

  if (response.status === 204) {
    return undefined as TRespuesta
  }

  return (await response.json()) as TRespuesta
}

export const httpClient = {
  get: <TRespuesta>(path: string) => request<TRespuesta>(path, { method: 'GET' }),
  post: <TRespuesta>(path: string, body: unknown) =>
    request<TRespuesta>(path, { method: 'POST', body: JSON.stringify(body) }),
  put: <TRespuesta>(path: string, body: unknown) =>
    request<TRespuesta>(path, { method: 'PUT', body: JSON.stringify(body) }),
  delete: <TRespuesta>(path: string) => request<TRespuesta>(path, { method: 'DELETE' }),
}
