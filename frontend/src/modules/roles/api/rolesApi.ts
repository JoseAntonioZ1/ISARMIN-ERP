import { httpClient } from '@/shared/api/httpClient'

export const ACCIONES = ['Crear', 'Editar', 'Eliminar', 'Consultar', 'Anular', 'Ajustar'] as const
export type Accion = (typeof ACCIONES)[number]

export interface Permiso {
  modulo: string
  accion: Accion
}

export interface Rol {
  id: string
  nombre: string
  descripcion: string | null
  permisos: Permiso[]
}

export interface CrearRolRequest {
  nombre: string
  descripcion?: string
}

export interface EditarRolRequest {
  nombre: string
  descripcion?: string
}

export const rolesApi = {
  listarConPermisos: () => httpClient.get<Rol[]>('/roles/detalle'),
  crear: (datos: CrearRolRequest) => httpClient.post<Rol>('/roles', datos),
  editar: (id: string, datos: EditarRolRequest) => httpClient.put<Rol>(`/roles/${id}`, datos),
  asignarPermisos: (id: string, permisos: Permiso[]) =>
    httpClient.put<Rol>(`/roles/${id}/permisos`, { permisos }),
}
