import { useState } from 'react'
import { ReporteCajaTab } from '@/modules/reportes/components/ReporteCajaTab'
import { ReporteInventarioTab } from '@/modules/reportes/components/ReporteInventarioTab'
import { ReporteOrdenesTrabajoTab } from '@/modules/reportes/components/ReporteOrdenesTrabajoTab'
import { ReporteServiciosCampoTab } from '@/modules/reportes/components/ReporteServiciosCampoTab'
import { ReporteVentasTab } from '@/modules/reportes/components/ReporteVentasTab'
import { useBranding } from '@/shared/hooks/useBranding'

const PESTANAS = [
  { id: 'ventas', etiqueta: 'Ventas' },
  { id: 'inventario', etiqueta: 'Inventario' },
  { id: 'ordenes-trabajo', etiqueta: 'Órdenes de Trabajo' },
  { id: 'servicios-campo', etiqueta: 'Servicios de Campo' },
  { id: 'caja', etiqueta: 'Caja' },
] as const

type PestanaId = (typeof PESTANAS)[number]['id']

export function ReportesPage() {
  const [pestana, setPestana] = useState<PestanaId>('ventas')
  const { data: branding } = useBranding()

  return (
    <div>
      <div className="mb-4">
        <h1 className="text-xl font-semibold text-slate-800 dark:text-slate-100">Reportes</h1>
        {branding && (
          <p className="text-xs text-slate-500 dark:text-slate-400">
            {branding.razonSocial}
            {branding.ruc ? ` — RUC ${branding.ruc}` : ''}
          </p>
        )}
      </div>

      <div className="mb-4 flex gap-1 border-b border-slate-200 dark:border-slate-700">
        {PESTANAS.map((p) => (
          <button
            key={p.id}
            type="button"
            onClick={() => setPestana(p.id)}
            className={`px-4 py-2 text-sm ${
              pestana === p.id
                ? 'border-b-2 border-slate-800 font-semibold text-slate-800 dark:border-slate-100 dark:text-slate-100'
                : 'text-slate-500 hover:text-slate-800 dark:hover:text-slate-200'
            }`}
          >
            {p.etiqueta}
          </button>
        ))}
      </div>

      {pestana === 'ventas' && <ReporteVentasTab />}
      {pestana === 'inventario' && <ReporteInventarioTab />}
      {pestana === 'ordenes-trabajo' && <ReporteOrdenesTrabajoTab />}
      {pestana === 'servicios-campo' && <ReporteServiciosCampoTab />}
      {pestana === 'caja' && <ReporteCajaTab />}
    </div>
  )
}
