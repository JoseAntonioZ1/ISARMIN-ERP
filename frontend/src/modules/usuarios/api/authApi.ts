import { httpClient } from '@/shared/api/httpClient'

export interface IniciarSesionRequest {
  nombreUsuario: string
  credencial: string
}

export interface SesionResponse {
  token: string
  expiraEn: string
  refreshToken: string
  refreshTokenExpiraEn: string
  usuario: { id: string; nombre: string }
  permisos: string[]
}

export const authApi = {
  login: (datos: IniciarSesionRequest) => httpClient.post<SesionResponse>('/auth/login', datos),
  refresh: (refreshToken: string) => httpClient.post<SesionResponse>('/auth/refresh', { refreshToken }),
  logout: (refreshToken: string | null) => httpClient.post<void>('/auth/logout', { refreshToken }),
}
