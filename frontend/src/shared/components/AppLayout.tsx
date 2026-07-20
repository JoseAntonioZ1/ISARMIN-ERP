import { Link, Outlet, useNavigate } from 'react-router'
import { authApi } from '@/modules/usuarios/api/authApi'
import { useBranding } from '@/shared/hooks/useBranding'
import { useSessionStore } from '@/shared/hooks/useSessionStore'

export function AppLayout() {
  const navigate = useNavigate()
  const usuario = useSessionStore((s) => s.usuario)
  const cerrarSesion = useSessionStore((s) => s.cerrarSesion)
  const { data: branding } = useBranding()

  const handleLogout = async () => {
    try {
      await authApi.logout()
    } finally {
      cerrarSesion()
      navigate('/login', { replace: true })
    }
  }

  return (
    <div className="min-h-screen bg-slate-50 dark:bg-slate-900">
      <header className="flex items-center justify-between border-b border-slate-200 bg-white px-6 py-3 dark:border-slate-700 dark:bg-slate-800">
        <div className="flex items-center gap-6">
          <span className="flex items-center gap-2 font-semibold text-slate-800 dark:text-slate-100">
            {branding?.logo && <img src={branding.logo} alt="Logo" className="h-7 w-7 object-contain" />}
            {branding?.razonSocial ?? 'ISARMIN ERP'}
          </span>
          <nav className="flex gap-4 text-sm">
            <Link to="/" className="text-slate-600 hover:text-slate-900 dark:text-slate-300">
              Inicio
            </Link>
            <Link to="/clientes" className="text-slate-600 hover:text-slate-900 dark:text-slate-300">
              Clientes
            </Link>
            <Link to="/proveedores" className="text-slate-600 hover:text-slate-900 dark:text-slate-300">
              Proveedores
            </Link>
            <Link to="/productos" className="text-slate-600 hover:text-slate-900 dark:text-slate-300">
              Productos
            </Link>
            <Link to="/compras" className="text-slate-600 hover:text-slate-900 dark:text-slate-300">
              Compras
            </Link>
            <Link to="/caja" className="text-slate-600 hover:text-slate-900 dark:text-slate-300">
              Caja
            </Link>
            <Link to="/taller" className="text-slate-600 hover:text-slate-900 dark:text-slate-300">
              Taller
            </Link>
            <Link to="/servicios-campo" className="text-slate-600 hover:text-slate-900 dark:text-slate-300">
              Servicios de Campo
            </Link>
            <Link to="/ventas" className="text-slate-600 hover:text-slate-900 dark:text-slate-300">
              Ventas
            </Link>
            <Link to="/reportes" className="text-slate-600 hover:text-slate-900 dark:text-slate-300">
              Reportes
            </Link>
            <Link to="/configuracion" className="text-slate-600 hover:text-slate-900 dark:text-slate-300">
              Configuración
            </Link>
          </nav>
        </div>
        <div className="flex items-center gap-4">
          <span className="text-sm text-slate-600 dark:text-slate-300">{usuario?.nombre}</span>
          <button
            type="button"
            onClick={handleLogout}
            className="rounded border border-slate-300 px-3 py-1 text-sm hover:bg-slate-100 dark:border-slate-600 dark:hover:bg-slate-700"
          >
            Cerrar sesión
          </button>
        </div>
      </header>
      <main className="p-6">
        <Outlet />
      </main>
    </div>
  )
}
