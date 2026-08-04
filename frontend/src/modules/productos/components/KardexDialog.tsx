import { useQuery } from '@tanstack/react-query'
import type { Producto } from '@/modules/productos/api/productosApi'
import { productosApi } from '@/modules/productos/api/productosApi'
import { EstadoCarga } from '@/shared/components/EstadoCarga'

interface KardexDialogProps {
  producto: Producto
  onCerrar: () => void
}

export function KardexDialog({ producto, onCerrar }: KardexDialogProps) {
  const { data: movimientos, isLoading } = useQuery({
    queryKey: ['productos', producto.id, 'movimientos'],
    queryFn: () => productosApi.consultarKardex(producto.id),
  })

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
      <div className="max-h-[85vh] w-full max-w-2xl overflow-y-auto rounded-lg bg-white p-6 shadow-lg dark:bg-slate-800">
        <h2 className="mb-1 text-lg font-semibold text-slate-800 dark:text-slate-100">Kardex — {producto.nombre}</h2>
        <p className="mb-4 text-sm text-[var(--color-terciario)] dark:text-slate-400">Stock actual: {producto.stockActual}</p>

        {isLoading ? (
          <EstadoCarga />
        ) : !movimientos || movimientos.length === 0 ? (
          <p className="text-[var(--color-terciario)]">Aún no hay movimientos registrados para este producto.</p>
        ) : (
          <div className="overflow-x-auto">
          <table className="w-full border-collapse overflow-hidden rounded-lg bg-white text-left text-sm shadow-sm dark:bg-slate-800">
            <thead className="bg-slate-100 dark:bg-slate-700">
              <tr>
                <th className="px-4 py-2">Fecha</th>
                <th className="px-4 py-2">Tipo</th>
                <th className="px-4 py-2">Cantidad</th>
                <th className="px-4 py-2">Motivo</th>
              </tr>
            </thead>
            <tbody>
              {movimientos.map((movimiento) => (
                <tr key={movimiento.id} className="border-t border-slate-200 dark:border-slate-700">
                  <td className="px-4 py-2">{new Date(movimiento.fecha).toLocaleString()}</td>
                  <td className="px-4 py-2">{movimiento.tipoMovimiento}</td>
                  <td className="px-4 py-2">{movimiento.cantidad}</td>
                  <td className="px-4 py-2">{movimiento.motivoAjuste ?? '—'}</td>
                </tr>
              ))}
            </tbody>
          </table>
          </div>
        )}

        <div className="mt-5 flex justify-end">
          <button
            type="button"
            onClick={onCerrar}
            className="rounded border border-slate-300 px-4 py-2 text-sm hover:bg-slate-100 dark:border-slate-600 dark:hover:bg-slate-700"
          >
            Cerrar
          </button>
        </div>
      </div>
    </div>
  )
}
