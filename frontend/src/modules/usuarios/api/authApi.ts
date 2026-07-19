import { httpClient } from '@/shared/api/httpClient'

export interface IniciarSesionRequest {
  nombreUsuario: string
  credencial: string
}

export interface SesionResponse {
  token: string
  expiraEn: string
  usuario: { id: string; nombre: string }
  permisos: string[]
}

export const authApi = {
  login: (datos: IniciarSesionRequest) => httpClient.post<SesionResponse>('/auth/login', datos),
  logout: () => httpClient.post<void>('/auth/logout'),
}
