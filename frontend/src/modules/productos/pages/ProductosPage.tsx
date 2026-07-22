import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useState } from 'react'
import { categoriasApi, unidadesMedidaApi } from '@/modules/catalogos/api/catalogosApi'
import { AjustarInventarioDialog } from '@/modules/productos/components/AjustarInventarioDialog'
import { CrearProductoDialog } from '@/modules/productos/components/CrearProductoDialog'
import { EditarProductoDialog } from '@/modules/productos/components/EditarProductoDialog'
import { KardexDialog } from '@/modules/productos/components/KardexDialog'
import { type Producto, productosApi } from '@/modules/productos/api/productosApi'
import { ApiError } from '@/shared/api/httpClient'
import { ConfirmDialog } from '@/shared/components/ConfirmDialog'

export function ProductosPage() {
  const queryClient = useQueryClient()
  const [busqueda, setBusqueda] = useState('')
  const [creando, setCreando] = useState(false)
  const [editando, setEditando] = useState<Producto | null>(null)
  const [cambiandoEstado, setCambiandoEstado] = useState<Producto | null>(null)
  const [ajustando, setAjustando] = useState<Producto | null>(null)
  const [verKardex, setVerKardex] = useState<Producto | null>(null)
  const [error, setError] = useState<string | null>(null)

  const { data: listado, isLoading } = useQuery({
    queryKey: ['productos', busqueda],
    queryFn: () => productosApi.buscar(busqueda),
  })

  const { data: categorias } = useQuery({ queryKey: ['categorias'], queryFn: categoriasApi.listar })
  const { data: unidadesMedida } = useQuery({ queryKey: ['unidades-medida'], queryFn: unidadesMedidaApi.listar })

  const invalidar = () => queryClient.invalidateQueries({ queryKey: ['productos'] })

  const mutacionRegistrar = useMutation({
    mutationFn: productosApi.registrar,
    onSuccess: () => {
      invalidar()
      setCreando(false)
    },
    onError: (e) => setError(e instanceof ApiError ? e.message : 'No se pudo registrar el producto.'),
  })

  const mutacionEditar = useMutation({
    mutationFn: ({ id, datos }: { id: string; datos: Parameters<typeof productosApi.editar>[1] }) =>
      productosApi.editar(id, datos),
    onSuccess: () => {
      invalidar()
      setEditando(null)
    },
    onError: (e) => setError(e instanceof ApiError ? e.message : 'No se pudo editar el producto.'),
  })

  const mutacionCambiarEstado = useMutation({
    mutationFn: ({ id, activo }: { id: string; activo: boolean }) => productosApi.cambiarEstado(id, activo),
    onSuccess: () => {
      invalidar()
      setCambiandoEstado(null)
    },
    onError: (e) => setError(e instanceof ApiError ? e.message : 'No se pudo cambiar el estado del producto.'),
  })

  const mutacionAjustar = useMutation({
    mutationFn: ({ id, cantidadAjuste, motivo }: { id: string; cantidadAjuste: number; motivo: string }) =>
      productosApi.ajustarInventario(id, cantidadAjuste, motivo),
    onSuccess: () => {
      invalidar()
      setAjustando(null)
    },
    onError: (e) => setError(e instanceof ApiError ? e.message : 'No se pudo ajustar el inventario.'),
  })

  const nombreCategoria = (id: string) => categorias?.find((c) => c.id === id)?.nombre ?? '—'
  const nombreUnidadMedida = (id: string) => unidadesMedida?.find((u) => u.id === id)?.nombre ?? '—'

  return (
    <div>
      <div className="mb-4 flex items-center justify-between">
        <h1 className="text-xl font-semibold text-slate-800 dark:text-slate-100">Productos</h1>
        <button
          type="button"
          onClick={() => setCreando(true)}
          className="rounded bg-[var(--color-principal)] px-4 py-2 text-sm text-white hover:brightness-90 dark:bg-[var(--color-principal)] dark:hover:brightness-110"
        >
          Nuevo producto
        </button>
      </div>

      <input
        value={busqueda}
        onChange={(e) => setBusqueda(e.target.value)}
        placeholder="Buscar por nombre, código interno o código de barras..."
        className="mb-4 w-full max-w-md rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
      />

      {error && <p className="mb-4 text-sm text-red-600">{error}</p>}

      {isLoading ? (
        <p className="text-[var(--color-terciario)]">Cargando...</p>
      ) : (
        <table className="w-full border-collapse overflow-hidden rounded-lg bg-white text-left text-sm shadow-sm dark:bg-slate-800">
          <thead className="bg-slate-100 dark:bg-slate-700">
            <tr>
              <th className="px-4 py-2">Código</th>
              <th className="px-4 py-2">Nombre</th>
              <th className="px-4 py-2">Categoría</th>
              <th className="px-4 py-2">Unidad</th>
              <th className="px-4 py-2">Precio</th>
              <th className="px-4 py-2">Stock</th>
              <th className="px-4 py-2">Estado</th>
              <th className="px-4 py-2">Acciones</th>
            </tr>
          </thead>
          <tbody>
            {listado?.datos.map((producto) => (
              <tr key={producto.id} className="border-t border-slate-200 dark:border-slate-700">
                <td className="px-4 py-2">{producto.codigoInterno}</td>
                <td className="px-4 py-2">{producto.nombre}</td>
                <td className="px-4 py-2">{nombreCategoria(producto.categoriaId)}</td>
                <td className="px-4 py-2">{nombreUnidadMedida(producto.unidadMedidaId)}</td>
                <td className="px-4 py-2">{producto.precioVenta.toFixed(2)}</td>
                <td className="px-4 py-2">{producto.stockActual}</td>
                <td className="px-4 py-2">{producto.estado}</td>
                <td className="px-4 py-2">
                  <button
                    type="button"
                    onClick={() => setEditando(producto)}
                    className="mr-3 text-[var(--color-apoyo)] underline hover:text-slate-900 dark:text-slate-300"
                  >
                    Editar
                  </button>
                  <button
                    type="button"
                    onClick={() => setAjustando(producto)}
                    className="mr-3 text-[var(--color-apoyo)] underline hover:text-slate-900 dark:text-slate-300"
                  >
                    Ajustar
                  </button>
                  <button
                    type="button"
                    onClick={() => setVerKardex(producto)}
                    className="mr-3 text-[var(--color-apoyo)] underline hover:text-slate-900 dark:text-slate-300"
                  >
                    Kardex
                  </button>
                  <button
                    type="button"
                    onClick={() => setCambiandoEstado(producto)}
                    className="text-red-600 underline hover:text-red-800"
                  >
                    {producto.estado === 'Activo' ? 'Desactivar' : 'Activar'}
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {creando && (
        <CrearProductoDialog
          categorias={categorias ?? []}
          unidadesMedida={unidadesMedida ?? []}
          onGuardar={(datos) => {
            setError(null)
            mutacionRegistrar.mutate(datos)
          }}
          onCancelar={() => setCreando(false)}
          guardando={mutacionRegistrar.isPending}
        />
      )}

      {editando && (
        <EditarProductoDialog
          producto={editando}
          categorias={categorias ?? []}
          unidadesMedida={unidadesMedida ?? []}
          onGuardar={(datos) => {
            setError(null)
            mutacionEditar.mutate({ id: editando.id, datos })
          }}
          onCancelar={() => setEditando(null)}
          guardando={mutacionEditar.isPending}
        />
      )}

      {cambiandoEstado && (
        <ConfirmDialog
          titulo={cambiandoEstado.estado === 'Activo' ? 'Desactivar producto' : 'Activar producto'}
          mensaje={`¿Confirmas ${cambiandoEstado.estado === 'Activo' ? 'desactivar' : 'activar'} "${cambiandoEstado.nombre}"?`}
          confirmando={mutacionCambiarEstado.isPending}
          onConfirmar={() =>
            mutacionCambiarEstado.mutate({ id: cambiandoEstado.id, activo: cambiandoEstado.estado !== 'Activo' })
          }
          onCancelar={() => setCambiandoEstado(null)}
        />
      )}

      {ajustando && (
        <AjustarInventarioDialog
          producto={ajustando}
          onGuardar={(datos) => {
            setError(null)
            mutacionAjustar.mutate({ id: ajustando.id, ...datos })
          }}
          onCancelar={() => setAjustando(null)}
          guardando={mutacionAjustar.isPending}
        />
      )}

      {verKardex && <KardexDialog producto={verKardex} onCerrar={() => setVerKardex(null)} />}
    </div>
  )
}
