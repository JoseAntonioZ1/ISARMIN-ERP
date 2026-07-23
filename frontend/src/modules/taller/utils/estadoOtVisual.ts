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
  /** Color sólido (fondo lleno) para usar en gráficos de barras — el tono tenue de `clase` casi no se
   * ve como relleno de barra. */
  claseBarra: string
}

/** Mapeo único estado→apariencia, reutilizado en el tablero Kanban, el badge del diálogo de detalle,
 * y el gráfico de Reportes → Órdenes de Trabajo. Solo cubre los 7 estados realmente alcanzables —
 * EnReparacion/EnPruebas nunca se persisten (RegistrarReparacionCommand pasa directo de Aprobado a
 * ListoParaEntrega). */
export const ESTADOS_OT_VISUAL: Record<EstadoOt, EstadoVisual> = {
  Recibido: {
    etiqueta: 'Recibido',
    icono: Inbox,
    clase: 'text-blue-600 border-blue-200 bg-blue-50 dark:bg-blue-950/40 dark:border-blue-900',
    claseBarra: 'bg-blue-500',
  },
  Diagnosticado: {
    etiqueta: 'Diagnosticado',
    icono: Stethoscope,
    clase: 'text-indigo-600 border-indigo-200 bg-indigo-50 dark:bg-indigo-950/40 dark:border-indigo-900',
    claseBarra: 'bg-indigo-500',
  },
  Cotizado: {
    etiqueta: 'Cotizado',
    icono: FileText,
    // Mismo tono que --color-secundario (amarillo de marca) — se usa la variable directamente en vez
    // de amber-* de Tailwind para que se note que es el amarillo de ISARMIN, no un ámbar genérico.
    clase: 'text-[var(--color-secundario)] border-[var(--color-secundario)]/30 bg-[var(--color-secundario)]/10',
    claseBarra: 'bg-[var(--color-secundario)]',
  },
  Aprobado: {
    etiqueta: 'Aprobado',
    icono: CircleCheck,
    clase: 'text-emerald-600 border-emerald-200 bg-emerald-50 dark:bg-emerald-950/40 dark:border-emerald-900',
    claseBarra: 'bg-emerald-500',
  },
  Rechazado: {
    etiqueta: 'Rechazado',
    icono: XCircle,
    // Mismo tono que --color-principal (rojo de marca) — coincide además con la convención universal
    // de "rechazado/detenido" en rojo.
    clase: 'text-[var(--color-principal)] border-[var(--color-principal)]/30 bg-[var(--color-principal)]/10',
    claseBarra: 'bg-[var(--color-principal)]',
  },
  EnReparacion: {
    etiqueta: 'En reparación',
    icono: Stethoscope,
    clase: 'text-indigo-600 border-indigo-200 bg-indigo-50',
    claseBarra: 'bg-indigo-500',
  },
  EnPruebas: {
    etiqueta: 'En pruebas',
    icono: Stethoscope,
    clase: 'text-indigo-600 border-indigo-200 bg-indigo-50',
    claseBarra: 'bg-indigo-500',
  },
  ListoParaEntrega: {
    etiqueta: 'Listo para Entrega',
    icono: PackageCheck,
    clase: 'text-violet-600 border-violet-200 bg-violet-50 dark:bg-violet-950/40 dark:border-violet-900',
    claseBarra: 'bg-violet-500',
  },
  Entregado: {
    etiqueta: 'Entregado',
    icono: CheckCheck,
    clase: 'text-[var(--color-terciario)] border-slate-200 bg-slate-50 dark:bg-slate-900 dark:border-slate-700',
    claseBarra: 'bg-slate-400',
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
