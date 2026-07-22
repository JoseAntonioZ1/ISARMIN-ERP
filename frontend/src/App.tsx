import { QueryClientProvider } from '@tanstack/react-query'
import { BrowserRouter, Route, Routes } from 'react-router'
import { CategoriasPage } from '@/modules/catalogos/pages/CategoriasPage'
import { ConfiguracionEmpresaPage } from '@/modules/catalogos/pages/ConfiguracionEmpresaPage'
import { MediosPagoPage } from '@/modules/catalogos/pages/MediosPagoPage'
import { UnidadesMedidaPage } from '@/modules/catalogos/pages/UnidadesMedidaPage'
import { CajaPage } from '@/modules/caja/pages/CajaPage'
import { ClientesPage } from '@/modules/clientes/pages/ClientesPage'
import { ComprasPage } from '@/modules/compras/pages/ComprasPage'
import { ProductosPage } from '@/modules/productos/pages/ProductosPage'
import { ProveedoresPage } from '@/modules/proveedores/pages/ProveedoresPage'
import { RolesPage } from '@/modules/roles/pages/RolesPage'
import { ReportesPage } from '@/modules/reportes/pages/ReportesPage'
import { ServiciosCampoPage } from '@/modules/serviciosCampo/pages/ServiciosCampoPage'
import { TallerPage } from '@/modules/taller/pages/TallerPage'
import { VentaPosPage } from '@/modules/ventas/pages/VentaPosPage'
import { VentasHistorialPage } from '@/modules/ventas/pages/VentasHistorialPage'
import { VentasLayout } from '@/modules/ventas/pages/VentasLayout'
import { LoginPage } from '@/modules/usuarios/pages/LoginPage'
import { UsuariosPage } from '@/modules/usuarios/pages/UsuariosPage'
import { queryClient } from '@/shared/api/queryClient'
import { AplicarBranding } from '@/shared/components/AplicarBranding'
import { AppLayout } from '@/shared/components/AppLayout'
import { ConfiguracionPage } from '@/shared/components/ConfiguracionPage'
import { PaginaInicio } from '@/shared/components/PaginaInicio'
import { RutaProtegida } from '@/shared/components/RutaProtegida'

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <AplicarBranding />
      <BrowserRouter>
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
            <Route path="/compras" element={<ComprasPage />} />
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
      </BrowserRouter>
    </QueryClientProvider>
  )
}

export default App
