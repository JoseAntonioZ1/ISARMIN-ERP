import { useQuery } from '@tanstack/react-query'
import { reportesApi } from '@/modules/reportes/api/reportesApi'

export function ReporteInventarioTab() {
  const { data: reporte, isLoading } = useQuery({
    queryKey: ['reportes', 'inventario'],
    queryFn: () => reportesApi.inventario(),
  })

  if (isLoading || !reporte) {
    return <p className="text-[var(--color-terciario)]">Cargando...</p>
  }

  return (
    <div>
      <div className="mb-4 grid grid-cols-2 gap-4">
        <div className="rounded-lg bg-white p-4 shadow-sm dark:bg-slate-800">
          <p className="text-xs text-[var(--color-terciario)]">Productos</p>
          <p className="text-xl font-semibold text-slate-800 dark:text-slate-100">{reporte.productos.length}</p>
        </div>
        <div className="rounded-lg bg-white p-4 shadow-sm dark:bg-slate-800">
          <p className="text-xs text-[var(--color-terciario)]">En quiebre de stock</p>
          <p className="text-xl font-semibold text-[var(--color-secundario)]">{reporte.productosEnQuiebre.length}</p>
        </div>
      </div>

      {reporte.productosEnQuiebre.length > 0 && (
        <div className="mb-4">
          <h3 className="mb-2 text-sm font-semibold text-[var(--color-secundario)]">Productos en quiebre</h3>
          <table className="w-full border-collapse overflow-hidden rounded-lg bg-white text-left text-sm shadow-sm dark:bg-slate-800">
            <thead className="bg-slate-100 dark:bg-slate-700">
              <tr>
                <th className="px-4 py-2">Código</th>
                <th className="px-4 py-2">Nombre</th>
                <th className="px-4 py-2">Stock actual</th>
                <th className="px-4 py-2">Stock mínimo</th>
              </tr>
            </thead>
            <tbody>
              {reporte.productosEnQuiebre.map((p) => (
                <tr key={p.id} className="border-t border-slate-200 dark:border-slate-700">
                  <td className="px-4 py-2">{p.codigoInterno}</td>
                  <td className="px-4 py-2">{p.nombre}</td>
                  <td className="px-4 py-2 text-[var(--color-secundario)]">{p.stockActual}</td>
                  <td className="px-4 py-2">{p.stockMinimo}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      <h3 className="mb-2 text-sm font-semibold text-[var(--color-apoyo)] dark:text-slate-300">Todos los productos</h3>
      <table className="w-full border-collapse overflow-hidden rounded-lg bg-white text-left text-sm shadow-sm dark:bg-slate-800">
        <thead className="bg-slate-100 dark:bg-slate-700">
          <tr>
            <th className="px-4 py-2">Código</th>
            <th className="px-4 py-2">Nombre</th>
            <th className="px-4 py-2">Stock actual</th>
            <th className="px-4 py-2">Estado</th>
          </tr>
        </thead>
        <tbody>
          {reporte.productos.map((p) => (
            <tr key={p.id} className="border-t border-slate-200 dark:border-slate-700">
              <td className="px-4 py-2">{p.codigoInterno}</td>
              <td className="px-4 py-2">{p.nombre}</td>
              <td className="px-4 py-2">{p.stockActual}</td>
              <td className="px-4 py-2">{p.estado}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
