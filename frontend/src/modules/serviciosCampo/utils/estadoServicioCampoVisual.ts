import { CircleDot, Clock, type LucideIcon } from 'lucide-react'
import type { EstadoServicioCampo } from '@/modules/serviciosCampo/api/serviciosCampoApi'

interface EstadoVisual {
  etiqueta: string
  icono: LucideIcon
  clase: string
  /** Color sólido para el gráfico de barras de Reportes → Servicios de Campo. */
  claseBarra: string
}

/** Mapeo estado→apariencia. Solo Solicitado y Cerrado son alcanzables en la práctica — el propio
 * `ServicioCampo.cs` deja explícito que `Cerrar()` exige `Estado == Solicitado` y salta directo a
 * `Cerrado` ("más simple que Taller: no tiene una máquina de estados con aprobación formal").
 * Agendado/EnEjecucion existen en el enum pero ningún comando los persiste — mismo caso que
 * EnReparacion/EnPruebas en Taller. Se mantienen aquí solo para que el Record esté completo. */
export const ESTADOS_SERVICIO_CAMPO_VISUAL: Record<EstadoServicioCampo, EstadoVisual> = {
  Solicitado: {
    etiqueta: 'Solicitado',
    icono: Clock,
    // Amarillo de marca — "pendiente de atención", con el mismo tono que ya se usa para lo mismo en otras vistas.
    clase: 'text-[var(--color-secundario)] border-[var(--color-secundario)]/30 bg-[var(--color-secundario)]/10',
    claseBarra: 'bg-[var(--color-secundario)]',
  },
  Agendado: { etiqueta: 'Agendado', icono: CircleDot, clase: 'text-blue-600 border-blue-200 bg-blue-50', claseBarra: 'bg-blue-500' },
  EnEjecucion: {
    etiqueta: 'En ejecución',
    icono: CircleDot,
    clase: 'text-blue-600 border-blue-200 bg-blue-50',
    claseBarra: 'bg-blue-500',
  },
  Cerrado: {
    etiqueta: 'Cerrado',
    icono: CircleDot,
    clase: 'text-[var(--color-terciario)] border-slate-200 bg-slate-50 dark:bg-slate-900 dark:border-slate-700',
    claseBarra: 'bg-slate-400',
  },
}

/** Únicos valores que un filtro de estado debería ofrecer — Agendado/EnEjecucion siempre estarían
 * vacíos, mostrarlos confundiría más de lo que ayuda. */
export const ESTADOS_SERVICIO_CAMPO_FILTRABLES: EstadoServicioCampo[] = ['Solicitado', 'Cerrado']
