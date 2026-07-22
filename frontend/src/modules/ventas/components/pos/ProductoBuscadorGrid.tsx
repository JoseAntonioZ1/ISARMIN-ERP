import { useQuery } from '@tanstack/react-query'
import { Package, Search } from 'lucide-react'
import { useEffect, useRef, useState } from 'react'
import type { KeyboardEvent } from 'react'
import type { Producto } from '@/modules/productos/api/productosApi'
import { productosApi } from '@/modules/productos/api/productosApi'

interface ProductoBuscadorGridProps {
  categoriaId: string | null
  onAgregarProducto: (producto: Producto) => void
}

export function ProductoBuscadorGrid({ categoriaId, onAgregarProducto }: ProductoBuscadorGridProps) {
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

    // Un lector de código de barras "escribe" el código y remata con Enter — si el texto
    // coincide exactamente con un codigoBarras del catálogo, se agrega directo al carrito
    // en vez de solo filtrar la grilla.
    const coincidenciaExacta = productos?.datos.find((p) => p.codigoBarras === termino.trim())
    if (coincidenciaExacta) {
      handleAgregar(coincidenciaExacta)
    }
  }

  return (
    <section className="flex h-full w-full flex-col overflow-hidden">
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

      <div className="flex-1 overflow-y-auto p-3">
        {isLoading ? (
          <p className="text-[var(--color-terciario)]">Cargando...</p>
        ) : productos?.datos.length === 0 ? (
          <p className="text-[var(--color-terciario)]">Sin productos que coincidan con la búsqueda.</p>
        ) : (
          <div className="grid grid-cols-2 gap-3 sm:grid-cols-3 xl:grid-cols-4">
            {productos?.datos.map((producto) => (
              <ProductoCardPos key={producto.id} producto={producto} onAgregar={() => handleAgregar(producto)} />
            ))}
          </div>
        )}
      </div>
    </section>
  )
}

function ProductoCardPos({ producto, onAgregar }: { producto: Producto; onAgregar: () => void }) {
  const sinStock = producto.stockActual <= 0
  const stockBajo = producto.stockMinimo !== null && producto.stockActual <= producto.stockMinimo

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
        <p className="text-base font-semibold text-[var(--color-principal)]">S/ {producto.precioVenta.toFixed(2)}</p>
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
