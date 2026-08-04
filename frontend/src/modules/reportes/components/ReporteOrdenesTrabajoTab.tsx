import { useQuery } from '@tanstack/react-query'
import { useMemo, useState } from 'react'
import { reportesApi } from '@/modules/reportes/api/reportesApi'
import { BotonExportarCsv } from '@/shared/components/BotonExportarCsv'
import { EstadoCarga } from '@/shared/components/EstadoCarga'
import { GraficoBarras } from '@/shared/components/GraficoBarras'
import { type EstadoOt, ESTADOS_OT } from '@/modules/taller/api/ordenesTrabajoApi'
import { ESTADOS_OT_VISUAL } from '@/modules/taller/utils/estadoOtVisual'
import { exportarCsv } from '@/shared/utils/exportarCsv'

export function ReporteOrdenesTrabajoTab() {
  const [estado, setEstado] = useState<EstadoOt | ''>('')
  const [desde, setDesde] = useState('')
  const [hasta, setHasta] = useState('')

  const { data: reporte, isLoading } = useQuery({
    queryKey: ['reportes', 'ordenes-trabajo', estado, desde, hasta],
    queryFn: () => reportesApi.ordenesTrabajo(estado || undefined, desde || undefined, hasta || undefined),
  })

  const conteoPorEstado = useMemo(() => {
    const mapa = new Map<EstadoOt, number>()
    for (const ot of reporte?.ordenes ?? []) {
      mapa.set(ot.estado, (mapa.get(ot.estado) ?? 0) + 1)
    }
    return Array.from(mapa.entries())
      .map(([est, valor]) => ({ etiqueta: ESTADOS_OT_VISUAL[est].etiqueta, valor, claseColor: ESTADOS_OT_VISUAL[est].claseBarra }))
      .sort((a, b) => b.valor - a.valor)
  }, [reporte])

  const handleExportar = () => {
    if (!reporte) return
    exportarCsv(
      `reporte-ordenes-trabajo-${desde || 'inicio'}_${hasta || 'fin'}.csv`,
      ['Recepción', 'Equipo', 'Estado'],
      reporte.ordenes.map((ot) => [new Date(ot.fechaRecepcion).toLocaleDateString(), ot.equipoDescripcion, ot.estado]),
    )
  }

  return (
    <div>
      <div className="mb-4 flex items-end justify-between gap-3">
        <div className="flex gap-3">
          <div>
            <label className="mb-1 block text-xs text-[var(--color-terciario)]">Estado</label>
            <select
              value={estado}
              onChange={(e) => setEstado(e.target.value as EstadoOt | '')}
              className="rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
            >
              <option value="">Todos</option>
              {ESTADOS_OT.map((e) => (
                <option key={e} value={e}>
                  {e}
                </option>
              ))}
            </select>
          </div>
          <div>
            <label className="mb-1 block text-xs text-[var(--color-terciario)]">Desde</label>
            <input
              type="date"
              value={desde}
              onChange={(e) => setDesde(e.target.value)}
              className="rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
            />
          </div>
          <div>
            <label className="mb-1 block text-xs text-[var(--color-terciario)]">Hasta</label>
            <input
              type="date"
              value={hasta}
              onChange={(e) => setHasta(e.target.value)}
              className="rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
            />
          </div>
        </div>
        <BotonExportarCsv onClick={handleExportar} disabled={!reporte || reporte.ordenes.length === 0} />
      </div>

      {isLoading || !reporte ? (
        <EstadoCarga />
      ) : (
        <>
          <div className="mb-4 rounded-lg bg-white p-4 shadow-sm dark:bg-slate-800">
            <p className="text-xs text-[var(--color-terciario)]">Cantidad de órdenes</p>
            <p className="text-xl font-semibold text-slate-800 dark:text-slate-100">{reporte.cantidadTotal}</p>
          </div>

          <div className="mb-4 rounded-lg bg-white p-4 shadow-sm dark:bg-slate-800">
            <h3 className="mb-3 text-sm font-semibold text-[var(--color-apoyo)] dark:text-slate-300">Órdenes por estado</h3>
            <GraficoBarras datos={conteoPorEstado} />
          </div>

          <div className="overflow-x-auto">
          <table className="w-full border-collapse overflow-hidden rounded-lg bg-white text-left text-sm shadow-sm dark:bg-slate-800">
            <thead className="bg-slate-100 dark:bg-slate-700">
              <tr>
                <th className="px-4 py-2">Recepción</th>
                <th className="px-4 py-2">Equipo</th>
                <th className="px-4 py-2">Estado</th>
              </tr>
            </thead>
            <tbody>
              {reporte.ordenes.map((ot) => (
                <tr key={ot.id} className="border-t border-slate-200 dark:border-slate-700">
                  <td className="px-4 py-2">{new Date(ot.fechaRecepcion).toLocaleDateString()}</td>
                  <td className="px-4 py-2">{ot.equipoDescripcion}</td>
                  <td className="px-4 py-2">{ot.estado}</td>
                </tr>
              ))}
            </tbody>
          </table>
          </div>
        </>
      )}
    </div>
  )
}
