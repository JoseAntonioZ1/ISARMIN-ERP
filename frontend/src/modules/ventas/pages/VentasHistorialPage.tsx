import { useQuery } from '@tanstack/react-query'
import { useState } from 'react'
import { clientesApi } from '@/modules/clientes/api/clientesApi'
import { productosApi } from '@/modules/productos/api/productosApi'
import { VentaDetalleDialog } from '@/modules/ventas/components/VentaDetalleDialog'
import { ventasApi } from '@/modules/ventas/api/ventasApi'
import { EstadoCarga } from '@/shared/components/EstadoCarga'

export function VentasHistorialPage() {
  const [verDetalle, setVerDetalle] = useState<string | null>(null)

  const { data: listado, isLoading } = useQuery({
    queryKey: ['ventas', 'lista'],
    queryFn: () => ventasApi.buscar(),
  })

  const { data: clientes } = useQuery({
    queryKey: ['clientes', 'todos'],
    queryFn: () => clientesApi.buscar(undefined, 1, 200),
  })

  const { data: productos } = useQuery({
    queryKey: ['productos', 'todos'],
    queryFn: () => productosApi.buscar(undefined, 1, 200),
  })

  const nombreCliente = (id: string | null) => (id ? clientes?.datos.find((c) => c.id === id)?.nombreRazonSocial ?? '—' : 'Sin cliente')

  return (
    <div>
      <h1 className="mb-4 text-xl font-semibold text-slate-800 dark:text-slate-100">Historial de Ventas</h1>

      {isLoading ? (
        <EstadoCarga />
      ) : (
        <div className="overflow-x-auto">
        <table className="w-full border-collapse overflow-hidden rounded-lg bg-white text-left text-sm shadow-sm dark:bg-slate-800">
          <thead className="bg-slate-100 dark:bg-slate-700">
            <tr>
              <th className="px-4 py-2">Fecha</th>
              <th className="px-4 py-2">Cliente</th>
              <th className="px-4 py-2">Comprobante</th>
              <th className="px-4 py-2">Total</th>
              <th className="px-4 py-2">Estado</th>
              <th className="px-4 py-2">Acciones</th>
            </tr>
          </thead>
          <tbody>
            {listado?.datos.map((venta) => (
              <tr key={venta.id} className="border-t border-slate-200 dark:border-slate-700">
                <td className="px-4 py-2">{new Date(venta.fecha).toLocaleDateString()}</td>
                <td className="px-4 py-2">{nombreCliente(venta.clienteId)}</td>
                <td className="px-4 py-2">{venta.tipoComprobante}</td>
                <td className="px-4 py-2">S/ {venta.total.toFixed(2)}</td>
                <td className="px-4 py-2">{venta.estado}</td>
                <td className="px-4 py-2">
                  <button
                    type="button"
                    onClick={() => setVerDetalle(venta.id)}
                    className="text-[var(--color-apoyo)] underline hover:text-slate-900 dark:text-slate-300"
                  >
                    Ver / Gestionar
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        </div>
      )}

      {verDetalle && (
        <VentaDetalleDialog ventaId={verDetalle} productos={productos?.datos ?? []} onCerrar={() => setVerDetalle(null)} />
      )}
    </div>
  )
}
