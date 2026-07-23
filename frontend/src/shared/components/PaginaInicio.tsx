import { useQuery } from '@tanstack/react-query'
import { AlertTriangle, DollarSign, MapPin, ShoppingCart, Wallet, Wrench } from 'lucide-react'
import type { LucideIcon } from 'lucide-react'
import { Link } from 'react-router'
import { cajaApi } from '@/modules/caja/api/cajaApi'
import { reportesApi } from '@/modules/reportes/api/reportesApi'
import { serviciosCampoApi } from '@/modules/serviciosCampo/api/serviciosCampoApi'
import { ordenesTrabajoApi } from '@/modules/taller/api/ordenesTrabajoApi'
import { LOGO_PREDETERMINADO } from '@/shared/constants/branding'
import { useBranding } from '@/shared/hooks/useBranding'

const ESTADOS_OT_CERRADOS = ['Entregado', 'Rechazado']

export function PaginaInicio() {
  const { data: branding } = useBranding()
  const hoy = new Date().toISOString().slice(0, 10)

  // Cada tarjeta se maneja de forma independiente y silenciosa ante error (ej. 403 por permisos):
  // un rol sin acceso a Reportes/Caja/Taller simplemente no ve esa tarjeta, sin ensuciar la pantalla
  // de inicio con errores de autorización que no son accionables para ese usuario.
  const { data: reporteVentas } = useQuery({
    queryKey: ['dashboard', 'ventas-hoy'],
    queryFn: () => reportesApi.ventas(hoy, hoy),
    retry: false,
  })

  const { data: caja } = useQuery({ queryKey: ['dashboard', 'caja'], queryFn: cajaApi.obtenerActual, retry: false })

  const { data: reporteInventario } = useQuery({
    queryKey: ['dashboard', 'inventario'],
    queryFn: reportesApi.inventario,
    retry: false,
  })

  const { data: listadoOt } = useQuery({
    queryKey: ['dashboard', 'ordenes-trabajo'],
    queryFn: () => ordenesTrabajoApi.buscar(undefined, undefined, 1, 200),
    retry: false,
  })

  const { data: listadoServiciosCampo } = useQuery({
    queryKey: ['dashboard', 'servicios-campo'],
    queryFn: () => serviciosCampoApi.buscar(undefined, undefined, 1, 200),
    retry: false,
  })

  const otPendientes = listadoOt?.datos.filter((ot) => !ESTADOS_OT_CERRADOS.includes(ot.estado)).length
  const serviciosPendientes = listadoServiciosCampo?.datos.filter((s) => s.estado === 'Solicitado').length

  return (
    <div>
      <div className="mb-6 flex flex-col items-center gap-2 text-center">
        <img src={branding?.logo ?? LOGO_PREDETERMINADO} alt="Logo" className="h-16 w-16 object-contain" />
        <h1 className="text-xl font-semibold text-slate-800 dark:text-slate-100">{branding?.razonSocial ?? 'ISARMIN ERP'}</h1>
        <p className="text-sm text-[var(--color-terciario)] dark:text-slate-400">
          {branding?.mensajeBienvenida ?? 'Entorno base listo. Los módulos se incorporarán de forma incremental.'}
        </p>
      </div>

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {reporteVentas && (
          <TarjetaKpi
            icono={ShoppingCart}
            titulo="Ventas de hoy"
            valor={`S/ ${reporteVentas.montoTotal.toFixed(2)}`}
            detalle={`${reporteVentas.cantidadVentas} venta${reporteVentas.cantidadVentas === 1 ? '' : 's'}`}
            enlace="/ventas/historial"
          />
        )}

        {caja && (
          <TarjetaKpi
            icono={Wallet}
            titulo="Caja"
            valor={caja.estado}
            detalle={caja.estado === 'Abierta' ? `Apertura: S/ ${caja.montoApertura.toFixed(2)}` : 'Sin caja abierta'}
            enlace="/caja"
            resaltado={caja.estado === 'Abierta'}
          />
        )}

        {reporteInventario && (
          <TarjetaKpi
            icono={AlertTriangle}
            titulo="Productos en quiebre"
            valor={String(reporteInventario.productosEnQuiebre.length)}
            detalle="Stock en o bajo el mínimo"
            enlace="/productos"
            resaltado={reporteInventario.productosEnQuiebre.length > 0}
          />
        )}

        {otPendientes !== undefined && (
          <TarjetaKpi
            icono={Wrench}
            titulo="Órdenes de Trabajo activas"
            valor={String(otPendientes)}
            detalle="En Taller, sin entregar"
            enlace="/taller"
          />
        )}

        {serviciosPendientes !== undefined && (
          <TarjetaKpi
            icono={MapPin}
            titulo="Servicios de Campo pendientes"
            valor={String(serviciosPendientes)}
            detalle="Solicitados, sin cerrar"
            enlace="/servicios-campo"
          />
        )}

        {reporteVentas && (
          <TarjetaKpi
            icono={DollarSign}
            titulo="Ticket promedio de hoy"
            valor={`S/ ${(reporteVentas.cantidadVentas > 0 ? reporteVentas.montoTotal / reporteVentas.cantidadVentas : 0).toFixed(2)}`}
            detalle="Por venta"
            enlace="/ventas/historial"
          />
        )}
      </div>
    </div>
  )
}

function TarjetaKpi({
  icono: Icono,
  titulo,
  valor,
  detalle,
  enlace,
  resaltado,
}: {
  icono: LucideIcon
  titulo: string
  valor: string
  detalle: string
  enlace: string
  resaltado?: boolean
}) {
  return (
    <Link
      to={enlace}
      className={`flex flex-col gap-2 rounded-lg border bg-white p-4 shadow-sm transition-colors hover:border-[var(--color-principal)] dark:bg-slate-800 ${
        resaltado ? 'border-[var(--color-secundario)]' : 'border-slate-200 dark:border-slate-700'
      }`}
    >
      <div className="flex items-center gap-2 text-[var(--color-terciario)]">
        <Icono className="h-4 w-4" />
        <span className="text-xs font-medium uppercase tracking-wide">{titulo}</span>
      </div>
      <p className="text-2xl font-bold text-slate-800 dark:text-slate-100">{valor}</p>
      <p className="text-xs text-[var(--color-terciario)]">{detalle}</p>
    </Link>
  )
}
