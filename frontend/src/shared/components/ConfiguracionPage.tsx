import { Building2, ClipboardList, Coins, Ruler, ShieldCheck, Users } from 'lucide-react'
import type { LucideIcon } from 'lucide-react'
import { Link } from 'react-router'

interface OpcionConfiguracion {
  to: string
  titulo: string
  descripcion: string
  icono: LucideIcon
}

const OPCIONES: OpcionConfiguracion[] = [
  { to: '/configuracion/empresa', titulo: 'Datos de la Empresa', descripcion: 'Razón social, RUC, logo, colores y mensaje de bienvenida.', icono: Building2 },
  { to: '/configuracion/usuarios', titulo: 'Usuarios', descripcion: 'Cuentas de acceso al sistema y sus roles asignados.', icono: Users },
  { to: '/configuracion/roles', titulo: 'Roles y Permisos', descripcion: 'Qué puede hacer cada rol en cada módulo.', icono: ShieldCheck },
  { to: '/configuracion/categorias', titulo: 'Categorías de Producto', descripcion: 'Organización del catálogo de productos.', icono: ClipboardList },
  { to: '/configuracion/medios-pago', titulo: 'Medios de Pago', descripcion: 'Formas de cobro disponibles en Ventas y Servicios de Campo.', icono: Coins },
  { to: '/configuracion/unidades-medida', titulo: 'Unidades de Medida', descripcion: 'Unidad de venta e inventario de cada producto.', icono: Ruler },
]

export function ConfiguracionPage() {
  return (
    <div>
      <h1 className="mb-4 text-xl font-semibold text-slate-800 dark:text-slate-100">Configuración</h1>
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {OPCIONES.map((opcion) => (
          <Link
            key={opcion.to}
            to={opcion.to}
            className="flex flex-col gap-2 rounded-lg border border-slate-200 bg-white p-4 shadow-sm transition-colors hover:border-[var(--color-principal)] dark:border-slate-700 dark:bg-slate-800"
          >
            <opcion.icono className="h-6 w-6 text-[var(--color-principal)]" />
            <p className="font-semibold text-slate-800 dark:text-slate-100">{opcion.titulo}</p>
            <p className="text-sm text-[var(--color-terciario)]">{opcion.descripcion}</p>
          </Link>
        ))}
      </div>
    </div>
  )
}
