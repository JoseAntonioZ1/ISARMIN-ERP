import { QueryClientProvider } from '@tanstack/react-query'
import { BrowserRouter, Route, Routes } from 'react-router'
import { CategoriasPage } from '@/modules/catalogos/pages/CategoriasPage'
import { MediosPagoPage } from '@/modules/catalogos/pages/MediosPagoPage'
import { ClientesPage } from '@/modules/clientes/pages/ClientesPage'
import { ProveedoresPage } from '@/modules/proveedores/pages/ProveedoresPage'
import { RolesPage } from '@/modules/roles/pages/RolesPage'
import { LoginPage } from '@/modules/usuarios/pages/LoginPage'
import { UsuariosPage } from '@/modules/usuarios/pages/UsuariosPage'
import { queryClient } from '@/shared/api/queryClient'
import { AppLayout } from '@/shared/components/AppLayout'
import { ConfiguracionPage } from '@/shared/components/ConfiguracionPage'
import { PaginaInicio } from '@/shared/components/PaginaInicio'
import { RutaProtegida } from '@/shared/components/RutaProtegida'

function App() {
  return (
    <QueryClientProvider client={queryClient}>
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
            <Route path="/configuracion" element={<ConfiguracionPage />} />
            <Route path="/configuracion/usuarios" element={<UsuariosPage />} />
            <Route path="/configuracion/roles" element={<RolesPage />} />
            <Route path="/configuracion/categorias" element={<CategoriasPage />} />
            <Route path="/configuracion/medios-pago" element={<MediosPagoPage />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </QueryClientProvider>
  )
}

export default App
