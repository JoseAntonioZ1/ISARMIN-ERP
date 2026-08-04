import { beforeEach, describe, expect, it } from 'vitest'
import { useSessionStore } from './useSessionStore'

describe('useSessionStore', () => {
  beforeEach(() => {
    useSessionStore.getState().cerrarSesion()
  })

  it('no tiene sesión ni permisos por defecto', () => {
    const estado = useSessionStore.getState()
    expect(estado.usuario).toBeNull()
    expect(estado.token).toBeNull()
    expect(estado.tienePermiso('Ventas.Crear')).toBe(false)
  })

  it('establecerSesion guarda usuario, token y refreshToken', () => {
    useSessionStore
      .getState()
      .establecerSesion({ id: '1', nombre: 'Ana', permisos: ['Ventas.Crear'] }, 'jwt-token', 'refresh-token')

    const estado = useSessionStore.getState()
    expect(estado.usuario?.nombre).toBe('Ana')
    expect(estado.token).toBe('jwt-token')
    expect(estado.refreshToken).toBe('refresh-token')
    expect(estado.tienePermiso('Ventas.Crear')).toBe(true)
    expect(estado.tienePermiso('Ventas.Anular')).toBe(false)
  })

  it('cerrarSesion limpia todo el estado', () => {
    useSessionStore
      .getState()
      .establecerSesion({ id: '1', nombre: 'Ana', permisos: ['Ventas.Crear'] }, 'jwt-token', 'refresh-token')

    useSessionStore.getState().cerrarSesion()

    const estado = useSessionStore.getState()
    expect(estado.usuario).toBeNull()
    expect(estado.token).toBeNull()
    expect(estado.refreshToken).toBeNull()
  })
})
