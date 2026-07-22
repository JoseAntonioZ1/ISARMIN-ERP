import { Search, Sparkles } from 'lucide-react'
import { useState } from 'react'
import type { Categoria } from '@/modules/catalogos/api/catalogosApi'
import { iconoParaCategoria } from '@/modules/ventas/utils/iconosPos'

interface CategoriaSidebarPosProps {
  categorias: Categoria[]
  categoriaSeleccionadaId: string | null
  onSeleccionar: (id: string | null) => void
}

export function CategoriaSidebarPos({ categorias, categoriaSeleccionadaId, onSeleccionar }: CategoriaSidebarPosProps) {
  const [busqueda, setBusqueda] = useState('')

  const categoriasFiltradas = categorias.filter((c) => c.nombre.toLowerCase().includes(busqueda.toLowerCase()))

  return (
    <aside className="flex h-full w-full flex-col border-r border-slate-200 bg-white dark:border-slate-700 dark:bg-slate-800">
      <div className="border-b border-slate-200 p-3 dark:border-slate-700">
        <div className="relative">
          <Search className="pointer-events-none absolute left-2.5 top-2.5 h-4 w-4 text-[var(--color-terciario)]" />
          <input
            value={busqueda}
            onChange={(e) => setBusqueda(e.target.value)}
            placeholder="Buscar categoría..."
            className="w-full rounded border border-slate-300 py-2 pl-8 pr-3 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
          />
        </div>
      </div>

      <div className="flex-1 space-y-1 overflow-y-auto p-3">
        <button
          type="button"
          onClick={() => onSeleccionar(null)}
          className={`flex w-full items-center gap-3 rounded-lg border px-3 py-2.5 text-left text-sm transition-colors ${
            categoriaSeleccionadaId === null
              ? 'border-[var(--color-principal)] bg-[var(--color-principal)]/10 font-medium text-[var(--color-principal)]'
              : 'border-transparent text-[var(--color-apoyo)] hover:border-slate-200 hover:bg-slate-50 dark:text-slate-300 dark:hover:border-slate-600 dark:hover:bg-slate-700'
          }`}
        >
          <Sparkles className="h-4 w-4 flex-shrink-0" />
          Todas
        </button>

        {categoriasFiltradas.map((categoria) => {
          const Icono = iconoParaCategoria(categoria.nombre)
          const seleccionada = categoriaSeleccionadaId === categoria.id
          return (
            <button
              key={categoria.id}
              type="button"
              onClick={() => onSeleccionar(categoria.id)}
              className={`flex w-full items-center gap-3 rounded-lg border px-3 py-2.5 text-left text-sm transition-colors ${
                seleccionada
                  ? 'border-[var(--color-principal)] bg-[var(--color-principal)]/10 font-medium text-[var(--color-principal)]'
                  : 'border-transparent text-[var(--color-apoyo)] hover:border-slate-200 hover:bg-slate-50 dark:text-slate-300 dark:hover:border-slate-600 dark:hover:bg-slate-700'
              }`}
            >
              <Icono className="h-4 w-4 flex-shrink-0" />
              <span className="truncate">{categoria.nombre}</span>
            </button>
          )
        })}

        {categoriasFiltradas.length === 0 && (
          <p className="px-3 py-2 text-xs text-[var(--color-terciario)]">Sin categorías que coincidan.</p>
        )}
      </div>
    </aside>
  )
}
