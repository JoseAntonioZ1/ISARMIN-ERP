import { httpClient } from '@/shared/api/httpClient'

export interface DetalleCompra {
  id: string
  productoId: string
  cantidad: number
  costoUnitario: number
}

export interface Compra {
  id: string
  proveedorId: string
  fecha: string
  documentoCompraTipo: string
  documentoCompraNumero: string
  usuarioId: string
  total: number
  detalles: DetalleCompra[]
}

export interface ListadoPaginado<T> {
  datos: T[]
  total: number
  pagina: number
  tamanoPagina: number
}

export interface DetalleCompraInput {
  productoId: string
  cantidad: number
  costoUnitario: number
}

export interface DatosRegistrarCompra {
  proveedorId: string
  fecha: string
  documentoCompraTipo: string
  documentoCompraNumero: string
  detalles: DetalleCompraInput[]
}

export const comprasApi = {
  buscar: (proveedorId?: string, productoId?: string, pagina = 1, tamanoPagina = 20) => {
    const params = new URLSearchParams({ pagina: String(pagina), tamanoPagina: String(tamanoPagina) })
    if (proveedorId) params.set('proveedor', proveedorId)
    if (productoId) params.set('producto', productoId)
    return httpClient.get<ListadoPaginado<Compra>>(`/compras?${params.toString()}`)
  },
  obtener: (id: string) => httpClient.get<Compra>(`/compras/${id}`),
  registrar: (datos: DatosRegistrarCompra) => httpClient.post<Compra>('/compras', datos),
}
