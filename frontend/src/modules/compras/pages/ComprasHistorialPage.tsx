import { useQuery } from '@tanstack/react-query'
import { useState } from 'react'
import { type Compra, comprasApi } from '@/modules/compras/api/comprasApi'
import { productosApi } from '@/modules/productos/api/productosApi'
import { proveedoresApi } from '@/modules/proveedores/api/proveedoresApi'
import { EstadoCarga } from '@/shared/components/EstadoCarga'

export function ComprasHistorialPage() {
  const [verDetalle, setVerDetalle] = useState<Compra | null>(null)

  const { data: listado, isLoading } = useQuery({
    queryKey: ['compras'],
    queryFn: () => comprasApi.buscar(),
  })

  const { data: proveedores } = useQuery({
    queryKey: ['proveedores', 'todos'],
    queryFn: () => proveedoresApi.buscar(undefined, 1, 200),
  })

  const { data: productos } = useQuery({
    queryKey: ['productos', 'todos'],
    queryFn: () => productosApi.buscar(undefined, 1, 200),
  })

  const nombreProveedor = (id: string) => proveedores?.datos.find((p) => p.id === id)?.nombreRazonSocial ?? '—'
  const nombreProducto = (id: string) => {
    const producto = productos?.datos.find((p) => p.id === id)
    return producto ? `${producto.codigoInterno} — ${producto.nombre}` : '—'
  }

  return (
    <div>
      <h1 className="mb-4 text-xl font-semibold text-slate-800 dark:text-slate-100">Historial de Compras</h1>

      {isLoading ? (
        <EstadoCarga />
      ) : (
        <div className="overflow-x-auto">
        <table className="w-full border-collapse overflow-hidden rounded-lg bg-white text-left text-sm shadow-sm dark:bg-slate-800">
          <thead className="bg-slate-100 dark:bg-slate-700">
            <tr>
              <th className="px-4 py-2">Fecha</th>
              <th className="px-4 py-2">Proveedor</th>
              <th className="px-4 py-2">Documento</th>
              <th className="px-4 py-2">Total</th>
              <th className="px-4 py-2">Acciones</th>
            </tr>
          </thead>
          <tbody>
            {listado?.datos.map((compra) => (
              <tr key={compra.id} className="border-t border-slate-200 dark:border-slate-700">
                <td className="px-4 py-2">{compra.fecha}</td>
                <td className="px-4 py-2">{nombreProveedor(compra.proveedorId)}</td>
                <td className="px-4 py-2">
                  {compra.documentoCompraTipo} {compra.documentoCompraNumero}
                </td>
                <td className="px-4 py-2">{compra.total.toFixed(2)}</td>
                <td className="px-4 py-2">
                  <button
                    type="button"
                    onClick={() => setVerDetalle(compra)}
                    className="text-[var(--color-apoyo)] underline hover:text-slate-900 dark:text-slate-300"
                  >
                    Ver detalle
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        </div>
      )}

      {verDetalle && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
          <div className="max-h-[85vh] w-full max-w-xl overflow-y-auto rounded-lg bg-white p-6 shadow-lg dark:bg-slate-800">
            <h2 className="mb-1 text-lg font-semibold text-slate-800 dark:text-slate-100">
              Compra {verDetalle.documentoCompraTipo} {verDetalle.documentoCompraNumero}
            </h2>
            <p className="mb-4 text-sm text-[var(--color-terciario)] dark:text-slate-400">
              {nombreProveedor(verDetalle.proveedorId)} — {verDetalle.fecha}
            </p>

            <div className="overflow-x-auto">
            <table className="w-full border-collapse overflow-hidden rounded-lg bg-white text-left text-sm shadow-sm dark:bg-slate-800">
              <thead className="bg-slate-100 dark:bg-slate-700">
                <tr>
                  <th className="px-4 py-2">Producto</th>
                  <th className="px-4 py-2">Cantidad</th>
                  <th className="px-4 py-2">Costo unitario</th>
                  <th className="px-4 py-2">Subtotal</th>
                </tr>
              </thead>
              <tbody>
                {verDetalle.detalles.map((detalle) => (
                  <tr key={detalle.id} className="border-t border-slate-200 dark:border-slate-700">
                    <td className="px-4 py-2">{nombreProducto(detalle.productoId)}</td>
                    <td className="px-4 py-2">{detalle.cantidad}</td>
                    <td className="px-4 py-2">{detalle.costoUnitario.toFixed(2)}</td>
                    <td className="px-4 py-2">{(detalle.cantidad * detalle.costoUnitario).toFixed(2)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
            </div>

            <p className="mt-3 text-right text-sm font-semibold text-slate-800 dark:text-slate-100">
              Total: {verDetalle.total.toFixed(2)}
            </p>

            <div className="mt-5 flex justify-end">
              <button
                type="button"
                onClick={() => setVerDetalle(null)}
                className="rounded border border-slate-300 px-4 py-2 text-sm hover:bg-slate-100 dark:border-slate-600 dark:hover:bg-slate-700"
              >
                Cerrar
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
