import { useQuery } from '@tanstack/react-query'
import { useMemo, useState } from 'react'
import { reportesApi } from '@/modules/reportes/api/reportesApi'
import { BotonExportarCsv } from '@/shared/components/BotonExportarCsv'
import { GraficoBarras } from '@/shared/components/GraficoBarras'
import { exportarCsv } from '@/shared/utils/exportarCsv'

export function ReporteVentasTab() {
  const [desde, setDesde] = useState('')
  const [hasta, setHasta] = useState('')

  const { data: reporte, isLoading } = useQuery({
    queryKey: ['reportes', 'ventas', desde, hasta],
    queryFn: () => reportesApi.ventas(desde || undefined, hasta || undefined),
  })

  // Excluye Anuladas del total por día, igual que el criterio ya usado por montoTotal del backend.
  // Se limita a los últimos 31 días con datos para que el gráfico no se vuelva ilegible con rangos muy amplios.
  const ventasPorDia = useMemo(() => {
    const mapa = new Map<string, number>()
    for (const venta of reporte?.ventas ?? []) {
      if (venta.estado === 'Anulada') continue
      const dia = new Date(venta.fecha).toLocaleDateString('es-PE')
      mapa.set(dia, (mapa.get(dia) ?? 0) + venta.total)
    }
    return Array.from(mapa.entries())
      .map(([etiqueta, valor]) => ({ etiqueta, valor }))
      .slice(-31)
  }, [reporte])

  const handleExportar = () => {
    if (!reporte) return
    exportarCsv(
      `reporte-ventas-${desde || 'inicio'}_${hasta || 'fin'}.csv`,
      ['Fecha', 'Comprobante', 'Total', 'Estado'],
      reporte.ventas.map((v) => [new Date(v.fecha).toLocaleDateString(), v.tipoComprobante, v.total.toFixed(2), v.estado]),
    )
  }

  return (
    <div>
      <div className="mb-4 flex items-end justify-between gap-3">
        <div className="flex gap-3">
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
        <BotonExportarCsv onClick={handleExportar} disabled={!reporte || reporte.ventas.length === 0} />
      </div>

      {isLoading || !reporte ? (
        <p className="text-[var(--color-terciario)]">Cargando...</p>
      ) : (
        <>
          <div className="mb-4 grid grid-cols-2 gap-4">
            <div className="rounded-lg bg-white p-4 shadow-sm dark:bg-slate-800">
              <p className="text-xs text-[var(--color-terciario)]">Cantidad de ventas</p>
              <p className="text-xl font-semibold text-slate-800 dark:text-slate-100">{reporte.cantidadVentas}</p>
            </div>
            <div className="rounded-lg bg-white p-4 shadow-sm dark:bg-slate-800">
              <p className="text-xs text-[var(--color-terciario)]">Monto total (excluye anuladas)</p>
              <p className="text-xl font-semibold text-slate-800 dark:text-slate-100">S/ {reporte.montoTotal.toFixed(2)}</p>
            </div>
          </div>

          <div className="mb-4 rounded-lg bg-white p-4 shadow-sm dark:bg-slate-800">
            <h3 className="mb-3 text-sm font-semibold text-[var(--color-apoyo)] dark:text-slate-300">Ventas por día</h3>
            <GraficoBarras datos={ventasPorDia} formatoValor={(v) => `S/ ${v.toFixed(2)}`} />
          </div>

          <table className="w-full border-collapse overflow-hidden rounded-lg bg-white text-left text-sm shadow-sm dark:bg-slate-800">
            <thead className="bg-slate-100 dark:bg-slate-700">
              <tr>
                <th className="px-4 py-2">Fecha</th>
                <th className="px-4 py-2">Comprobante</th>
                <th className="px-4 py-2">Total</th>
                <th className="px-4 py-2">Estado</th>
              </tr>
            </thead>
            <tbody>
              {reporte.ventas.map((venta) => (
                <tr key={venta.id} className="border-t border-slate-200 dark:border-slate-700">
                  <td className="px-4 py-2">{new Date(venta.fecha).toLocaleDateString()}</td>
                  <td className="px-4 py-2">{venta.tipoComprobante}</td>
                  <td className="px-4 py-2">S/ {venta.total.toFixed(2)}</td>
                  <td className="px-4 py-2">{venta.estado}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </>
      )}
    </div>
  )
}
