import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useState } from 'react'
import { categoriasApi } from '@/modules/catalogos/api/catalogosApi'
import { comprasApi } from '@/modules/compras/api/comprasApi'
import { CarritoPanelCompra, type LineaCarritoCompra } from '@/modules/compras/components/pos/CarritoPanelCompra'
import type { Producto } from '@/modules/productos/api/productosApi'
import { proveedoresApi } from '@/modules/proveedores/api/proveedoresApi'
import { ApiError } from '@/shared/api/httpClient'
import { CategoriaSidebarPos } from '@/shared/components/pos/CategoriaSidebarPos'
import { ProductoBuscadorGrid } from '@/shared/components/pos/ProductoBuscadorGrid'

export function CompraPosPage() {
  const queryClient = useQueryClient()
  const [categoriaId, setCategoriaId] = useState<string | null>(null)
  const [lineas, setLineas] = useState<LineaCarritoCompra[]>([])
  const [error, setError] = useState<string | null>(null)
  const [exito, setExito] = useState(false)

  const { data: categorias } = useQuery({ queryKey: ['categorias'], queryFn: categoriasApi.listar })
  const { data: proveedores } = useQuery({ queryKey: ['proveedores', 'todos'], queryFn: () => proveedoresApi.buscar(undefined, 1, 200) })

  const agregarProducto = (producto: Producto) => {
    setExito(false)
    setLineas((actual) => {
      const existente = actual.find((l) => l.productoId === producto.id)
      if (existente) {
        return actual.map((l) => (l.productoId === producto.id ? { ...l, cantidad: l.cantidad + 1 } : l))
      }
      return [...actual, { productoId: producto.id, nombre: producto.nombre, costoUnitario: producto.costoReferencia, cantidad: 1 }]
    })
  }

  const cambiarCantidad = (productoId: string, cantidad: number) =>
    setLineas((actual) => actual.map((l) => (l.productoId === productoId ? { ...l, cantidad } : l)))

  const cambiarCosto = (productoId: string, costoUnitario: number) =>
    setLineas((actual) => actual.map((l) => (l.productoId === productoId ? { ...l, costoUnitario } : l)))

  const quitarLinea = (productoId: string) => setLineas((actual) => actual.filter((l) => l.productoId !== productoId))

  const mutacionRegistrar = useMutation({
    mutationFn: comprasApi.registrar,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['compras'] })
      queryClient.invalidateQueries({ queryKey: ['productos', 'pos'] })
      setLineas([])
      setExito(true)
      setError(null)
    },
    onError: (e) => {
      setExito(false)
      setError(e instanceof ApiError ? e.message : 'No se pudo registrar la compra.')
    },
  })

  return (
    <div className="flex h-full flex-col overflow-hidden rounded-lg border border-slate-200 dark:border-slate-700">
      {(error || exito) && (
        <div className="border-b border-slate-200 p-2 dark:border-slate-700">
          {error && <p className="text-sm text-red-600">{error}</p>}
          {exito && <p className="text-sm text-emerald-600">Compra registrada correctamente.</p>}
        </div>
      )}
      <div className="grid min-h-0 flex-1 grid-cols-[25%_45%_30%] overflow-hidden">
        <CategoriaSidebarPos categorias={categorias ?? []} categoriaSeleccionadaId={categoriaId} onSeleccionar={setCategoriaId} />
        <ProductoBuscadorGrid
          categoriaId={categoriaId}
          onAgregarProducto={agregarProducto}
          obtenerPrecio={(p) => p.costoReferencia}
          validarStock={false}
        />
        <CarritoPanelCompra
          lineas={lineas}
          onCambiarCantidad={cambiarCantidad}
          onCambiarCosto={cambiarCosto}
          onQuitar={quitarLinea}
          proveedores={proveedores?.datos ?? []}
          onRegistrarCompra={(payload) => mutacionRegistrar.mutate(payload)}
          guardando={mutacionRegistrar.isPending}
        />
      </div>
    </div>
  )
}
