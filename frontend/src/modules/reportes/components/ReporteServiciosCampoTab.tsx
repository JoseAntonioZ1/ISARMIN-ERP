import { useQuery } from '@tanstack/react-query'
import { useMemo, useState } from 'react'
import { reportesApi } from '@/modules/reportes/api/reportesApi'
import type { EstadoServicioCampo } from '@/modules/serviciosCampo/api/serviciosCampoApi'
import { ESTADOS_SERVICIO_CAMPO_VISUAL } from '@/modules/serviciosCampo/utils/estadoServicioCampoVisual'
import { usuariosApi } from '@/modules/usuarios/api/usuariosApi'
import { BotonExportarCsv } from '@/shared/components/BotonExportarCsv'
import { GraficoBarras } from '@/shared/components/GraficoBarras'
import { exportarCsv } from '@/shared/utils/exportarCsv'

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

  const conteoPorEstado = useMemo(() => {
    const mapa = new Map<EstadoServicioCampo, number>()
    for (const s of reporte?.servicios ?? []) {
      mapa.set(s.estado, (mapa.get(s.estado) ?? 0) + 1)
    }
    return Array.from(mapa.entries()).map(([est, valor]) => ({
      etiqueta: ESTADOS_SERVICIO_CAMPO_VISUAL[est].etiqueta,
      valor,
      claseColor: ESTADOS_SERVICIO_CAMPO_VISUAL[est].claseBarra,
    }))
  }, [reporte])

  const handleExportar = () => {
    if (!reporte) return
    exportarCsv(
      `reporte-servicios-campo-${desde || 'inicio'}_${hasta || 'fin'}.csv`,
      ['Solicitud', 'Trabajo', 'Técnico', 'Estado'],
      reporte.servicios.map((s) => [
        new Date(s.fechaSolicitud).toLocaleDateString(),
        s.descripcionTrabajo,
        nombreTecnico(s.tecnicoAsignadoId),
        s.estado,
      ]),
    )
  }

  return (
    <div>
      <div className="mb-4 flex items-end justify-between gap-3">
        <div className="flex gap-3">
          <div>
            <label className="mb-1 block text-xs text-[var(--color-terciario)]">Técnico</label>
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
        <BotonExportarCsv onClick={handleExportar} disabled={!reporte || reporte.servicios.length === 0} />
      </div>

      {isLoading || !reporte ? (
        <p className="text-[var(--color-terciario)]">Cargando...</p>
      ) : (
        <>
          <div className="mb-4 rounded-lg bg-white p-4 shadow-sm dark:bg-slate-800">
            <p className="text-xs text-[var(--color-terciario)]">Cantidad de servicios</p>
            <p className="text-xl font-semibold text-slate-800 dark:text-slate-100">{reporte.cantidadTotal}</p>
          </div>

          <div className="mb-4 rounded-lg bg-white p-4 shadow-sm dark:bg-slate-800">
            <h3 className="mb-3 text-sm font-semibold text-[var(--color-apoyo)] dark:text-slate-300">Servicios por estado</h3>
            <GraficoBarras datos={conteoPorEstado} />
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
