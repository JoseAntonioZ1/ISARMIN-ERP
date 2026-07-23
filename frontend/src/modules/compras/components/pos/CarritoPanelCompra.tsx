import { Minus, Plus, ShoppingCart, Trash2 } from 'lucide-react'
import { useState } from 'react'
import type { DetalleCompraInput } from '@/modules/compras/api/comprasApi'
import type { Proveedor } from '@/modules/proveedores/api/proveedoresApi'

export interface LineaCarritoCompra {
  productoId: string
  nombre: string
  costoUnitario: number
  cantidad: number
}

interface CarritoPanelCompraProps {
  lineas: LineaCarritoCompra[]
  onCambiarCantidad: (productoId: string, cantidad: number) => void
  onCambiarCosto: (productoId: string, costoUnitario: number) => void
  onQuitar: (productoId: string) => void
  proveedores: Proveedor[]
  onRegistrarCompra: (payload: {
    proveedorId: string
    fecha: string
    documentoCompraTipo: string
    documentoCompraNumero: string
    detalles: DetalleCompraInput[]
  }) => void
  guardando: boolean
}

const TIPOS_DOCUMENTO_COMPRA = ['Factura', 'Boleta', 'Guía de Remisión', 'Otro'] as const

export function CarritoPanelCompra({
  lineas,
  onCambiarCantidad,
  onCambiarCosto,
  onQuitar,
  proveedores,
  onRegistrarCompra,
  guardando,
}: CarritoPanelCompraProps) {
  const [proveedorId, setProveedorId] = useState('')
  const [fecha, setFecha] = useState(() => new Date().toISOString().slice(0, 10))
  const [documentoCompraTipo, setDocumentoCompraTipo] = useState<string>(TIPOS_DOCUMENTO_COMPRA[0])
  const [documentoCompraNumero, setDocumentoCompraNumero] = useState('')

  const total = lineas.reduce((suma, l) => suma + l.cantidad * l.costoUnitario, 0)

  const puedeRegistrar =
    lineas.length > 0 && !!proveedorId && !!fecha && !!documentoCompraTipo && !!documentoCompraNumero.trim() && !guardando

  const handleRegistrar = () => {
    const detalles: DetalleCompraInput[] = lineas.map((l) => ({
      productoId: l.productoId,
      cantidad: l.cantidad,
      costoUnitario: l.costoUnitario,
    }))

    onRegistrarCompra({
      proveedorId,
      fecha,
      documentoCompraTipo,
      documentoCompraNumero: documentoCompraNumero.trim(),
      detalles,
    })
  }

  return (
    <aside className="flex h-full min-h-0 w-full flex-col border-l border-slate-200 bg-white dark:border-slate-700 dark:bg-slate-800">
      <div className="space-y-2 border-b border-slate-200 p-3 dark:border-slate-700">
        <select
          value={proveedorId}
          onChange={(e) => setProveedorId(e.target.value)}
          className="w-full rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        >
          <option value="">Seleccionar proveedor...</option>
          {proveedores.map((p) => (
            <option key={p.id} value={p.id}>
              {p.nombreRazonSocial}
            </option>
          ))}
        </select>

        <div className="grid grid-cols-2 gap-2">
          <input
            type="date"
            value={fecha}
            onChange={(e) => setFecha(e.target.value)}
            className="w-full rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
          />
          <select
            value={documentoCompraTipo}
            onChange={(e) => setDocumentoCompraTipo(e.target.value)}
            className="w-full rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
          >
            {TIPOS_DOCUMENTO_COMPRA.map((tipo) => (
              <option key={tipo} value={tipo}>
                {tipo}
              </option>
            ))}
          </select>
        </div>

        <input
          value={documentoCompraNumero}
          onChange={(e) => setDocumentoCompraNumero(e.target.value)}
          placeholder="N° de documento del proveedor"
          className="w-full rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />
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
                    Costo
                    <input
                      type="number"
                      step="0.01"
                      min={0}
                      value={linea.costoUnitario}
                      onChange={(e) => onCambiarCosto(linea.productoId, Math.max(0, Number(e.target.value)))}
                      className="w-20 rounded border border-slate-300 px-1 py-0.5 text-right dark:border-slate-600 dark:bg-slate-900"
                    />
                  </div>
                  <p className="text-sm font-semibold text-slate-800 dark:text-slate-100">
                    S/ {(linea.cantidad * linea.costoUnitario).toFixed(2)}
                  </p>
                </div>
              </li>
            ))}
          </ul>
        )}
      </div>

      <div className="space-y-2 border-t border-slate-200 p-3 dark:border-slate-700">
        <div className="flex justify-between border-t border-slate-200 pt-1 text-lg font-bold text-slate-800 dark:border-slate-700 dark:text-slate-100">
          <span>Total</span>
          <span>S/ {total.toFixed(2)}</span>
        </div>

        <button
          type="button"
          disabled={!puedeRegistrar}
          onClick={handleRegistrar}
          className="w-full rounded bg-[var(--color-principal)] py-3 text-base font-semibold text-white hover:brightness-90 disabled:cursor-not-allowed disabled:opacity-40 dark:bg-[var(--color-principal)] dark:hover:brightness-110"
        >
          {guardando ? 'Registrando...' : 'Registrar Compra'}
        </button>
      </div>
    </aside>
  )
}
