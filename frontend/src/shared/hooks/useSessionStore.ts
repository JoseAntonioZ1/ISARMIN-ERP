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
  establecerSesion: (usuario: UsuarioSesion, token: string) => void
  cerrarSesion: () => void
  tienePermiso: (permiso: string) => boolean
}

export const useSessionStore = create<SessionState>()(
  persist(
    (set, get) => ({
      usuario: null,
      token: null,
      establecerSesion: (usuario, token) => set({ usuario, token }),
      cerrarSesion: () => set({ usuario: null, token: null }),
      tienePermiso: (permiso) => get().usuario?.permisos.includes(permiso) ?? false,
    }),
    { name: 'isarmin-sesion' },
  ),
)
