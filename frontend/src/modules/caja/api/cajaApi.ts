import { httpClient } from '@/shared/api/httpClient'

export const CONCEPTOS_MOVIMIENTO = ['GastoOperativo', 'RetiroPropietario', 'AporteCapital'] as const
export type ConceptoMovimiento = (typeof CONCEPTOS_MOVIMIENTO)[number]

export interface Caja {
  id: string
  fechaApertura: string
  montoApertura: number
  fechaCierre: string | null
  montoTeoricoCierre: number | null
  montoFisicoDeclarado: number | null
  diferencia: number | null
  usuarioId: string
  estado: 'Abierta' | 'Cerrada'
}

export interface MovimientoCaja {
  id: string
  cajaId: string
  tipo: 'Ingreso' | 'Egreso'
  monto: number
  concepto: ConceptoMovimiento
  descripcion: string | null
  usuarioId: string
  fecha: string
}

export const cajaApi = {
  obtenerActual: () => httpClient.get<Caja | null>('/caja'),
  abrir: (montoApertura: number) => httpClient.post<Caja>('/caja/apertura', { montoApertura }),
  cerrar: (montoFisicoDeclarado: number) => httpClient.post<Caja>('/caja/cierre', { montoFisicoDeclarado }),
  listarMovimientos: (cajaId?: string) =>
    httpClient.get<MovimientoCaja[]>(`/caja/movimientos${cajaId ? `?cajaId=${cajaId}` : ''}`),
  registrarMovimiento: (concepto: ConceptoMovimiento, monto: number, descripcion: string | null) =>
    httpClient.post<MovimientoCaja>('/caja/movimientos', { concepto, monto, descripcion }),
}
