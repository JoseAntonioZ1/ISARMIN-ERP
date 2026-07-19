import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useState } from 'react'
import { type Proveedor, proveedoresApi } from '@/modules/proveedores/api/proveedoresApi'
import { ProveedorDialog, type FormularioProveedor } from '@/modules/proveedores/components/ProveedorDialog'
import { ApiError } from '@/shared/api/httpClient'
import { ConfirmDialog } from '@/shared/components/ConfirmDialog'

export function ProveedoresPage() {
  const queryClient = useQueryClient()
  const [busqueda, setBusqueda] = useState('')
  const [creando, setCreando] = useState(false)
  const [editando, setEditando] = useState<Proveedor | null>(null)
  const [cambiandoEstado, setCambiandoEstado] = useState<Proveedor | null>(null)
  const [error, setError] = useState<string | null>(null)

  const { data: listado, isLoading } = useQuery({
    queryKey: ['proveedores', busqueda],
    queryFn: () => proveedoresApi.buscar(busqueda),
  })

  const invalidar = () => queryClient.invalidateQueries({ queryKey: ['proveedores'] })

  const normalizar = (datos: FormularioProveedor) => ({
    nombreRazonSocial: datos.nombreRazonSocial,
    documento: datos.documento || null,
    telefono: datos.telefono || null,
    direccion: datos.direccion || null,
  })

  const mutacionRegistrar = useMutation({
    mutationFn: proveedoresApi.registrar,
    onSuccess: () => {
      invalidar()
      setCreando(false)
    },
    onError: (e) => setError(e instanceof ApiError ? e.message : 'No se pudo registrar el proveedor.'),
  })

  const mutacionEditar = useMutation({
    mutationFn: ({ id, datos }: { id: string; datos: ReturnType<typeof normalizar> }) =>
      proveedoresApi.editar(id, datos),
    onSuccess: () => {
      invalidar()
      setEditando(null)
    },
    onError: (e) => setError(e instanceof ApiError ? e.message : 'No se pudo editar el proveedor.'),
  })

  const mutacionCambiarEstado = useMutation({
    mutationFn: ({ id, activo }: { id: string; activo: boolean }) => proveedoresApi.cambiarEstado(id, activo),
    onSuccess: () => {
      invalidar()
      setCambiandoEstado(null)
    },
    onError: (e) => setError(e instanceof ApiError ? e.message : 'No se pudo cambiar el estado del proveedor.'),
  })

  return (
    <div>
      <div className="mb-4 flex items-center justify-between">
        <h1 className="text-xl font-semibold text-slate-800 dark:text-slate-100">Proveedores</h1>
        <button
          type="button"
          onClick={() => setCreando(true)}
          className="rounded bg-slate-800 px-4 py-2 text-sm text-white hover:bg-slate-700 dark:bg-slate-600 dark:hover:bg-slate-500"
        >
          Nuevo proveedor
        </button>
      </div>

      <input
        value={busqueda}
        onChange={(e) => setBusqueda(e.target.value)}
        placeholder="Buscar por nombre o documento..."
        className="mb-4 w-full max-w-md rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
      />

      {error && <p className="mb-4 text-sm text-red-600">{error}</p>}

      {isLoading ? (
        <p className="text-slate-500">Cargando...</p>
      ) : (
        <table className="w-full border-collapse overflow-hidden rounded-lg bg-white text-left text-sm shadow-sm dark:bg-slate-800">
          <thead className="bg-slate-100 dark:bg-slate-700">
            <tr>
              <th className="px-4 py-2">Nombre / Razón social</th>
              <th className="px-4 py-2">Documento</th>
              <th className="px-4 py-2">Teléfono</th>
              <th className="px-4 py-2">Estado</th>
              <th className="px-4 py-2">Acciones</th>
            </tr>
          </thead>
          <tbody>
            {listado?.datos.map((proveedor) => (
              <tr key={proveedor.id} className="border-t border-slate-200 dark:border-slate-700">
                <td className="px-4 py-2">{proveedor.nombreRazonSocial}</td>
                <td className="px-4 py-2">{proveedor.documento ?? '—'}</td>
                <td className="px-4 py-2">{proveedor.telefono ?? '—'}</td>
                <td className="px-4 py-2">{proveedor.estado}</td>
                <td className="px-4 py-2">
                  <button
                    type="button"
                    onClick={() => setEditando(proveedor)}
                    className="mr-3 text-slate-700 underline hover:text-slate-900 dark:text-slate-300"
                  >
                    Editar
                  </button>
                  <button
                    type="button"
                    onClick={() => setCambiandoEstado(proveedor)}
                    className="text-red-600 underline hover:text-red-800"
                  >
                    {proveedor.estado === 'Activo' ? 'Desactivar' : 'Activar'}
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {creando && (
        <ProveedorDialog
          onGuardar={(datos) => {
            setError(null)
            mutacionRegistrar.mutate(normalizar(datos))
          }}
          onCancelar={() => setCreando(false)}
          guardando={mutacionRegistrar.isPending}
        />
      )}

      {editando && (
        <ProveedorDialog
          proveedor={editando}
          onGuardar={(datos) => {
            setError(null)
            mutacionEditar.mutate({ id: editando.id, datos: normalizar(datos) })
          }}
          onCancelar={() => setEditando(null)}
          guardando={mutacionEditar.isPending}
        />
      )}

      {cambiandoEstado && (
        <ConfirmDialog
          titulo={cambiandoEstado.estado === 'Activo' ? 'Desactivar proveedor' : 'Activar proveedor'}
          mensaje={`¿Confirmas ${cambiandoEstado.estado === 'Activo' ? 'desactivar' : 'activar'} a "${cambiandoEstado.nombreRazonSocial}"?`}
          confirmando={mutacionCambiarEstado.isPending}
          onConfirmar={() =>
            mutacionCambiarEstado.mutate({ id: cambiandoEstado.id, activo: cambiandoEstado.estado !== 'Activo' })
          }
          onCancelar={() => setCambiandoEstado(null)}
        />
      )}
    </div>
  )
}
