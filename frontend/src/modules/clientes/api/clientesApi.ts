import { httpClient } from '@/shared/api/httpClient'

export const TIPOS_DOCUMENTO = ['Dni', 'Ruc', 'CarneExtranjeria', 'Pasaporte'] as const
export type TipoDocumento = (typeof TIPOS_DOCUMENTO)[number]

export interface Cliente {
  id: string
  nombreRazonSocial: string
  telefono: string
  direccion: string | null
  tipoDocumento: TipoDocumento | null
  numeroDocumento: string | null
  tipoCliente: 'Natural' | 'Juridica' | null
  estado: 'Activo' | 'Inactivo'
}

export interface ListadoPaginado<T> {
  datos: T[]
  total: number
  pagina: number
  tamanoPagina: number
}

export interface DatosCliente {
  nombreRazonSocial: string
  telefono: string
  direccion: string | null
  tipoDocumento: string | null
  numeroDocumento: string | null
}

export const clientesApi = {
  buscar: (busqueda?: string, pagina = 1, tamanoPagina = 20) =>
    httpClient.get<ListadoPaginado<Cliente>>(
      `/clientes?busqueda=${encodeURIComponent(busqueda ?? '')}&pagina=${pagina}&tamanoPagina=${tamanoPagina}`,
    ),
  registrar: (datos: DatosCliente) => httpClient.post<Cliente>('/clientes', datos),
  editar: (id: string, datos: DatosCliente) => httpClient.put<Cliente>(`/clientes/${id}`, datos),
  cambiarEstado: (id: string, activo: boolean) => httpClient.patch<void>(`/clientes/${id}/estado`, { activo }),
}
