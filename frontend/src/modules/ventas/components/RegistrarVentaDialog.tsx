import { useState } from 'react'
import type { Cliente } from '@/modules/clientes/api/clientesApi'
import type { MedioPago } from '@/modules/catalogos/api/catalogosApi'
import type { Producto } from '@/modules/productos/api/productosApi'
import type { Usuario } from '@/modules/usuarios/api/usuariosApi'
import {
  type DetalleVentaInput,
  type PagoVentaInput,
  type TipoComprobante,
  TIPOS_COMPROBANTE,
} from '@/modules/ventas/api/ventasApi'

interface RegistrarVentaDialogProps {
  clientes: Cliente[]
  productos: Producto[]
  mediosPago: MedioPago[]
  usuarios: Usuario[]
  onGuardar: (datos: {
    clienteId: string | null
    tipoComprobante: TipoComprobante
    detalles: DetalleVentaInput[]
    pagos: PagoVentaInput[]
    usuarioAutorizoSaldoId: string | null
  }) => void
  onCancelar: () => void
  guardando?: boolean
}

export function RegistrarVentaDialog({
  clientes,
  productos,
  mediosPago,
  usuarios,
  onGuardar,
  onCancelar,
  guardando,
}: RegistrarVentaDialogProps) {
  const [clienteId, setClienteId] = useState('')
  const [tipoComprobante, setTipoComprobante] = useState<TipoComprobante>('Ticket')
  const [filasDetalle, setFilasDetalle] = useState<{ productoId: string; cantidad: string; precioUnitario: string }[]>([
    { productoId: '', cantidad: '', precioUnitario: '' },
  ])
  const [filasPago, setFilasPago] = useState<{ medioPagoId: string; monto: string }[]>([{ medioPagoId: '', monto: '' }])
  const [usuarioAutorizoId, setUsuarioAutorizoId] = useState('')

  const agregarFilaDetalle = () => setFilasDetalle([...filasDetalle, { productoId: '', cantidad: '', precioUnitario: '' }])
  const quitarFilaDetalle = (index: number) => setFilasDetalle(filasDetalle.filter((_, i) => i !== index))
  const actualizarFilaDetalle = (index: number, campo: 'productoId' | 'cantidad' | 'precioUnitario', valor: string) =>
    setFilasDetalle(
      filasDetalle.map((f, i) => {
        if (i !== index) return f
        if (campo === 'productoId') {
          const producto = productos.find((p) => p.id === valor)
          return { ...f, productoId: valor, precioUnitario: producto ? String(producto.precioVenta) : f.precioUnitario }
        }
        return { ...f, [campo]: valor }
      }),
    )

  const agregarFilaPago = () => setFilasPago([...filasPago, { medioPagoId: '', monto: '' }])
  const quitarFilaPago = (index: number) => setFilasPago(filasPago.filter((_, i) => i !== index))
  const actualizarFilaPago = (index: number, campo: 'medioPagoId' | 'monto', valor: string) =>
    setFilasPago(filasPago.map((f, i) => (i === index ? { ...f, [campo]: valor } : f)))

  const total = filasDetalle.reduce((suma, f) => suma + (Number(f.cantidad) || 0) * (Number(f.precioUnitario) || 0), 0)
  const montoPagado = filasPago.reduce((suma, f) => suma + (Number(f.monto) || 0), 0)
  const saldoPendiente = total - montoPagado

  const detallesValidos = filasDetalle.filter((f) => f.productoId && f.cantidad && f.precioUnitario)
  const pagosValidos = filasPago.filter((f) => f.medioPagoId && f.monto)

  const puedeGuardar =
    detallesValidos.length > 0 &&
    saldoPendiente >= 0 &&
    (saldoPendiente <= 0 || usuarioAutorizoId) &&
    !guardando

  const handleGuardar = () => {
    onGuardar({
      clienteId: clienteId || null,
      tipoComprobante,
      detalles: detallesValidos.map((f) => ({
        productoId: f.productoId,
        cantidad: Number(f.cantidad),
        precioUnitario: Number(f.precioUnitario),
      })),
      pagos: pagosValidos.map((f) => ({ medioPagoId: f.medioPagoId, monto: Number(f.monto) })),
      usuarioAutorizoSaldoId: saldoPendiente > 0 ? usuarioAutorizoId : null,
    })
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
      <div className="max-h-[90vh] w-full max-w-2xl overflow-y-auto rounded-lg bg-white p-6 shadow-lg dark:bg-slate-800">
        <h2 className="mb-4 text-lg font-semibold text-slate-800 dark:text-slate-100">Registrar venta</h2>

        <div className="mb-3 grid grid-cols-2 gap-3">
          <div>
            <label className="mb-1 block text-xs text-slate-500">Cliente (opcional)</label>
            <select
              value={clienteId}
              onChange={(e) => setClienteId(e.target.value)}
              className="w-full rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
            >
              <option value="">Sin cliente registrado</option>
              {clientes.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.nombreRazonSocial}
                </option>
              ))}
            </select>
          </div>
          <div>
            <label className="mb-1 block text-xs text-slate-500">Tipo de comprobante</label>
            <select
              value={tipoComprobante}
              onChange={(e) => setTipoComprobante(e.target.value as TipoComprobante)}
              className="w-full rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
            >
              {TIPOS_COMPROBANTE.map((tipo) => (
                <option key={tipo} value={tipo}>
                  {tipo}
                </option>
              ))}
            </select>
          </div>
        </div>

        <div className="mb-2 flex items-center justify-between">
          <span className="text-xs font-semibold text-slate-500">Productos</span>
          <button type="button" onClick={agregarFilaDetalle} className="text-xs text-slate-700 underline dark:text-slate-300">
            Agregar producto
          </button>
        </div>

        {filasDetalle.map((fila, index) => (
          <div key={index} className="mb-2 grid grid-cols-[2fr_0.8fr_0.8fr_auto] gap-2">
            <select
              value={fila.productoId}
              onChange={(e) => actualizarFilaDetalle(index, 'productoId', e.target.value)}
              className="rounded border border-slate-300 px-2 py-1 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
            >
              <option value="">Producto...</option>
              {productos.map((p) => (
                <option key={p.id} value={p.id}>
                  {p.codigoInterno} — {p.nombre}
                </option>
              ))}
            </select>
            <input
              type="number"
              step="0.001"
              value={fila.cantidad}
              onChange={(e) => actualizarFilaDetalle(index, 'cantidad', e.target.value)}
              placeholder="Cantidad"
              className="rounded border border-slate-300 px-2 py-1 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
            />
            <input
              type="number"
              step="0.01"
              value={fila.precioUnitario}
              onChange={(e) => actualizarFilaDetalle(index, 'precioUnitario', e.target.value)}
              placeholder="Precio"
              className="rounded border border-slate-300 px-2 py-1 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
            />
            <button type="button" onClick={() => quitarFilaDetalle(index)} className="text-red-600 underline">
              Quitar
            </button>
          </div>
        ))}

        <div className="mb-3 mt-2 text-right text-sm font-semibold text-slate-700 dark:text-slate-300">
          Total: S/ {total.toFixed(2)}
        </div>

        <div className="mb-2 flex items-center justify-between">
          <span className="text-xs font-semibold text-slate-500">Pagos</span>
          <button type="button" onClick={agregarFilaPago} className="text-xs text-slate-700 underline dark:text-slate-300">
            Agregar pago
          </button>
        </div>

        {filasPago.map((fila, index) => (
          <div key={index} className="mb-2 grid grid-cols-[2fr_1fr_auto] gap-2">
            <select
              value={fila.medioPagoId}
              onChange={(e) => actualizarFilaPago(index, 'medioPagoId', e.target.value)}
              className="rounded border border-slate-300 px-2 py-1 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
            >
              <option value="">Medio de pago...</option>
              {mediosPago.map((m) => (
                <option key={m.id} value={m.id}>
                  {m.nombre}
                </option>
              ))}
            </select>
            <input
              type="number"
              step="0.01"
              value={fila.monto}
              onChange={(e) => actualizarFilaPago(index, 'monto', e.target.value)}
              placeholder="Monto"
              className="rounded border border-slate-300 px-2 py-1 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
            />
            <button type="button" onClick={() => quitarFilaPago(index)} className="text-red-600 underline">
              Quitar
            </button>
          </div>
        ))}

        <div className="mb-2 text-right text-sm text-slate-600 dark:text-slate-400">
          Pagado: S/ {montoPagado.toFixed(2)}
          {saldoPendiente > 0 && <span className="ml-2 text-amber-600">— Saldo pendiente: S/ {saldoPendiente.toFixed(2)}</span>}
          {saldoPendiente < 0 && <span className="ml-2 text-red-600">— El monto pagado excede el total</span>}
        </div>

        {saldoPendiente > 0 && (
          <div className="mb-3">
            <label className="mb-1 block text-xs text-slate-500">Usuario Administrador/Propietario que autoriza el saldo pendiente</label>
            <select
              value={usuarioAutorizoId}
              onChange={(e) => setUsuarioAutorizoId(e.target.value)}
              className="w-full rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
            >
              <option value="">Seleccionar...</option>
              {usuarios.map((u) => (
                <option key={u.id} value={u.id}>
                  {u.nombre}
                </option>
              ))}
            </select>
          </div>
        )}

        <div className="mt-5 flex justify-end gap-3">
          <button
            type="button"
            onClick={onCancelar}
            className="rounded border border-slate-300 px-4 py-2 text-sm hover:bg-slate-100 dark:border-slate-600 dark:hover:bg-slate-700"
          >
            Cancelar
          </button>
          <button
            type="button"
            disabled={!puedeGuardar}
            onClick={handleGuardar}
            className="rounded bg-[var(--color-acento)] px-4 py-2 text-sm text-white hover:brightness-90 disabled:opacity-50 dark:bg-[var(--color-acento)] dark:hover:brightness-110"
          >
            {guardando ? 'Guardando...' : 'Registrar venta'}
          </button>
        </div>
      </div>
    </div>
  )
}
