import { QueryClientProvider } from '@tanstack/react-query'
import { lazy, Suspense } from 'react'
import { BrowserRouter, Route, Routes } from 'react-router'
import { queryClient } from '@/shared/api/queryClient'
import { AplicarBranding } from '@/shared/components/AplicarBranding'
import { AppLayout } from '@/shared/components/AppLayout'
import { EstadoCarga } from '@/shared/components/EstadoCarga'
import { RutaProtegida } from '@/shared/components/RutaProtegida'
import { ToastProvider } from '@/shared/hooks/useToast'

const CategoriasPage = lazy(() => import('@/modules/catalogos/pages/CategoriasPage').then((m) => ({ default: m.CategoriasPage })))
const ConfiguracionEmpresaPage = lazy(() =>
  import('@/modules/catalogos/pages/ConfiguracionEmpresaPage').then((m) => ({ default: m.ConfiguracionEmpresaPage })),
)
const MediosPagoPage = lazy(() => import('@/modules/catalogos/pages/MediosPagoPage').then((m) => ({ default: m.MediosPagoPage })))
const UnidadesMedidaPage = lazy(() =>
  import('@/modules/catalogos/pages/UnidadesMedidaPage').then((m) => ({ default: m.UnidadesMedidaPage })),
)
const CajaPage = lazy(() => import('@/modules/caja/pages/CajaPage').then((m) => ({ default: m.CajaPage })))
const ClientesPage = lazy(() => import('@/modules/clientes/pages/ClientesPage').then((m) => ({ default: m.ClientesPage })))
const ComprasHistorialPage = lazy(() =>
  import('@/modules/compras/pages/ComprasHistorialPage').then((m) => ({ default: m.ComprasHistorialPage })),
)
const ComprasLayout = lazy(() => import('@/modules/compras/pages/ComprasLayout').then((m) => ({ default: m.ComprasLayout })))
const CompraPosPage = lazy(() => import('@/modules/compras/pages/CompraPosPage').then((m) => ({ default: m.CompraPosPage })))
const ProductosPage = lazy(() => import('@/modules/productos/pages/ProductosPage').then((m) => ({ default: m.ProductosPage })))
const ProveedoresPage = lazy(() =>
  import('@/modules/proveedores/pages/ProveedoresPage').then((m) => ({ default: m.ProveedoresPage })),
)
const RolesPage = lazy(() => import('@/modules/roles/pages/RolesPage').then((m) => ({ default: m.RolesPage })))
const ReportesPage = lazy(() => import('@/modules/reportes/pages/ReportesPage').then((m) => ({ default: m.ReportesPage })))
const ServiciosCampoPage = lazy(() =>
  import('@/modules/serviciosCampo/pages/ServiciosCampoPage').then((m) => ({ default: m.ServiciosCampoPage })),
)
const TallerPage = lazy(() => import('@/modules/taller/pages/TallerPage').then((m) => ({ default: m.TallerPage })))
const VentaPosPage = lazy(() => import('@/modules/ventas/pages/VentaPosPage').then((m) => ({ default: m.VentaPosPage })))
const VentasHistorialPage = lazy(() =>
  import('@/modules/ventas/pages/VentasHistorialPage').then((m) => ({ default: m.VentasHistorialPage })),
)
const VentasLayout = lazy(() => import('@/modules/ventas/pages/VentasLayout').then((m) => ({ default: m.VentasLayout })))
const LoginPage = lazy(() => import('@/modules/usuarios/pages/LoginPage').then((m) => ({ default: m.LoginPage })))
const UsuariosPage = lazy(() => import('@/modules/usuarios/pages/UsuariosPage').then((m) => ({ default: m.UsuariosPage })))
const ConfiguracionPage = lazy(() =>
  import('@/shared/components/ConfiguracionPage').then((m) => ({ default: m.ConfiguracionPage })),
)
const PaginaInicio = lazy(() => import('@/shared/components/PaginaInicio').then((m) => ({ default: m.PaginaInicio })))

function CargandoPagina() {
  return (
    <div className="flex min-h-screen items-center justify-center">
      <EstadoCarga mensaje="Cargando..." />
    </div>
  )
}

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <AplicarBranding />
      <ToastProvider>
        <BrowserRouter>
          <Suspense fallback={<CargandoPagina />}>
            <Routes>
              <Route path="/login" element={<LoginPage />} />
              <Route
                element={
                  <RutaProtegida>
                    <AppLayout />
                  </RutaProtegida>
                }
              >
                <Route path="/" element={<PaginaInicio />} />
                <Route path="/clientes" element={<ClientesPage />} />
                <Route path="/proveedores" element={<ProveedoresPage />} />
                <Route path="/productos" element={<ProductosPage />} />
                <Route path="/compras" element={<ComprasLayout />}>
                  <Route index element={<CompraPosPage />} />
                  <Route path="historial" element={<ComprasHistorialPage />} />
                </Route>
                <Route path="/caja" element={<CajaPage />} />
                <Route path="/taller" element={<TallerPage />} />
                <Route path="/servicios-campo" element={<ServiciosCampoPage />} />
                <Route path="/ventas" element={<VentasLayout />}>
                  <Route index element={<VentaPosPage />} />
                  <Route path="historial" element={<VentasHistorialPage />} />
                </Route>
                <Route path="/reportes" element={<ReportesPage />} />
                <Route path="/configuracion" element={<ConfiguracionPage />} />
                <Route path="/configuracion/empresa" element={<ConfiguracionEmpresaPage />} />
                <Route path="/configuracion/usuarios" element={<UsuariosPage />} />
                <Route path="/configuracion/roles" element={<RolesPage />} />
                <Route path="/configuracion/categorias" element={<CategoriasPage />} />
                <Route path="/configuracion/medios-pago" element={<MediosPagoPage />} />
                <Route path="/configuracion/unidades-medida" element={<UnidadesMedidaPage />} />
              </Route>
            </Routes>
          </Suspense>
        </BrowserRouter>
      </ToastProvider>
    </QueryClientProvider>
  )
}

export default App
