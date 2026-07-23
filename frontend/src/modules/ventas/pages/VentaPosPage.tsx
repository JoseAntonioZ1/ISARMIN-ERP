import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useState } from 'react'
import { categoriasApi, mediosPagoApi } from '@/modules/catalogos/api/catalogosApi'
import { clientesApi } from '@/modules/clientes/api/clientesApi'
import type { Producto } from '@/modules/productos/api/productosApi'
import { usuariosApi } from '@/modules/usuarios/api/usuariosApi'
import { CarritoPanelPos, type LineaCarritoPos } from '@/modules/ventas/components/pos/CarritoPanelPos'
import { type DetalleVentaInput, type PagoVentaInput, type TipoComprobante, ventasApi } from '@/modules/ventas/api/ventasApi'
import { generarComprobantePdf } from '@/modules/ventas/utils/comprobantePdf'
import { ApiError } from '@/shared/api/httpClient'
import { CategoriaSidebarPos } from '@/shared/components/pos/CategoriaSidebarPos'
import { ProductoBuscadorGrid } from '@/shared/components/pos/ProductoBuscadorGrid'
import { useBranding } from '@/shared/hooks/useBranding'

export function VentaPosPage() {
  const queryClient = useQueryClient()
  const [categoriaId, setCategoriaId] = useState<string | null>(null)
  const [lineas, setLineas] = useState<LineaCarritoPos[]>([])
  const [error, setError] = useState<string | null>(null)
  const [exito, setExito] = useState(false)

  const { data: categorias } = useQuery({ queryKey: ['categorias'], queryFn: categoriasApi.listar })
  const { data: clientes } = useQuery({ queryKey: ['clientes', 'todos'], queryFn: () => clientesApi.buscar(undefined, 1, 200) })
  const { data: usuarios } = useQuery({ queryKey: ['usuarios', 'todos'], queryFn: () => usuariosApi.listar(1, 200) })
  const { data: mediosPago } = useQuery({ queryKey: ['medios-pago'], queryFn: mediosPagoApi.listar })

  const agregarProducto = (producto: Producto) => {
    setExito(false)
    setLineas((actual) => {
      const existente = actual.find((l) => l.productoId === producto.id)
      if (existente) {
        return actual.map((l) => (l.productoId === producto.id ? { ...l, cantidad: l.cantidad + 1 } : l))
      }
      return [...actual, { productoId: producto.id, nombre: producto.nombre, precioVenta: producto.precioVenta, descuentoPct: 0, cantidad: 1 }]
    })
  }

  const cambiarCantidad = (productoId: string, cantidad: number) =>
    setLineas((actual) => actual.map((l) => (l.productoId === productoId ? { ...l, cantidad } : l)))

  const cambiarDescuento = (productoId: string, descuentoPct: number) =>
    setLineas((actual) => actual.map((l) => (l.productoId === productoId ? { ...l, descuentoPct } : l)))

  const quitarLinea = (productoId: string) => setLineas((actual) => actual.filter((l) => l.productoId !== productoId))

  const { data: branding } = useBranding()

  const mutacionRegistrar = useMutation({
    mutationFn: (payload: {
      clienteId: string | null
      tipoComprobante: TipoComprobante
      detalles: DetalleVentaInput[]
      pagos: PagoVentaInput[]
      usuarioAutorizoSaldoId: string | null
      lineas: LineaCarritoPos[]
    }) => ventasApi.registrar(payload.clienteId, payload.tipoComprobante, payload.detalles, payload.pagos, payload.usuarioAutorizoSaldoId),
    onSuccess: (venta, payload) => {
      queryClient.invalidateQueries({ queryKey: ['ventas', 'lista'] })
      queryClient.invalidateQueries({ queryKey: ['productos', 'pos'] })

      const clienteSeleccionado = clientes?.datos.find((c) => c.id === payload.clienteId) ?? null
      generarComprobantePdf({
        venta,
        productos: payload.lineas.map((l) => ({ id: l.productoId, nombre: l.nombre })),
        cliente: clienteSeleccionado,
        mediosPago: mediosPago ?? [],
        branding: branding ?? null,
      }).save(`comprobante-${venta.tipoComprobante}-${venta.id.slice(0, 8)}.pdf`)

      setLineas([])
      setExito(true)
      setError(null)
    },
    onError: (e) => {
      setExito(false)
      setError(e instanceof ApiError ? e.message : 'No se pudo registrar la venta.')
    },
  })

  return (
    <div className="flex h-full flex-col overflow-hidden rounded-lg border border-slate-200 dark:border-slate-700">
      {(error || exito) && (
        <div className="border-b border-slate-200 p-2 dark:border-slate-700">
          {error && <p className="text-sm text-red-600">{error}</p>}
          {exito && <p className="text-sm text-emerald-600">Venta registrada correctamente.</p>}
        </div>
      )}
      <div className="grid min-h-0 flex-1 grid-cols-[25%_45%_30%] overflow-hidden">
        <CategoriaSidebarPos categorias={categorias ?? []} categoriaSeleccionadaId={categoriaId} onSeleccionar={setCategoriaId} />
        <ProductoBuscadorGrid categoriaId={categoriaId} onAgregarProducto={agregarProducto} />
        <CarritoPanelPos
          lineas={lineas}
          onCambiarCantidad={cambiarCantidad}
          onCambiarDescuento={cambiarDescuento}
          onQuitar={quitarLinea}
          clientes={clientes?.datos ?? []}
          usuarios={usuarios?.datos ?? []}
          mediosPago={mediosPago ?? []}
          onFinalizarVenta={(payload) => mutacionRegistrar.mutate(payload)}
          guardando={mutacionRegistrar.isPending}
        />
      </div>
    </div>
  )
}
