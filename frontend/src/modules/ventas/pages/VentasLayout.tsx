import { NavLink, Outlet } from 'react-router'

export function VentasLayout() {
  return (
    <div className="flex h-full flex-col gap-3">
      <div className="flex gap-1 border-b border-slate-200 dark:border-slate-700">
        <NavLink
          to="/ventas"
          end
          className={({ isActive }) =>
            `border-b-2 px-4 py-2 text-sm font-medium ${
              isActive
                ? 'border-[var(--color-principal)] text-[var(--color-principal)]'
                : 'border-transparent text-[var(--color-apoyo)] hover:text-slate-900 dark:text-slate-300'
            }`
          }
        >
          Punto de Venta
        </NavLink>
        <NavLink
          to="/ventas/historial"
          className={({ isActive }) =>
            `border-b-2 px-4 py-2 text-sm font-medium ${
              isActive
                ? 'border-[var(--color-principal)] text-[var(--color-principal)]'
                : 'border-transparent text-[var(--color-apoyo)] hover:text-slate-900 dark:text-slate-300'
            }`
          }
        >
          Historial
        </NavLink>
      </div>
      <div className="min-h-0 flex-1 overflow-hidden">
        <Outlet />
      </div>
    </div>
  )
}
