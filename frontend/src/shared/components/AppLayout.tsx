import {
  BarChart3,
  Home,
  LogOut,
  MapPin,
  Package,
  Receipt,
  Settings,
  ShoppingCart,
  Truck,
  Users,
  Wallet,
  Wrench,
} from 'lucide-react'
import { NavLink, Outlet, useNavigate } from 'react-router'
import { authApi } from '@/modules/usuarios/api/authApi'
import { useBranding } from '@/shared/hooks/useBranding'
import { useSessionStore } from '@/shared/hooks/useSessionStore'

interface ItemNav {
  to: string
  etiqueta: string
  icono: typeof Home
  fin?: boolean
}

interface GrupoNav {
  titulo: string
  items: ItemNav[]
}

const GRUPOS_NAV: GrupoNav[] = [
  { titulo: '', items: [{ to: '/', etiqueta: 'Inicio', icono: Home, fin: true }] },
  {
    titulo: 'Comercial',
    items: [
      { to: '/clientes', etiqueta: 'Clientes', icono: Users },
      { to: '/proveedores', etiqueta: 'Proveedores', icono: Truck },
      { to: '/productos', etiqueta: 'Productos', icono: Package },
      { to: '/compras', etiqueta: 'Compras', icono: ShoppingCart },
      { to: '/ventas', etiqueta: 'Ventas', icono: Receipt },
    ],
  },
  {
    titulo: 'Operaciones',
    items: [
      { to: '/caja', etiqueta: 'Caja', icono: Wallet },
      { to: '/taller', etiqueta: 'Taller', icono: Wrench },
      { to: '/servicios-campo', etiqueta: 'Servicios de Campo', icono: MapPin },
    ],
  },
  {
    titulo: 'Análisis',
    items: [{ to: '/reportes', etiqueta: 'Reportes', icono: BarChart3 }],
  },
  {
    titulo: 'Sistema',
    items: [{ to: '/configuracion', etiqueta: 'Configuración', icono: Settings }],
  },
]

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
    <div className="flex h-screen bg-[var(--color-fondo)] dark:bg-slate-900">
      <aside className="flex w-64 flex-shrink-0 flex-col border-r border-slate-200 bg-white dark:border-slate-700 dark:bg-slate-800">
        <div className="flex items-center gap-2 border-b border-slate-200 px-4 py-4 dark:border-slate-700">
          {branding?.logo && <img src={branding.logo} alt="Logo" className="h-8 w-8 flex-shrink-0 object-contain" />}
          <span className="truncate font-semibold text-slate-800 dark:text-slate-100">{branding?.razonSocial ?? 'ISARMIN ERP'}</span>
        </div>

        <nav className="flex-1 space-y-4 overflow-y-auto px-3 py-4">
          {GRUPOS_NAV.map((grupo) => (
            <div key={grupo.titulo || 'principal'}>
              {grupo.titulo && (
                <p className="mb-1 px-3 text-xs font-semibold uppercase tracking-wider text-slate-400 dark:text-slate-500">
                  {grupo.titulo}
                </p>
              )}
              <div className="space-y-0.5">
                {grupo.items.map((item) => (
                  <NavLink
                    key={item.to}
                    to={item.to}
                    end={item.fin}
                    className={({ isActive }) =>
                      `flex items-center gap-3 rounded px-3 py-2 text-sm transition-colors ${
                        isActive
                          ? 'bg-[var(--color-principal)] text-white'
                          : 'text-slate-600 hover:bg-slate-100 dark:text-slate-300 dark:hover:bg-slate-700'
                      }`
                    }
                  >
                    <item.icono className="h-4 w-4 flex-shrink-0" />
                    <span className="truncate">{item.etiqueta}</span>
                  </NavLink>
                ))}
              </div>
            </div>
          ))}
        </nav>

        <div className="border-t border-slate-200 px-3 py-3 dark:border-slate-700">
          <p className="truncate px-3 py-1 text-sm text-slate-600 dark:text-slate-300">{usuario?.nombre}</p>
          <button
            type="button"
            onClick={handleLogout}
            className="flex w-full items-center gap-3 rounded px-3 py-2 text-sm text-slate-600 hover:bg-slate-100 dark:text-slate-300 dark:hover:bg-slate-700"
          >
            <LogOut className="h-4 w-4 flex-shrink-0" />
            Cerrar sesión
          </button>
        </div>
      </aside>

      <main className="flex-1 overflow-y-auto p-6">
        <Outlet />
      </main>
    </div>
  )
}
