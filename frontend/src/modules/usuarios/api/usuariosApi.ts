import { httpClient } from '@/shared/api/httpClient'

export interface RolResumen {
  id: string
  nombre: string
}

export interface Usuario {
  id: string
  nombre: string
  nombreUsuario: string
  estado: 'Activo' | 'Inactivo'
  roles: RolResumen[]
}

export interface ListadoPaginado<T> {
  datos: T[]
  total: number
  pagina: number
  tamanoPagina: number
}

export interface CrearUsuarioRequest {
  nombre: string
  nombreUsuario: string
  credencialInicial: string
  rolIds: string[]
}

export interface EditarUsuarioRequest {
  nombre: string
  rolIds: string[]
}

export const usuariosApi = {
  listar: (pagina = 1, tamanoPagina = 20) =>
    httpClient.get<ListadoPaginado<Usuario>>(`/usuarios?pagina=${pagina}&tamanoPagina=${tamanoPagina}`),
  crear: (datos: CrearUsuarioRequest) => httpClient.post<Usuario>('/usuarios', datos),
  editar: (id: string, datos: EditarUsuarioRequest) => httpClient.put<Usuario>(`/usuarios/${id}`, datos),
  cambiarEstado: (id: string, activo: boolean) => httpClient.patch<void>(`/usuarios/${id}/estado`, { activo }),
  restablecerCredencial: (id: string, nuevaCredencial: string) =>
    httpClient.patch<void>(`/usuarios/${id}/restablecer-credencial`, { nuevaCredencial }),
}

export const rolesApi = {
  listar: () => httpClient.get<RolResumen[]>('/roles'),
}
