import { useQuery } from '@tanstack/react-query'
import { Package, Search } from 'lucide-react'
import { useEffect, useRef, useState } from 'react'
import type { KeyboardEvent } from 'react'
import type { Producto } from '@/modules/productos/api/productosApi'
import { productosApi } from '@/modules/productos/api/productosApi'
import { EstadoCarga } from '@/shared/components/EstadoCarga'

interface ProductoBuscadorGridProps {
  categoriaId: string | null
  onAgregarProducto: (producto: Producto) => void
  /** Qué precio mostrar en la tarjeta — venta usa precioVenta, compra usa costoReferencia. */
  obtenerPrecio?: (producto: Producto) => number
  /** En Ventas no se puede vender sin stock; en Compras el stock bajo/cero es justo el motivo para
   * comprar, así que no debe bloquear ni marcarse como error. */
  validarStock?: boolean
}

/** Panel central compartido por las pantallas tipo POS (Ventas y Compras) — buscador + grilla de
 * productos. Maneja lector de código de barras: al detectar Enter, si el texto coincide exactamente
 * con un codigoBarras del resultado actual, agrega el producto directo en vez de solo filtrar. */
export function ProductoBuscadorGrid({
  categoriaId,
  onAgregarProducto,
  obtenerPrecio = (p) => p.precioVenta,
  validarStock = true,
}: ProductoBuscadorGridProps) {
  const [termino, setTermino] = useState('')
  const [terminoDebounced, setTerminoDebounced] = useState('')
  const inputRef = useRef<HTMLInputElement>(null)

  useEffect(() => {
    const temporizador = setTimeout(() => setTerminoDebounced(termino), 250)
    return () => clearTimeout(temporizador)
  }, [termino])

  const { data: productos, isLoading } = useQuery({
    queryKey: ['productos', 'pos', terminoDebounced, categoriaId],
    queryFn: () => productosApi.buscar(terminoDebounced, 1, 60, categoriaId ?? undefined),
    placeholderData: (anterior) => anterior,
  })

  const handleAgregar = (producto: Producto) => {
    onAgregarProducto(producto)
    setTermino('')
    inputRef.current?.focus()
  }

  const handleKeyDown = (e: KeyboardEvent<HTMLInputElement>) => {
    if (e.key !== 'Enter' || !termino.trim()) return

    const coincidenciaExacta = productos?.datos.find((p) => p.codigoBarras === termino.trim())
    if (coincidenciaExacta) {
      handleAgregar(coincidenciaExacta)
    }
  }

  return (
    <section className="flex h-full min-h-0 w-full flex-col overflow-hidden">
      <div className="border-b border-slate-200 bg-white p-3 dark:border-slate-700 dark:bg-slate-800">
        <div className="relative">
          <Search className="pointer-events-none absolute left-3 top-3.5 h-5 w-5 text-[var(--color-terciario)]" />
          <input
            ref={inputRef}
            autoFocus
            value={termino}
            onChange={(e) => setTermino(e.target.value)}
            onKeyDown={handleKeyDown}
            placeholder="Buscar por nombre, código o código de barras..."
            className="w-full rounded-lg border border-slate-300 py-3 pl-10 pr-3 text-base dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
          />
        </div>
      </div>

      <div className="min-h-0 flex-1 overflow-y-auto p-3">
        {isLoading ? (
          <EstadoCarga />
        ) : productos?.datos.length === 0 ? (
          <p className="text-[var(--color-terciario)]">Sin productos que coincidan con la búsqueda.</p>
        ) : (
          <div className="grid grid-cols-2 gap-3 sm:grid-cols-3 xl:grid-cols-4">
            {productos?.datos.map((producto) => (
              <ProductoCardPos
                key={producto.id}
                producto={producto}
                precio={obtenerPrecio(producto)}
                validarStock={validarStock}
                onAgregar={() => handleAgregar(producto)}
              />
            ))}
          </div>
        )}
      </div>
    </section>
  )
}

function ProductoCardPos({
  producto,
  precio,
  validarStock,
  onAgregar,
}: {
  producto: Producto
  precio: number
  validarStock: boolean
  onAgregar: () => void
}) {
  const sinStock = validarStock && producto.stockActual <= 0
  const stockBajo = validarStock && producto.stockMinimo !== null && producto.stockActual <= producto.stockMinimo

  return (
    <div className="flex flex-col overflow-hidden rounded-lg border border-slate-200 bg-white shadow-sm dark:border-slate-700 dark:bg-slate-800">
      <div className="flex h-24 items-center justify-center bg-slate-100 dark:bg-slate-900">
        {producto.imagen ? (
          <img src={producto.imagen} alt={producto.nombre} className="h-full w-full object-contain" />
        ) : (
          <Package className="h-10 w-10 text-[var(--color-terciario)]" />
        )}
      </div>
      <div className="flex flex-1 flex-col gap-1 p-2">
        <p className="line-clamp-2 text-sm font-medium text-slate-800 dark:text-slate-100">{producto.nombre}</p>
        <p className="text-base font-semibold text-[var(--color-principal)]">S/ {precio.toFixed(2)}</p>
        <p className={`text-xs ${stockBajo ? 'text-red-600' : 'text-[var(--color-terciario)]'}`}>
          Stock: {producto.stockActual}
        </p>
        <button
          type="button"
          disabled={sinStock}
          onClick={onAgregar}
          className="mt-1 rounded bg-[var(--color-principal)] px-2 py-1.5 text-xs font-medium text-white hover:brightness-90 disabled:cursor-not-allowed disabled:opacity-40 dark:bg-[var(--color-principal)] dark:hover:brightness-110"
        >
          {sinStock ? 'Sin stock' : 'Agregar'}
        </button>
      </div>
    </div>
  )
}
