import { Minus, Plus, ShoppingCart, Trash2 } from 'lucide-react'
import { useState } from 'react'
import type { Cliente } from '@/modules/clientes/api/clientesApi'
import type { MedioPago } from '@/modules/catalogos/api/catalogosApi'
import type { Usuario } from '@/modules/usuarios/api/usuariosApi'
import { TIPOS_COMPROBANTE, type DetalleVentaInput, type PagoVentaInput, type TipoComprobante } from '@/modules/ventas/api/ventasApi'
import { esMedioPagoEfectivo, iconoParaMedioPago } from '@/modules/ventas/utils/iconosPos'
import { useSessionStore } from '@/shared/hooks/useSessionStore'

export interface LineaCarritoPos {
  productoId: string
  nombre: string
  precioVenta: number
  descuentoPct: number
  cantidad: number
}

interface CarritoPanelPosProps {
  lineas: LineaCarritoPos[]
  onCambiarCantidad: (productoId: string, cantidad: number) => void
  onCambiarDescuento: (productoId: string, descuentoPct: number) => void
  onQuitar: (productoId: string) => void
  clientes: Cliente[]
  usuarios: Usuario[]
  mediosPago: MedioPago[]
  onFinalizarVenta: (payload: {
    clienteId: string | null
    tipoComprobante: TipoComprobante
    detalles: DetalleVentaInput[]
    pagos: PagoVentaInput[]
    usuarioAutorizoSaldoId: string | null
    lineas: LineaCarritoPos[]
  }) => void
  guardando: boolean
}

const TASA_IGV = 0.18

