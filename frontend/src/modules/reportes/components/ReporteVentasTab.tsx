import { useQuery } from '@tanstack/react-query'
import { useState } from 'react'
import { reportesApi } from '@/modules/reportes/api/reportesApi'

export function ReporteVentasTab() {
  const [desde, setDesde] = useState('')
  const [hasta, setHasta] = useState('')

  const { data: reporte, isLoading } = useQuery({
    queryKey: ['reportes', 'ventas', desde, hasta],
    queryFn: () => reportesApi.ventas(desde || undefined, hasta || undefined),
  })

  return (
    <div>
      <div className="mb-4 flex gap-3">
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
