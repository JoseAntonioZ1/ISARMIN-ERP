import { httpClient } from '@/shared/api/httpClient'

export interface Branding {
  razonSocial: string
  ruc: string | null
  direccion: string | null
  logo: string | null
  colorAcento: string | null
  mensajeBienvenida: string | null
}

export const brandingApi = {
  obtener: () => httpClient.get<Branding>('/configuracion/branding'),
}
