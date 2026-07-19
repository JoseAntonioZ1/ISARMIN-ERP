import { Link } from 'react-router'

export function ConfiguracionPage() {
  return (
    <div>
      <h1 className="mb-4 text-xl font-semibold text-slate-800 dark:text-slate-100">Configuración</h1>
      <div className="flex flex-col gap-2">
        <Link to="/configuracion/usuarios" className="text-slate-700 underline hover:text-slate-900 dark:text-slate-300">
          Usuarios
        </Link>
        <Link to="/configuracion/roles" className="text-slate-700 underline hover:text-slate-900 dark:text-slate-300">
          Roles y Permisos
        </Link>
      </div>
    </div>
  )
}
