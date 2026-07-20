import { httpClient } from '@/shared/api/httpClient'

export const ESTADOS_OT = [
  'Recibido',
  'Diagnosticado',
  'Cotizado',
  'Aprobado',
  'Rechazado',
  'EnReparacion',
  'EnPruebas',
  'ListoParaEntrega',
  'Entregado',
] as const
export type EstadoOt = (typeof ESTADOS_OT)[number]

export const ESTADOS_PAGO = ['CompletoAntes', 'CompletoAlMomento', 'Adelanto', 'SaldoPendiente'] as const
export type EstadoPago = (typeof ESTADOS_PAGO)[number]

export interface Diagnostico {
  id: string
  descripcion: string
  usuarioId: string
  fecha: string
}

export interface CotizacionReparacion {
  id: string
  montoEstimado: number
  fecha: string
  decisionCliente: 'Aprobada' | 'Rechazada' | null
  cobroDiagnosticoRechazo: number | null
  evidenciaAprobacion: string | null
}

export interface ConsumoRepuesto {
  id: string
  productoId: string
  cantidad: number
}

export interface OrdenTrabajo {
  id: string
  clienteId: string
  equipoDescripcion: string
  fallaReportada: string
  fechaRecepcion: string
  usuarioRecepcionId: string
  estado: EstadoOt
  fechaEntrega: string | null
  usuarioEntregaId: string | null
  estadoPago: EstadoPago | null
  montoPagado: number | null
  saldoPendiente: number | null
  usuarioAutorizoSaldoId: string | null
  resultadoPruebas: string | null
  diagnostico: Diagnostico | null
  cotizacionReparacion: CotizacionReparacion | null
  consumosRepuesto: ConsumoRepuesto[]
}

export interface Garantia {
  id: string
  ordenTrabajoId: string
  fechaInicio: string
  fechaFin: string
  ordenTrabajoReingresoId: string | null
}

export interface OrdenTrabajoDetalle {
  ordenTrabajo: OrdenTrabajo
  garantia: Garantia | null
}

export interface ListadoPaginado<T> {
  datos: T[]
  total: number
  pagina: number
  tamanoPagina: number
}

export interface DetalleConsumoInput {
  productoId: string
  cantidad: number
}

export const ordenesTrabajoApi = {
  buscar: (estado?: string, clienteId?: string, pagina = 1, tamanoPagina = 20) => {
    const params = new URLSearchParams({ pagina: String(pagina), tamanoPagina: String(tamanoPagina) })
    if (estado) params.set('estado', estado)
    if (clienteId) params.set('cliente', clienteId)
    return httpClient.get<ListadoPaginado<OrdenTrabajo>>(`/ordenes-trabajo?${params.toString()}`)
  },
  obtener: (id: string) => httpClient.get<OrdenTrabajoDetalle>(`/ordenes-trabajo/${id}`),
  registrarRecepcion: (clienteId: string, equipoDescripcion: string, fallaReportada: string) =>
    httpClient.post<OrdenTrabajo>('/ordenes-trabajo', { clienteId, equipoDescripcion, fallaReportada }),
  registrarDiagnostico: (id: string, descripcion: string) =>
    httpClient.post<OrdenTrabajo>(`/ordenes-trabajo/${id}/diagnostico`, { descripcion }),
  generarCotizacion: (id: string, montoEstimado: number) =>
    httpClient.post<OrdenTrabajo>(`/ordenes-trabajo/${id}/cotizacion`, { montoEstimado }),
  registrarDecision: (id: string, decision: 'Aprobada' | 'Rechazada', cobroDiagnosticoRechazo: number | null, evidenciaAprobacion: string | null) =>
    httpClient.post<OrdenTrabajo>(`/ordenes-trabajo/${id}/decision`, { decision, cobroDiagnosticoRechazo, evidenciaAprobacion }),
  registrarReparacion: (id: string, consumos: DetalleConsumoInput[], resultadoPruebas: string | null) =>
    httpClient.post<OrdenTrabajo>(`/ordenes-trabajo/${id}/reparacion`, { consumos, resultadoPruebas }),
  entregarEquipo: (
    id: string,
    estadoPago: EstadoPago,
    montoPagado: number,
    saldoPendiente: number | null,
    usuarioAutorizoSaldoId: string | null,
  ) => httpClient.post<OrdenTrabajo>(`/ordenes-trabajo/${id}/entrega`, { estadoPago, montoPagado, saldoPendiente, usuarioAutorizoSaldoId }),
  registrarGarantia: (id: string, fechaInicio: string, fechaFin: string) =>
    httpClient.post<Garantia>(`/ordenes-trabajo/${id}/garantia`, { fechaInicio, fechaFin }),
}
