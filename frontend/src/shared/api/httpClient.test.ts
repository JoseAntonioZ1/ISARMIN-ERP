import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'
import { useSessionStore } from '@/shared/hooks/useSessionStore'
import { httpClient } from './httpClient'

function mockJsonResponse(status: number, body: unknown): Response {
  return {
    ok: status >= 200 && status < 300,
    status,
    json: async () => body,
  } as Response
}

describe('httpClient - renovación automática de sesión ante un 401', () => {
  beforeEach(() => {
    useSessionStore
      .getState()
      .establecerSesion({ id: '1', nombre: 'Ana', permisos: [] }, 'token-viejo', 'refresh-viejo')
  })

  afterEach(() => {
    vi.unstubAllGlobals()
    useSessionStore.getState().cerrarSesion()
  })

  it('renueva la sesión con el refresh token y reintenta la petición original', async () => {
    const fetchMock = vi
      .fn()
      .mockResolvedValueOnce(mockJsonResponse(401, { error: { codigo: 'NO_AUTORIZADO', mensaje: 'Token expirado' } }))
      .mockResolvedValueOnce(
        mockJsonResponse(200, {
          token: 'token-nuevo',
          refreshToken: 'refresh-nuevo',
          usuario: { id: '1', nombre: 'Ana' },
          permisos: ['Ventas.Crear'],
        }),
      )
      .mockResolvedValueOnce(mockJsonResponse(200, { datos: 'ok' }))

    vi.stubGlobal('fetch', fetchMock)

    const resultado = await httpClient.get<{ datos: string }>('/ventas')

    expect(resultado).toEqual({ datos: 'ok' })
    expect(fetchMock).toHaveBeenCalledTimes(3)
    expect(useSessionStore.getState().token).toBe('token-nuevo')
    expect(useSessionStore.getState().refreshToken).toBe('refresh-nuevo')
  })

  it('si el refresh también falla, propaga el error y cierra la sesión', async () => {
    const fetchMock = vi
      .fn()
      .mockResolvedValueOnce(mockJsonResponse(401, { error: { codigo: 'NO_AUTORIZADO', mensaje: 'Token expirado' } }))
      .mockResolvedValueOnce(
        mockJsonResponse(401, { error: { codigo: 'REFRESH_TOKEN_INVALIDO', mensaje: 'Sesión expirada' } }),
      )

    vi.stubGlobal('fetch', fetchMock)

    await expect(httpClient.get('/ventas')).rejects.toThrow()

    expect(fetchMock).toHaveBeenCalledTimes(2)
    expect(useSessionStore.getState().token).toBeNull()
  })
})
