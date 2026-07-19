import { QueryClientProvider } from '@tanstack/react-query'
import { BrowserRouter, Route, Routes } from 'react-router'
import { queryClient } from '@/shared/api/queryClient'
import { PaginaInicio } from '@/shared/components/PaginaInicio'

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<PaginaInicio />} />
        </Routes>
      </BrowserRouter>
    </QueryClientProvider>
  )
}

export default App
