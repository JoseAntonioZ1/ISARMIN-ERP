interface ControlesPaginacionProps {
  pagina: number
  tamanoPagina: number
  total: number
  onCambiarPagina: (pagina: number) => void
}

/** Controles Anterior/Siguiente reutilizables — el backend de Clientes/Proveedores/Productos/Usuarios
 * ya devuelve `total`/`pagina`/`tamanoPagina` en cada listado paginado, pero ninguna pantalla los usaba. */
export function ControlesPaginacion({ pagina, tamanoPagina, total, onCambiarPagina }: ControlesPaginacionProps) {
  const totalPaginas = Math.max(1, Math.ceil(total / tamanoPagina))
  const desde = total === 0 ? 0 : (pagina - 1) * tamanoPagina + 1
  const hasta = Math.min(pagina * tamanoPagina, total)

  return (
    <div className="mt-3 flex items-center justify-between text-sm text-[var(--color-terciario)]">
      <span>
        Mostrando {desde}–{hasta} de {total}
      </span>
      <div className="flex items-center gap-2">
        <button
          type="button"
          disabled={pagina <= 1}
          onClick={() => onCambiarPagina(pagina - 1)}
          className="rounded border border-slate-300 px-3 py-1 hover:bg-slate-100 disabled:cursor-not-allowed disabled:opacity-40 dark:border-slate-600 dark:hover:bg-slate-700"
        >
          Anterior
        </button>
        <span>
          Página {pagina} de {totalPaginas}
        </span>
        <button
          type="button"
          disabled={pagina >= totalPaginas}
          onClick={() => onCambiarPagina(pagina + 1)}
          className="rounded border border-slate-300 px-3 py-1 hover:bg-slate-100 disabled:cursor-not-allowed disabled:opacity-40 dark:border-slate-600 dark:hover:bg-slate-700"
        >
          Siguiente
        </button>
      </div>
    </div>
  )
}
