import { httpClient } from '@/shared/api/httpClient'

export const ESTADOS_VENTA = ['Registrada', 'Emitida', 'Pagada', 'Anulada'] as const
export type EstadoVenta = (typeof ESTADOS_VENTA)[number]

export const TIPOS_COMPROBANTE = ['Cotizacion', 'Boleta', 'Factura', 'NotaVenta', 'Ticket'] as const
export type TipoComprobante = (typeof TIPOS_COMPROBANTE)[number]

export interface DetalleVentaInput {
  productoId: string
  cantidad: number
  precioUnitario: number
}

export interface PagoVentaInput {
  medioPagoId: string
  monto: number
}

export interface VentaDetalle {
  id: string
  productoId: string
  cantidad: number
  precioUnitario: number
}

export interface PagoVenta {
  id: string
  medioPagoId: string
  monto: number
}

export interface Venta {
  id: string
  clienteId: string | null
  tipoComprobante: TipoComprobante
  origen: string
  fecha: string
  usuarioId: string
  total: number
  estado: EstadoVenta
  saldoPendiente: number | null
  usuarioAutorizoSaldoId: string | null
  motivoAnulacion: string | null
  usuarioAnuloId: string | null
  fechaAnulacion: string | null
  detalles: VentaDetalle[]
  pagos: PagoVenta[]
}

export interface ListadoPaginado<T> {
  datos: T[]
  total: number
  pagina: number
  tamanoPagina: number
}

export interface DetalleDevolucionInput {
  productoId: string
  cantidad: number
}

export const ventasApi = {
  buscar: (estado?: string, clienteId?: string, pagina = 1, tamanoPagina = 20) => {
    const params = new URLSearchParams({ pagina: String(pagina), tamanoPagina: String(tamanoPagina) })
    if (estado) params.set('estado', estado)
    if (clienteId) params.set('cliente', clienteId)
    return httpClient.get<ListadoPaginado<Venta>>(`/ventas?${params.toString()}`)
  },
  obtener: (id: string) => httpClient.get<Venta>(`/ventas/${id}`),
  registrar: (
    clienteId: string | null,
    tipoComprobante: TipoComprobante,
    detalles: DetalleVentaInput[],
    pagos: PagoVentaInput[],
    usuarioAutorizoSaldoId: string | null,
  ) => httpClient.post<Venta>('/ventas', { clienteId, tipoComprobante, detalles, pagos, usuarioAutorizoSaldoId }),
  anular: (id: string, motivo: string) => httpClient.post<Venta>(`/ventas/${id}/anular`, { motivo }),
  registrarDevolucion: (id: string, detalles: DetalleDevolucionInput[], motivo: string | null) =>
    httpClient.post<Venta>(`/ventas/${id}/devoluciones`, { detalles, motivo }),
}