export function CarritoPanelPos({
  lineas,
  onCambiarCantidad,
  onCambiarDescuento,
  onQuitar,
  clientes,
  usuarios,
  mediosPago,
  onFinalizarVenta,
  guardando,
}: CarritoPanelPosProps) {
  const vendedorNombre = useSessionStore((s) => s.usuario?.nombre)

  const [clienteId, setClienteId] = useState('')
  const [tipoComprobante, setTipoComprobante] = useState<TipoComprobante>('Ticket')
  const [medioPagoId, setMedioPagoId] = useState('')
  const [montoPrincipal, setMontoPrincipal] = useState('')
  const [pagosAdicionales, setPagosAdicionales] = useState<{ medioPagoId: string; monto: string }[]>([])
  const [usuarioAutorizoId, setUsuarioAutorizoId] = useState('')

  const precioEfectivo = (linea: LineaCarritoPos) => linea.precioVenta * (1 - linea.descuentoPct / 100)

  const subtotal = lineas.reduce((suma, l) => suma + l.cantidad * l.precioVenta, 0)
  const total = lineas.reduce((suma, l) => suma + l.cantidad * precioEfectivo(l), 0)
  const descuento = subtotal - total
  const baseImponible = total / (1 + TASA_IGV)
  const igv = total - baseImponible

  const montoAdicionales = pagosAdicionales.reduce((suma, p) => suma + (Number(p.monto) || 0), 0)
  const totalPendienteParaPrincipal = Math.max(0, total - montoAdicionales)

  const medioSeleccionado = mediosPago.find((m) => m.id === medioPagoId)
  const esEfectivo = medioSeleccionado ? esMedioPagoEfectivo(medioSeleccionado.nombre) : false

  const montoPrincipalEfectivo = montoPrincipal === '' ? totalPendienteParaPrincipal : Number(montoPrincipal) || 0
  const montoAplicadoPrincipal = medioPagoId ? Math.min(montoPrincipalEfectivo, totalPendienteParaPrincipal) : 0
  const vuelto = esEfectivo && montoPrincipalEfectivo > totalPendienteParaPrincipal ? montoPrincipalEfectivo - totalPendienteParaPrincipal : 0

  const montoPagadoTotal = montoAplicadoPrincipal + montoAdicionales
  const saldoPendiente = total - montoPagadoTotal

  const esFactura = tipoComprobante === 'Factura'
  const clienteSeleccionado = clientes.find((c) => c.id === clienteId)
  const clienteTieneRuc = clienteSeleccionado?.tipoDocumento === 'Ruc'
  const clientesDisponibles = esFactura ? clientes.filter((c) => c.tipoDocumento === 'Ruc') : clientes

  const puedeFinalizar =
    lineas.length > 0 &&
    saldoPendiente >= -0.001 &&
    (saldoPendiente <= 0.001 || usuarioAutorizoId) &&
    (!esFactura || clienteTieneRuc) &&
    !guardando

  const handleFinalizar = () => {
    const detalles: DetalleVentaInput[] = lineas.map((l) => ({
      productoId: l.productoId,
      cantidad: l.cantidad,
      precioUnitario: precioEfectivo(l),
    }))

    const pagos: PagoVentaInput[] = []
    if (medioPagoId && montoAplicadoPrincipal > 0) {
      pagos.push({ medioPagoId, monto: montoAplicadoPrincipal })
    }
    for (const p of pagosAdicionales) {
      if (p.medioPagoId && Number(p.monto) > 0) {
        pagos.push({ medioPagoId: p.medioPagoId, monto: Number(p.monto) })
      }
    }

    onFinalizarVenta({
      clienteId: clienteId || null,
      tipoComprobante,
      detalles,
      pagos,
      usuarioAutorizoSaldoId: saldoPendiente > 0.001 ? usuarioAutorizoId : null,
      lineas,
    })
  }

  return (
    <aside className="flex h-full min-h-0 w-full flex-col border-l border-slate-200 bg-white dark:border-slate-700 dark:bg-slate-800">
      <div className="space-y-2 border-b border-slate-200 p-3 dark:border-slate-700">
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

        <select
          value={clienteId}
          onChange={(e) => setClienteId(e.target.value)}
          className="w-full rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        >
          <option value="">Sin cliente registrado</option>
          {clientesDisponibles.map((c) => (
            <option key={c.id} value={c.id}>
              {c.nombreRazonSocial}
            </option>
          ))}
        </select>
        {esFactura && !clienteTieneRuc && (
          <p className="text-xs text-red-600">Una Factura requiere seleccionar un cliente con RUC (RN-009).</p>
        )}

        <p className="text-xs text-[var(--color-terciario)]">Vendedor: {vendedorNombre ?? '—'}</p>
      </div>

      <div className="min-h-0 flex-1 overflow-y-auto p-3">
        {lineas.length === 0 ? (
          <div className="flex h-full flex-col items-center justify-center gap-2 text-center text-[var(--color-terciario)]">
            <ShoppingCart className="h-10 w-10" />
            <p className="text-sm">Agrega productos desde el buscador.</p>
          </div>
        ) : (
          <ul className="space-y-2">
            {lineas.map((linea) => (
              <li key={linea.productoId} className="rounded-lg border border-slate-200 p-2 dark:border-slate-700">
                <div className="flex items-start justify-between gap-2">
                  <p className="text-sm font-medium text-slate-800 dark:text-slate-100">{linea.nombre}</p>
                  <button type="button" onClick={() => onQuitar(linea.productoId)} className="text-red-600 hover:text-red-800">
                    <Trash2 className="h-4 w-4" />
                  </button>
                </div>
                <div className="mt-1 flex items-center justify-between gap-2">
                  <div className="flex items-center gap-1">
                    <button
                      type="button"
                      onClick={() => onCambiarCantidad(linea.productoId, Math.max(1, linea.cantidad - 1))}
                      className="rounded border border-slate-300 p-1 hover:bg-slate-100 dark:border-slate-600 dark:hover:bg-slate-700"
                    >
                      <Minus className="h-3 w-3" />
                    </button>
                    <span className="w-8 text-center text-sm">{linea.cantidad}</span>
                    <button
                      type="button"
                      onClick={() => onCambiarCantidad(linea.productoId, linea.cantidad + 1)}
                      className="rounded border border-slate-300 p-1 hover:bg-slate-100 dark:border-slate-600 dark:hover:bg-slate-700"
                    >
                      <Plus className="h-3 w-3" />
                    </button>
                  </div>
                  <div className="flex items-center gap-1 text-xs text-[var(--color-terciario)]">
                    Desc.
                    <input
                      type="number"
                      min={0}
                      max={100}
                      value={linea.descuentoPct}
                      onChange={(e) => onCambiarDescuento(linea.productoId, Math.min(100, Math.max(0, Number(e.target.value))))}
                      className="w-12 rounded border border-slate-300 px-1 py-0.5 text-right dark:border-slate-600 dark:bg-slate-900"
                    />
                    %
                  </div>
                  <p className="text-sm font-semibold text-slate-800 dark:text-slate-100">
                    S/ {(linea.cantidad * precioEfectivo(linea)).toFixed(2)}
                  </p>
                </div>
              </li>
            ))}
          </ul>
        )}
      </div>

      <div className="space-y-1 border-t border-slate-200 p-3 text-sm dark:border-slate-700">
        <div className="flex justify-between text-[var(--color-apoyo)] dark:text-slate-300">
          <span>Subtotal</span>
          <span>S/ {subtotal.toFixed(2)}</span>
        </div>
        {descuento > 0 && (
          <div className="flex justify-between text-[var(--color-secundario)]">
            <span>Descuento</span>
            <span>- S/ {descuento.toFixed(2)}</span>
          </div>
        )}
        <div className="flex justify-between text-xs text-[var(--color-terciario)]">
          <span>Base imponible</span>
          <span>S/ {baseImponible.toFixed(2)}</span>
        </div>
        <div className="flex justify-between text-xs text-[var(--color-terciario)]">
          <span>IGV (18%)</span>
          <span>S/ {igv.toFixed(2)}</span>
        </div>
        <div className="flex justify-between border-t border-slate-200 pt-1 text-lg font-bold text-slate-800 dark:border-slate-700 dark:text-slate-100">
          <span>Total</span>
          <span>S/ {total.toFixed(2)}</span>
        </div>
      </div>

      <div className="space-y-2 border-t border-slate-200 p-3 dark:border-slate-700">
        <div className="grid grid-cols-4 gap-2">
          {mediosPago.map((medio) => {
            const Icono = iconoParaMedioPago(medio.nombre)
            const seleccionado = medioPagoId === medio.id
            return (
              <button
                key={medio.id}
                type="button"
                onClick={() => setMedioPagoId(medio.id)}
                className={`flex flex-col items-center gap-1 rounded-lg border p-2 text-xs transition-colors ${
                  seleccionado
                    ? 'border-[var(--color-principal)] bg-[var(--color-principal)]/10 text-[var(--color-principal)]'
                    : 'border-slate-200 text-[var(--color-apoyo)] hover:bg-slate-50 dark:border-slate-600 dark:text-slate-300 dark:hover:bg-slate-700'
                }`}
              >
                <Icono className="h-5 w-5" />
                <span className="truncate">{medio.nombre}</span>
              </button>
            )
          })}
        </div>

        {esEfectivo && (
          <div>
            <label className="mb-1 block text-xs text-[var(--color-terciario)]">Monto recibido</label>
            <input
              type="number"
              step="0.01"
              value={montoPrincipal}
              onChange={(e) => setMontoPrincipal(e.target.value)}
              placeholder={totalPendienteParaPrincipal.toFixed(2)}
              className="w-full rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
            />
            {vuelto > 0 && (
              <p className="mt-1 text-sm font-medium text-emerald-600">Vuelto: S/ {vuelto.toFixed(2)}</p>
            )}
          </div>
        )}

        {pagosAdicionales.map((pago, index) => (
          <div key={index} className="flex items-center gap-2">
            <select
              value={pago.medioPagoId}
              onChange={(e) =>
                setPagosAdicionales(pagosAdicionales.map((p, i) => (i === index ? { ...p, medioPagoId: e.target.value } : p)))
              }
              className="flex-1 rounded border border-slate-300 px-2 py-1.5 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
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
              value={pago.monto}
              onChange={(e) => setPagosAdicionales(pagosAdicionales.map((p, i) => (i === index ? { ...p, monto: e.target.value } : p)))}
              placeholder="Monto"
              className="w-24 rounded border border-slate-300 px-2 py-1.5 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
            />
            <button
              type="button"
              onClick={() => setPagosAdicionales(pagosAdicionales.filter((_, i) => i !== index))}
              className="text-red-600 hover:text-red-800"
            >
              <Trash2 className="h-4 w-4" />
            </button>
          </div>
        ))}

        <button
          type="button"
          onClick={() => setPagosAdicionales([...pagosAdicionales, { medioPagoId: '', monto: '' }])}
          className="text-xs text-[var(--color-apoyo)] underline dark:text-slate-300"
        >
          + Agregar otro medio de pago
        </button>

        {saldoPendiente > 0.001 && (
          <div>
            <label className="mb-1 block text-xs text-[var(--color-terciario)]">
              Saldo pendiente S/ {saldoPendiente.toFixed(2)} — requiere autorización de Administrador/Propietario
            </label>
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

        <button
          type="button"
          disabled={!puedeFinalizar}
          onClick={handleFinalizar}
          className="w-full rounded bg-[var(--color-principal)] py-3 text-base font-semibold text-white hover:brightness-90 disabled:cursor-not-allowed disabled:opacity-40 dark:bg-[var(--color-principal)] dark:hover:brightness-110"
        >
          {guardando ? 'Procesando...' : 'Finalizar Venta'}
        </button>
      </div>
    </aside>
  )
}
