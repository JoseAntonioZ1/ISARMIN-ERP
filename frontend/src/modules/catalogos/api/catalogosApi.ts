import { httpClient } from '@/shared/api/httpClient'

export interface Categoria {
  id: string
  nombre: string
  categoriaPadreId: string | null
}

export interface MedioPago {
  id: string
  nombre: string
  activo: boolean
}

export const categoriasApi = {
  listar: () => httpClient.get<Categoria[]>('/categorias'),
  crear: (datos: { nombre: string; categoriaPadreId: string | null }) =>
    httpClient.post<Categoria>('/categorias', datos),
  editar: (id: string, datos: { nombre: string; categoriaPadreId: string | null }) =>
    httpClient.put<Categoria>(`/categorias/${id}`, datos),
}

export const mediosPagoApi = {
  listar: () => httpClient.get<MedioPago[]>('/configuracion/medios-pago'),
  crear: (nombre: string) => httpClient.post<MedioPago>('/configuracion/medios-pago', { nombre }),
  cambiarEstado: (id: string, activo: boolean) =>
    httpClient.patch<void>(`/configuracion/medios-pago/${id}/estado`, { activo }),
}
