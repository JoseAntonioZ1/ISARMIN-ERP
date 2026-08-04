import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useState } from 'react'
import { mediosPagoApi } from '@/modules/catalogos/api/catalogosApi'
import { clientesApi } from '@/modules/clientes/api/clientesApi'
import type { Producto } from '@/modules/productos/api/productosApi'
import { type DetalleDevolucionInput, ventasApi } from '@/modules/ventas/api/ventasApi'
import { generarComprobantePdf } from '@/modules/ventas/utils/comprobantePdf'
import { ApiError } from '@/shared/api/httpClient'
import { EstadoCarga } from '@/shared/components/EstadoCarga'
import { useBranding } from '@/shared/hooks/useBranding'

interface VentaDetalleDialogProps {
  ventaId: string
  productos: Producto[]
  onCerrar: () => void
}

export function VentaDetalleDialog({ ventaId, productos, onCerrar }: VentaDetalleDialogProps) {
  const queryClient = useQueryClient()
  const [error, setError] = useState<string | null>(null)

  const { data: venta, isLoading } = useQuery({
    queryKey: ['ventas', ventaId],
    queryFn: () => ventasApi.obtener(ventaId),
  })

  const { data: mediosPago } = useQuery({ queryKey: ['medios-pago'], queryFn: mediosPagoApi.listar })
  const { data: branding } = useBranding()
  const { data: clientes } = useQuery({ queryKey: ['clientes', 'todos'], queryFn: () => clientesApi.buscar(undefined, 1, 200) })

  const handleDescargarComprobante = () => {
    if (!venta) return
    const cliente = venta.clienteId ? (clientes?.datos.find((c) => c.id === venta.clienteId) ?? null) : null
    generarComprobantePdf({
      venta,
      productos,
      cliente,
      mediosPago: mediosPago ?? [],
      branding: branding ?? null,
    }).save(`comprobante-${venta.tipoComprobante}-${venta.id.slice(0, 8)}.pdf`)
  }

  const invalidar = () => {
    queryClient.invalidateQueries({ queryKey: ['ventas', ventaId] })
    queryClient.invalidateQueries({ queryKey: ['ventas', 'lista'] })
  }

  const onError = (e: unknown, mensaje: string) => setError(e instanceof ApiError ? e.message : mensaje)

  const mutacionAnular = useMutation({
    mutationFn: (motivo: string) => ventasApi.anular(ventaId, motivo),
    onSuccess: invalidar,
    onError: (e) => onError(e, 'No se pudo anular la venta.'),
  })

  const mutacionDevolucion = useMutation({
    mutationFn: ({ detalles, motivo }: { detalles: DetalleDevolucionInput[]; motivo: string | null }) =>
      ventasApi.registrarDevolucion(ventaId, detalles, motivo),
    onSuccess: invalidar,
    onError: (e) => onError(e, 'No se pudo registrar la devolución.'),
  })

  const nombreProducto = (id: string) => {
    const producto = productos.find((p) => p.id === id)
    return producto ? `${producto.codigoInterno} — ${producto.nombre}` : id
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
      <div className="max-h-[90vh] w-full max-w-2xl overflow-y-auto rounded-lg bg-white p-6 shadow-lg dark:bg-slate-800">
        {isLoading || !venta ? (
          <EstadoCarga />
        ) : (
          <>
            <div className="mb-4 flex items-center justify-between">
              <h2 className="text-lg font-semibold text-slate-800 dark:text-slate-100">Venta — {venta.tipoComprobante}</h2>
              <span className="rounded bg-slate-100 px-2 py-1 text-xs font-medium text-[var(--color-apoyo)] dark:bg-slate-700 dark:text-slate-200">
                {venta.estado}
              </span>
            </div>

            {error && <p className="mb-4 text-sm text-red-600">{error}</p>}

            <p className="mb-4 text-sm text-slate-600 dark:text-slate-400">
              <span className="font-semibold">Fecha:</span> {new Date(venta.fecha).toLocaleString()}
            </p>

            <div className="mb-2 text-sm">
              <span className="font-semibold">Productos:</span>
              <ul className="ml-4 list-disc">
                {venta.detalles.map((d) => (
                  <li key={d.id}>
                    {nombreProducto(d.productoId)} — {d.cantidad} x S/ {d.precioUnitario.toFixed(2)}
                  </li>
                ))}
              </ul>
            </div>

            <p className="mb-2 text-sm">
              <span className="font-semibold">Total:</span> S/ {venta.total.toFixed(2)}
            </p>

            {venta.pagos.length > 0 && (
              <div className="mb-2 text-sm">
                <span className="font-semibold">Pagos:</span>
                <ul className="ml-4 list-disc">
                  {venta.pagos.map((p) => (
                    <li key={p.id}>S/ {p.monto.toFixed(2)}</li>
                  ))}
                </ul>
              </div>
            )}

            {venta.saldoPendiente != null && (
              <p className="mb-2 text-sm text-[var(--color-secundario)]">Saldo pendiente: S/ {venta.saldoPendiente.toFixed(2)}</p>
            )}

            {venta.estado === 'Anulada' && venta.motivoAnulacion && (
              <p className="mb-2 text-sm">
                <span className="font-semibold">Motivo de anulación:</span> {venta.motivoAnulacion}
              </p>
            )}

            {venta.estado !== 'Anulada' && (
              <div className="mt-4 space-y-4 border-t border-slate-200 pt-4 dark:border-slate-700">
                <FormularioDevolucion
                  productos={productos}
                  detallesVenta={venta.detalles}
                  guardando={mutacionDevolucion.isPending}
                  onGuardar={(datos) => {
                    setError(null)
                    mutacionDevolucion.mutate(datos)
                  }}
                />
                <FormularioAnular
                  guardando={mutacionAnular.isPending}
                  onGuardar={(motivo) => {
                    setError(null)
                    mutacionAnular.mutate(motivo)
                  }}
                />
              </div>
            )}
          </>
        )}

        <div className="mt-5 flex justify-end gap-3">
          {venta && (
            <button
              type="button"
              onClick={handleDescargarComprobante}
              className="rounded bg-[var(--color-principal)] px-4 py-2 text-sm text-white hover:brightness-90 dark:bg-[var(--color-principal)] dark:hover:brightness-110"
            >
              Descargar comprobante
            </button>
          )}
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

function FormularioDevolucion({
  productos,
  detallesVenta,
  onGuardar,
  guardando,
}: {
  productos: Producto[]
  detallesVenta: { productoId: string }[]
  onGuardar: (datos: { detalles: DetalleDevolucionInput[]; motivo: string | null }) => void
  guardando?: boolean
}) {
  const [filas, setFilas] = useState<{ productoId: string; cantidad: string }[]>([])
  const [motivo, setMotivo] = useState('')

  const productosVendidos = productos.filter((p) => detallesVenta.some((d) => d.productoId === p.id))

  const agregarFila = () => setFilas([...filas, { productoId: '', cantidad: '' }])
  const quitarFila = (index: number) => setFilas(filas.filter((_, i) => i !== index))
  const actualizarFila = (index: number, campo: 'productoId' | 'cantidad', valor: string) =>
    setFilas(filas.map((f, i) => (i === index ? { ...f, [campo]: valor } : f)))

  const handleGuardar = () => {
    const detalles = filas
      .filter((f) => f.productoId && f.cantidad)
      .map((f) => ({ productoId: f.productoId, cantidad: Number(f.cantidad) }))
    onGuardar({ detalles, motivo: motivo.trim() || null })
    setFilas([])
    setMotivo('')
  }

  return (
    <div>
      <h3 className="mb-2 text-sm font-semibold text-[var(--color-apoyo)] dark:text-slate-300">Registrar devolución</h3>

      <div className="mb-2 flex items-center justify-between">
        <span className="text-xs text-[var(--color-terciario)]">Productos devueltos</span>
        <button type="button" onClick={agregarFila} className="text-xs text-[var(--color-apoyo)] underline dark:text-slate-300">
          Agregar producto
        </button>
      </div>

      {filas.map((fila, index) => (
        <div key={index} className="mb-2 grid grid-cols-[2fr_1fr_auto] gap-2">
          <select
            value={fila.productoId}
            onChange={(e) => actualizarFila(index, 'productoId', e.target.value)}
            className="rounded border border-slate-300 px-2 py-1 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
          >
            <option value="">Producto...</option>
            {productosVendidos.map((p) => (
              <option key={p.id} value={p.id}>
                {p.codigoInterno} — {p.nombre}
              </option>
            ))}
          </select>
          <input
            type="number"
            step="0.001"
            value={fila.cantidad}
            onChange={(e) => actualizarFila(index, 'cantidad', e.target.value)}
            placeholder="Cantidad"
            className="rounded border border-slate-300 px-2 py-1 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
          />
          <button type="button" onClick={() => quitarFila(index)} className="text-red-600 underline">
            Quitar
          </button>
        </div>
      ))}

      <label className="mb-1 mt-2 block text-xs text-[var(--color-terciario)]">Motivo (opcional)</label>
      <input
        value={motivo}
        onChange={(e) => setMotivo(e.target.value)}
        className="mb-2 w-full rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
      />

      <button
        type="button"
        disabled={filas.filter((f) => f.productoId && f.cantidad).length === 0 || guardando}
        onClick={handleGuardar}
        className="rounded bg-[var(--color-principal)] px-4 py-2 text-sm text-white hover:brightness-90 disabled:opacity-50 dark:bg-[var(--color-principal)] dark:hover:brightness-110"
      >
        {guardando ? 'Guardando...' : 'Registrar devolución'}
      </button>
    </div>
  )
}

function FormularioAnular({ onGuardar, guardando }: { onGuardar: (motivo: string) => void; guardando?: boolean }) {
  const [motivo, setMotivo] = useState('')

  return (
    <div>
      <h3 className="mb-2 text-sm font-semibold text-[var(--color-apoyo)] dark:text-slate-300">Anular venta</h3>
      <div className="flex gap-2">
        <input
          value={motivo}
          onChange={(e) => setMotivo(e.target.value)}
          placeholder="Motivo de anulación"
          className="flex-1 rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />
        <button
          type="button"
          disabled={!motivo.trim() || guardando}
          onClick={() => onGuardar(motivo.trim())}
          className="rounded border border-red-300 px-4 py-2 text-sm text-red-600 hover:bg-red-50 disabled:opacity-50"
        >
          {guardando ? 'Guardando...' : 'Anular'}
        </button>
      </div>
    </div>
  )
}
