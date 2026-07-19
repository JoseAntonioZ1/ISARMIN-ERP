import { httpClient } from '@/shared/api/httpClient'

export interface Proveedor {
  id: string
  nombreRazonSocial: string
  documento: string | null
  telefono: string | null
  direccion: string | null
  estado: 'Activo' | 'Inactivo'
}

export interface ListadoPaginado<T> {
  datos: T[]
  total: number
  pagina: number
  tamanoPagina: number
}

export interface DatosProveedor {
  nombreRazonSocial: string
  documento: string | null
  telefono: string | null
  direccion: string | null
}

export const proveedoresApi = {
  buscar: (busqueda?: string, pagina = 1, tamanoPagina = 20) =>
    httpClient.get<ListadoPaginado<Proveedor>>(
      `/proveedores?busqueda=${encodeURIComponent(busqueda ?? '')}&pagina=${pagina}&tamanoPagina=${tamanoPagina}`,
    ),
  registrar: (datos: DatosProveedor) => httpClient.post<Proveedor>('/proveedores', datos),
  editar: (id: string, datos: DatosProveedor) => httpClient.put<Proveedor>(`/proveedores/${id}`, datos),
  cambiarEstado: (id: string, activo: boolean) => httpClient.patch<void>(`/proveedores/${id}/estado`, { activo }),
}
