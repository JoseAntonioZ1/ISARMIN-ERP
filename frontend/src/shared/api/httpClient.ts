import { useSessionStore } from '@/shared/hooks/useSessionStore'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5080/api/v1'

interface ErrorApi {
  codigo: string
  mensaje: string
  detalles: unknown
}

export class ApiError extends Error {
  readonly status: number
  readonly codigo: string | undefined
  readonly detalles: unknown

  constructor(status: number, error: ErrorApi | undefined) {
    super(error?.mensaje ?? 'Ocurrió un error inesperado al comunicarse con el servidor.')
    this.name = 'ApiError'
    this.status = status
    this.codigo = error?.codigo
    this.detalles = error?.detalles
  }
}

async function request<TRespuesta>(path: string, init?: RequestInit): Promise<TRespuesta> {
  const token = useSessionStore.getState().token

  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...init,
    headers: {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...init?.headers,
    },
  })

  if (!response.ok) {
    const cuerpo = await response.json().catch(() => undefined)
    throw new ApiError(response.status, cuerpo?.error)
  }

  if (response.status === 204) {
    return undefined as TRespuesta
  }

  return (await response.json()) as TRespuesta
}

export const httpClient = {
  get: <TRespuesta>(path: string) => request<TRespuesta>(path, { method: 'GET' }),
  post: <TRespuesta>(path: string, body?: unknown) =>
    request<TRespuesta>(path, { method: 'POST', body: body ? JSON.stringify(body) : undefined }),
  put: <TRespuesta>(path: string, body: unknown) =>
    request<TRespuesta>(path, { method: 'PUT', body: JSON.stringify(body) }),
  patch: <TRespuesta>(path: string, body?: unknown) =>
    request<TRespuesta>(path, { method: 'PATCH', body: body ? JSON.stringify(body) : undefined }),
  delete: <TRespuesta>(path: string) => request<TRespuesta>(path, { method: 'DELETE' }),
}
