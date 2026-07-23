interface EstadoBadgeProps {
  estado: 'Activo' | 'Inactivo'
}

/** Pastilla de color para Activo/Inactivo — antes se mostraba como texto plano sin diferenciación
 * visual en Clientes, Proveedores, Productos, Usuarios y Medios de Pago. */
export function EstadoBadge({ estado }: EstadoBadgeProps) {
  const activo = estado === 'Activo'
  return (
    <span
      className={`inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium ${
        activo
          ? 'bg-emerald-50 text-emerald-700 dark:bg-emerald-950/40 dark:text-emerald-400'
          : 'bg-slate-100 text-slate-500 dark:bg-slate-700 dark:text-slate-400'
      }`}
    >
      {estado}
    </span>
  )
}
