import { Link } from 'react-router'

export function ConfiguracionPage() {
  return (
    <div>
      <h1 className="mb-4 text-xl font-semibold text-slate-800 dark:text-slate-100">Configuración</h1>
      <div className="flex flex-col gap-2">
        <Link to="/configuracion/empresa" className="text-[var(--color-apoyo)] underline hover:text-slate-900 dark:text-slate-300">
          Datos de la Empresa
        </Link>
        <Link to="/configuracion/usuarios" className="text-[var(--color-apoyo)] underline hover:text-slate-900 dark:text-slate-300">
          Usuarios
        </Link>
        <Link to="/configuracion/roles" className="text-[var(--color-apoyo)] underline hover:text-slate-900 dark:text-slate-300">
          Roles y Permisos
        </Link>
        <Link to="/configuracion/categorias" className="text-[var(--color-apoyo)] underline hover:text-slate-900 dark:text-slate-300">
          Categorías de Producto
        </Link>
        <Link to="/configuracion/medios-pago" className="text-[var(--color-apoyo)] underline hover:text-slate-900 dark:text-slate-300">
          Medios de Pago
        </Link>
        <Link to="/configuracion/unidades-medida" className="text-[var(--color-apoyo)] underline hover:text-slate-900 dark:text-slate-300">
          Unidades de Medida
        </Link>
      </div>
    </div>
  )
}
