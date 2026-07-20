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

export interface UnidadMedida {
  id: string
  nombre: string
}

export interface ConfiguracionEmpresa {
  id: string
  razonSocial: string
  ruc: string | null
  direccion: string | null
  logo: string | null
  montoAperturaCajaPredeterminado: number | null
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

export const unidadesMedidaApi = {
  listar: () => httpClient.get<UnidadMedida[]>('/unidades-medida'),
  crear: (nombre: string) => httpClient.post<UnidadMedida>('/unidades-medida', { nombre }),
  editar: (id: string, nombre: string) => httpClient.put<UnidadMedida>(`/unidades-medida/${id}`, { nombre }),
}

export const configuracionEmpresaApi = {
  obtener: () => httpClient.get<ConfiguracionEmpresa>('/configuracion/empresa'),
  actualizar: (datos: {
    razonSocial: string
    ruc: string | null
    direccion: string | null
    logo: string | null
    montoAperturaCajaPredeterminado: number | null
  }) => httpClient.put<ConfiguracionEmpresa>('/configuracion/empresa', datos),
}
