import { useQuery } from '@tanstack/react-query'
import { brandingApi } from '@/shared/api/brandingApi'

/** Público (sin sesión) — usado en el login y en el encabezado para mostrar razón social/logo. */
export function useBranding() {
  return useQuery({
    queryKey: ['branding'],
    queryFn: brandingApi.obtener,
    staleTime: 5 * 60 * 1000,
  })
}
