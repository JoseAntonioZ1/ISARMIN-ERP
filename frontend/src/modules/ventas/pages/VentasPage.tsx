import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useState } from 'react'
import { mediosPagoApi } from '@/modules/catalogos/api/catalogosApi'
import { clientesApi } from '@/modules/clientes/api/clientesApi'
import { productosApi } from '@/modules/productos/api/productosApi'
import { usuariosApi } from '@/modules/usuarios/api/usuariosApi'
import { RegistrarVentaDialog } from '@/modules/ventas/components/RegistrarVentaDialog'
import { VentaDetalleDialog } from '@/modules/ventas/components/VentaDetalleDialog'
import { type DetalleVentaInput, type PagoVentaInput, type TipoComprobante, ventasApi } from '@/modules/ventas/api/ventasApi'
import { ApiError } from '@/shared/api/httpClient'

export function VentasPage() {
  const queryClient = useQueryClient()
  const [registrando, setRegistrando] = useState(false)
  const [verDetalle, setVerDetalle] = useState<string | null>(null)
  const [error, setError] = useState<string | null>(null)

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

  const { data: mediosPago } = useQuery({ queryKey: ['medios-pago'], queryFn: mediosPagoApi.listar })

  const { data: usuarios } = useQuery({ queryKey: ['usuarios', 'todos'], queryFn: () => usuariosApi.listar(1, 200) })

  const nombreCliente = (id: string | null) => (id ? clientes?.datos.find((c) => c.id === id)?.nombreRazonSocial ?? '—' : 'Sin cliente')

  const mutacionRegistrar = useMutation({
    mutationFn: ({
      clienteId,
      tipoComprobante,
      detalles,
      pagos,
      usuarioAutorizoSaldoId,
    }: {
      clienteId: string | null
      tipoComprobante: TipoComprobante
      detalles: DetalleVentaInput[]
      pagos: PagoVentaInput[]
      usuarioAutorizoSaldoId: string | null
    }) => ventasApi.registrar(clienteId, tipoComprobante, detalles, pagos, usuarioAutorizoSaldoId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['ventas', 'lista'] })
      queryClient.invalidateQueries({ queryKey: ['productos', 'todos'] })
      setRegistrando(false)
    },
    onError: (e) => setError(e instanceof ApiError ? e.message : 'No se pudo registrar la venta.'),
  })

  return (
    <div>
      <div className="mb-4 flex items-center justify-between">
        <h1 className="text-xl font-semibold text-slate-800 dark:text-slate-100">Ventas</h1>
        <button
          type="button"
          onClick={() => setRegistrando(true)}
          className="rounded bg-slate-800 px-4 py-2 text-sm text-white hover:bg-slate-700 dark:bg-slate-600 dark:hover:bg-slate-500"
        >
          Nueva venta
        </button>
      </div>

      {error && <p className="mb-4 text-sm text-red-600">{error}</p>}

      {isLoading ? (
        <p className="text-slate-500">Cargando...</p>
      ) : (
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
                    className="text-slate-700 underline hover:text-slate-900 dark:text-slate-300"
                  >
                    Ver / Gestionar
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {registrando && (
        <RegistrarVentaDialog
          clientes={clientes?.datos ?? []}
          productos={productos?.datos ?? []}
          mediosPago={mediosPago ?? []}
          usuarios={usuarios?.datos ?? []}
          onGuardar={(datos) => {
            setError(null)
            mutacionRegistrar.mutate(datos)
          }}
          onCancelar={() => setRegistrando(false)}
          guardando={mutacionRegistrar.isPending}
        />
      )}

      {verDetalle && (
        <VentaDetalleDialog ventaId={verDetalle} productos={productos?.datos ?? []} onCerrar={() => setVerDetalle(null)} />
      )}
    </div>
  )
}
