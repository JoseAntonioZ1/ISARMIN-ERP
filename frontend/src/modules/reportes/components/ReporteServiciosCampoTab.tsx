import { useQuery } from '@tanstack/react-query'
import { useState } from 'react'
import { reportesApi } from '@/modules/reportes/api/reportesApi'
import { usuariosApi } from '@/modules/usuarios/api/usuariosApi'

export function ReporteServiciosCampoTab() {
  const [tecnico, setTecnico] = useState('')
  const [desde, setDesde] = useState('')
  const [hasta, setHasta] = useState('')

  const { data: usuarios } = useQuery({ queryKey: ['usuarios', 'todos'], queryFn: () => usuariosApi.listar(1, 200) })

  const { data: reporte, isLoading } = useQuery({
    queryKey: ['reportes', 'servicios-campo', tecnico, desde, hasta],
    queryFn: () => reportesApi.serviciosCampo(tecnico || undefined, desde || undefined, hasta || undefined),
  })

  const nombreTecnico = (id: string | null) => (id ? usuarios?.datos.find((u) => u.id === id)?.nombre ?? '—' : 'Sin asignar')

  return (
    <div>
      <div className="mb-4 flex gap-3">
        <div>
          <label className="mb-1 block text-xs text-slate-500">Técnico</label>
          <select
            value={tecnico}
            onChange={(e) => setTecnico(e.target.value)}
            className="rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
          >
            <option value="">Todos</option>
            {usuarios?.datos.map((u) => (
              <option key={u.id} value={u.id}>
                {u.nombre}
              </option>
            ))}
          </select>
        </div>
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
          <div className="mb-4 rounded-lg bg-white p-4 shadow-sm dark:bg-slate-800">
            <p className="text-xs text-slate-500">Cantidad de servicios</p>
            <p className="text-xl font-semibold text-slate-800 dark:text-slate-100">{reporte.cantidadTotal}</p>
          </div>

          <table className="w-full border-collapse overflow-hidden rounded-lg bg-white text-left text-sm shadow-sm dark:bg-slate-800">
            <thead className="bg-slate-100 dark:bg-slate-700">
              <tr>
                <th className="px-4 py-2">Solicitud</th>
                <th className="px-4 py-2">Trabajo</th>
                <th className="px-4 py-2">Técnico</th>
                <th className="px-4 py-2">Estado</th>
              </tr>
            </thead>
            <tbody>
              {reporte.servicios.map((s) => (
                <tr key={s.id} className="border-t border-slate-200 dark:border-slate-700">
                  <td className="px-4 py-2">{new Date(s.fechaSolicitud).toLocaleDateString()}</td>
                  <td className="px-4 py-2">{s.descripcionTrabajo}</td>
                  <td className="px-4 py-2">{nombreTecnico(s.tecnicoAsignadoId)}</td>
                  <td className="px-4 py-2">{s.estado}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </>
      )}
    </div>
  )
}
