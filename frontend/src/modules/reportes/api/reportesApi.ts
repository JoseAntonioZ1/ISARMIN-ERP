import type { Caja, MovimientoCaja } from '@/modules/caja/api/cajaApi'
import type { Producto } from '@/modules/productos/api/productosApi'
import type { ServicioCampo } from '@/modules/serviciosCampo/api/serviciosCampoApi'
import type { EstadoOt, OrdenTrabajo } from '@/modules/taller/api/ordenesTrabajoApi'
import type { Venta } from '@/modules/ventas/api/ventasApi'
import { httpClient } from '@/shared/api/httpClient'

export interface ReporteVentas {
  desde: string | null
  hasta: string | null
  cantidadVentas: number
  montoTotal: number
  ventas: Venta[]
}

export interface ReporteInventario {
  productos: Producto[]
  productosEnQuiebre: Producto[]
}

export interface ReporteOrdenesTrabajo {
  desde: string | null
  hasta: string | null
  cantidadTotal: number
  ordenes: OrdenTrabajo[]
}

export interface ReporteServiciosCampo {
  desde: string | null
  hasta: string | null
  cantidadTotal: number
  servicios: ServicioCampo[]
}

export interface ReporteCaja {
  desde: string | null
  hasta: string | null
  totalIngresos: number
  totalEgresos: number
  saldoNeto: number
  cajas: Caja[]
  movimientos: MovimientoCaja[]
}

const construirParams = (params: Record<string, string | undefined>) => {
  const query = new URLSearchParams()
  for (const [clave, valor] of Object.entries(params)) {
    if (valor) query.set(clave, valor)
  }
  const texto = query.toString()
  return texto ? `?${texto}` : ''
}

export const reportesApi = {
  ventas: (desde?: string, hasta?: string) =>
    httpClient.get<ReporteVentas>(`/reportes/ventas${construirParams({ desde, hasta })}`),
  inventario: () => httpClient.get<ReporteInventario>('/reportes/inventario'),
  ordenesTrabajo: (estado?: EstadoOt, desde?: string, hasta?: string) =>
    httpClient.get<ReporteOrdenesTrabajo>(`/reportes/ordenes-trabajo${construirParams({ estado, desde, hasta })}`),
  serviciosCampo: (tecnico?: string, desde?: string, hasta?: string) =>
    httpClient.get<ReporteServiciosCampo>(`/reportes/servicios-campo${construirParams({ tecnico, desde, hasta })}`),
  caja: (desde?: string, hasta?: string) => httpClient.get<ReporteCaja>(`/reportes/caja${construirParams({ desde, hasta })}`),
}
