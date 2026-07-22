import { httpClient } from '@/shared/api/httpClient'

export interface Producto {
  id: string
  codigoInterno: string
  codigoBarras: string | null
  nombre: string
  categoriaId: string
  marca: string | null
  unidadMedidaId: string
  costoReferencia: number
  precioVenta: number
  margen: number
  stockActual: number
  stockMinimo: number | null
  estado: 'Activo' | 'Inactivo'
  imagen: string | null
}

export interface ListadoPaginado<T> {
  datos: T[]
  total: number
  pagina: number
  tamanoPagina: number
}

export interface DatosRegistrarProducto {
  codigoInterno: string
  nombre: string
  categoriaId: string
  unidadMedidaId: string
  costoReferencia: number
  precioVenta: number
  stockInicial: number
  marca: string | null
  codigoBarras: string | null
  stockMinimo: number | null
  imagen: string | null
}

export interface DatosEditarProducto {
  codigoInterno: string
  nombre: string
  categoriaId: string
  unidadMedidaId: string
  costoReferencia: number
  precioVenta: number
  marca: string | null
  codigoBarras: string | null
  stockMinimo: number | null
  imagen: string | null
}

export interface MovimientoInventario {
  id: string
  productoId: string
  tipoMovimiento: string
  cantidad: number
  origenTipo: string | null
  origenId: string | null
  motivoAjuste: string | null
  usuarioId: string
  fecha: string
}

export const productosApi = {
  buscar: (busqueda?: string, pagina = 1, tamanoPagina = 20, categoriaId?: string) => {
    const params = new URLSearchParams({
      busqueda: busqueda ?? '',
      pagina: String(pagina),
      tamanoPagina: String(tamanoPagina),
    })
    if (categoriaId) params.set('categoria', categoriaId)
    return httpClient.get<ListadoPaginado<Producto>>(`/productos?${params.toString()}`)
  },
  registrar: (datos: DatosRegistrarProducto) => httpClient.post<Producto>('/productos', datos),
  editar: (id: string, datos: DatosEditarProducto) => httpClient.put<Producto>(`/productos/${id}`, datos),
  cambiarEstado: (id: string, activo: boolean) => httpClient.patch<void>(`/productos/${id}/estado`, { activo }),
  ajustarInventario: (id: string, cantidadAjuste: number, motivo: string) =>
    httpClient.post<Producto>(`/productos/${id}/ajustes`, { cantidadAjuste, motivo }),
  consultarKardex: (id: string) => httpClient.get<MovimientoInventario[]>(`/productos/${id}/movimientos`),
}
