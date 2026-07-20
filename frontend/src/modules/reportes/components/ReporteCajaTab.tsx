import { useQuery } from '@tanstack/react-query'
import { useState } from 'react'
import { reportesApi } from '@/modules/reportes/api/reportesApi'

export function ReporteCajaTab() {
  const [desde, setDesde] = useState('')
  const [hasta, setHasta] = useState('')

  const { data: reporte, isLoading } = useQuery({
    queryKey: ['reportes', 'caja', desde, hasta],
    queryFn: () => reportesApi.caja(desde || undefined, hasta || undefined),
  })

  return (
    <div>
      <p className="mb-4 text-xs text-slate-500">
        Refleja únicamente los movimientos manuales de Caja (aperturas, cierres, gastos y aportes) — no incluye cobros de
        Ventas, Taller ni Servicios de Campo.
      </p>

      <div className="mb-4 flex gap-3">
        <div>
          <label className="mb-1 block text-xs text-slate-500">Desde</label>
          <input
            type="date"
            value={desde}
            onChange={(e) => setDesde(e.target.value)}
            className="rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
          />
        </div>
        <div>
          <label className="mb-1 block text-xs text-slate-500">Hasta</label>
          <input
            type="date"
            value={hasta}
            onChange={(e) => setHasta(e.target.value)}
            className="rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
          />
        </div>
      </div>

      {isLoading || !reporte ? (
        <p className="text-slate-500">Cargando...</p>
      ) : (
        <>
          <div className="mb-4 grid grid-cols-3 gap-4">
            <div className="rounded-lg bg-white p-4 shadow-sm dark:bg-slate-800">
              <p className="text-xs text-slate-500">Total ingresos</p>
              <p className="text-xl font-semibold text-emerald-600">S/ {reporte.totalIngresos.toFixed(2)}</p>
            </div>
            <div className="rounded-lg bg-white p-4 shadow-sm dark:bg-slate-800">
              <p className="text-xs text-slate-500">Total egresos</p>
              <p className="text-xl font-semibold text-red-600">S/ {reporte.totalEgresos.toFixed(2)}</p>
            </div>
            <div className="rounded-lg bg-white p-4 shadow-sm dark:bg-slate-800">
              <p className="text-xs text-slate-500">Saldo neto</p>
              <p className="text-xl font-semibold text-slate-800 dark:text-slate-100">S/ {reporte.saldoNeto.toFixed(2)}</p>
            </div>
          </div>

          <table className="w-full border-collapse overflow-hidden rounded-lg bg-white text-left text-sm shadow-sm dark:bg-slate-800">
            <thead className="bg-slate-100 dark:bg-slate-700">
              <tr>
                <th className="px-4 py-2">Fecha</th>
                <th className="px-4 py-2">Tipo</th>
                <th className="px-4 py-2">Concepto</th>
                <th className="px-4 py-2">Monto</th>
              </tr>
            </thead>
            <tbody>
              {reporte.movimientos.map((m) => (
                <tr key={m.id} className="border-t border-slate-200 dark:border-slate-700">
                  <td className="px-4 py-2">{new Date(m.fecha).toLocaleDateString()}</td>
                  <td className="px-4 py-2">{m.tipo}</td>
                  <td className="px-4 py-2">{m.concepto}</td>
                  <td className="px-4 py-2">S/ {m.monto.toFixed(2)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </>
      )}
    </div>
  )
}
