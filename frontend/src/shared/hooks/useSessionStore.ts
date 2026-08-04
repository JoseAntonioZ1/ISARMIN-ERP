import { create } from 'zustand'
import { persist } from 'zustand/middleware'

interface UsuarioSesion {
  id: string
  nombre: string
  permisos: string[]
}

interface SessionState {
  usuario: UsuarioSesion | null
  token: string | null
  refreshToken: string | null
  establecerSesion: (usuario: UsuarioSesion, token: string, refreshToken: string) => void
  cerrarSesion: () => void
  tienePermiso: (permiso: string) => boolean
}

export const useSessionStore = create<SessionState>()(
  persist(
    (set, get) => ({
      usuario: null,
      token: null,
      refreshToken: null,
      establecerSesion: (usuario, token, refreshToken) => set({ usuario, token, refreshToken }),
      cerrarSesion: () => set({ usuario: null, token: null, refreshToken: null }),
      tienePermiso: (permiso) => get().usuario?.permisos.includes(permiso) ?? false,
    }),
    { name: 'isarmin-sesion' },
  ),
)
