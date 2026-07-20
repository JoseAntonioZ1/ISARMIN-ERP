import { httpClient } from '@/shared/api/httpClient'

export const ESTADOS_SERVICIO_CAMPO = ['Solicitado', 'Agendado', 'EnEjecucion', 'Cerrado'] as const
export type EstadoServicioCampo = (typeof ESTADOS_SERVICIO_CAMPO)[number]

export interface DetalleConsumoCampoInput {
  productoId: string
  cantidad: number
}

export interface ServicioCampoDetalle {
  id: string
  productoId: string
  cantidad: number
}

export interface ServicioCampo {
  id: string
  clienteId: string
  descripcionTrabajo: string
  fechaSolicitud: string
  tecnicoAsignadoId: string | null
  estado: EstadoServicioCampo
  montoEstimado: number | null
  fechaEjecucion: string | null
  estadoFinal: string | null
  observaciones: string | null
  usuarioCierreId: string | null
  medioPagoId: string | null
  montoPagado: number | null
  saldoPendiente: number | null
  usuarioAutorizoSaldoId: string | null
  detalles: ServicioCampoDetalle[]
}

export interface ListadoPaginado<T> {
  datos: T[]
  total: number
  pagina: number
  tamanoPagina: number
}

export const serviciosCampoApi = {
  buscar: (estado?: string, clienteId?: string, pagina = 1, tamanoPagina = 20) => {
    const params = new URLSearchParams({ pagina: String(pagina), tamanoPagina: String(tamanoPagina) })
    if (estado) params.set('estado', estado)
    if (clienteId) params.set('cliente', clienteId)
    return httpClient.get<ListadoPaginado<ServicioCampo>>(`/servicios-campo?${params.toString()}`)
  },
  obtener: (id: string) => httpClient.get<ServicioCampo>(`/servicios-campo/${id}`),
  solicitar: (clienteId: string, descripcionTrabajo: string, tecnicoAsignadoId: string | null) =>
    httpClient.post<ServicioCampo>('/servicios-campo', { clienteId, descripcionTrabajo, tecnicoAsignadoId }),
  cotizar: (id: string, montoEstimado: number) =>
    httpClient.post<ServicioCampo>(`/servicios-campo/${id}/cotizacion`, { montoEstimado }),
  cerrar: (id: string, consumos: DetalleConsumoCampoInput[], estadoFinal: string, observaciones: string | null) =>
    httpClient.post<ServicioCampo>(`/servicios-campo/${id}/cierre`, { consumos, estadoFinal, observaciones }),
  cobrar: (
    id: string,
    medioPagoId: string,
    montoPagado: number,
    saldoPendiente: number | null,
    usuarioAutorizoSaldoId: string | null,
  ) => httpClient.post<ServicioCampo>(`/servicios-campo/${id}/cobro`, { medioPagoId, montoPagado, saldoPendiente, usuarioAutorizoSaldoId }),
}
