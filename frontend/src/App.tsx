import { QueryClientProvider } from '@tanstack/react-query'
import { BrowserRouter, Route, Routes } from 'react-router'
import { LoginPage } from '@/modules/usuarios/pages/LoginPage'
import { UsuariosPage } from '@/modules/usuarios/pages/UsuariosPage'
import { queryClient } from '@/shared/api/queryClient'
import { AppLayout } from '@/shared/components/AppLayout'
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
            <Route path="/configuracion/usuarios" element={<UsuariosPage />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </QueryClientProvider>
  )
}

export default App
