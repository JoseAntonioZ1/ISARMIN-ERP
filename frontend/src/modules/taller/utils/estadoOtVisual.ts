import {
  CheckCheck,
  CircleCheck,
  FileText,
  Inbox,
  type LucideIcon,
  PackageCheck,
  Stethoscope,
  XCircle,
} from 'lucide-react'
import type { EstadoOt } from '@/modules/taller/api/ordenesTrabajoApi'

interface EstadoVisual {
  etiqueta: string
  icono: LucideIcon
  /** Clases del badge/borde (texto + fondo tenue) — mismo tono se usa para la tarjeta del Kanban. */
  clase: string
}

/** Mapeo único estado→apariencia, reutilizado en el tablero Kanban y en el badge del diálogo de
 * detalle. Solo cubre los 7 estados realmente alcanzables — EnReparacion/EnPruebas nunca se
 * persisten (RegistrarReparacionCommand pasa directo de Aprobado a ListoParaEntrega). */
export const ESTADOS_OT_VISUAL: Record<EstadoOt, EstadoVisual> = {
  Recibido: { etiqueta: 'Recibido', icono: Inbox, clase: 'text-blue-600 border-blue-200 bg-blue-50 dark:bg-blue-950/40 dark:border-blue-900' },
  Diagnosticado: {
    etiqueta: 'Diagnosticado',
    icono: Stethoscope,
    clase: 'text-indigo-600 border-indigo-200 bg-indigo-50 dark:bg-indigo-950/40 dark:border-indigo-900',
  },
  Cotizado: {
    etiqueta: 'Cotizado',
    icono: FileText,
    clase: 'text-amber-600 border-amber-200 bg-amber-50 dark:bg-amber-950/40 dark:border-amber-900',
  },
  Aprobado: {
    etiqueta: 'Aprobado',
    icono: CircleCheck,
    clase: 'text-emerald-600 border-emerald-200 bg-emerald-50 dark:bg-emerald-950/40 dark:border-emerald-900',
  },
  Rechazado: { etiqueta: 'Rechazado', icono: XCircle, clase: 'text-red-600 border-red-200 bg-red-50 dark:bg-red-950/40 dark:border-red-900' },
  EnReparacion: { etiqueta: 'En reparación', icono: Stethoscope, clase: 'text-indigo-600 border-indigo-200 bg-indigo-50' },
  EnPruebas: { etiqueta: 'En pruebas', icono: Stethoscope, clase: 'text-indigo-600 border-indigo-200 bg-indigo-50' },
  ListoParaEntrega: {
    etiqueta: 'Listo para Entrega',
    icono: PackageCheck,
    clase: 'text-violet-600 border-violet-200 bg-violet-50 dark:bg-violet-950/40 dark:border-violet-900',
  },
  Entregado: {
    etiqueta: 'Entregado',
    icono: CheckCheck,
    clase: 'text-[var(--color-terciario)] border-slate-200 bg-slate-50 dark:bg-slate-900 dark:border-slate-700',
  },
}

/** Columnas del tablero Kanban, en orden de flujo. Rechazado se muestra aparte por ser un estado
 * terminal (no sigue el flujo lineal de los demás). */
export const COLUMNAS_KANBAN_OT: EstadoOt[] = [
  'Recibido',
  'Diagnosticado',
  'Cotizado',
  'Aprobado',
  'ListoParaEntrega',
  'Entregado',
]
