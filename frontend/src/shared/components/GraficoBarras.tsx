export interface BarraDato {
  etiqueta: string
  valor: number
  /** Clase Tailwind de fondo (ej. "bg-emerald-500" o "bg-[var(--color-principal)]") — por defecto usa el color principal de marca. */
  claseColor?: string
}

/** Gráfico de barras horizontales simple, sin dependencias — deliberadamente no se agregó una
 * librería de gráficos (recharts, chart.js, etc.) para 5 reportes con datos simples de conteo/monto;
 * esto evita sumar peso al bundle y una API nueva que aprender para algo que no necesita zoom,
 * animaciones ni tooltips interactivos. */
export function GraficoBarras({
  datos,
  formatoValor = (v) => String(v),
}: {
  datos: BarraDato[]
  formatoValor?: (valor: number) => string
}) {
  const maximo = Math.max(1, ...datos.map((d) => d.valor))

  return (
    <div className="space-y-2">
      {datos.map((dato) => (
        <div key={dato.etiqueta} className="flex items-center gap-2">
          <span className="w-32 flex-shrink-0 truncate text-xs text-[var(--color-terciario)]">{dato.etiqueta}</span>
          <div className="h-4 flex-1 overflow-hidden rounded bg-slate-100 dark:bg-slate-700">
            <div
              className={`h-full rounded transition-all ${dato.claseColor ?? 'bg-[var(--color-principal)]'}`}
              style={{ width: `${(dato.valor / maximo) * 100}%` }}
            />
          </div>
          <span className="w-20 flex-shrink-0 text-right text-xs font-medium text-slate-700 dark:text-slate-200">
            {formatoValor(dato.valor)}
          </span>
        </div>
      ))}
      {datos.length === 0 && <p className="text-xs text-[var(--color-terciario)]">Sin datos para graficar.</p>}
    </div>
  )
}
