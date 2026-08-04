import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useEffect, useState } from 'react'
import { type Proveedor, proveedoresApi } from '@/modules/proveedores/api/proveedoresApi'
import { ProveedorDialog, type FormularioProveedor } from '@/modules/proveedores/components/ProveedorDialog'
import { ApiError } from '@/shared/api/httpClient'
import { ConfirmDialog } from '@/shared/components/ConfirmDialog'
import { ControlesPaginacion } from '@/shared/components/ControlesPaginacion'
import { EstadoBadge } from '@/shared/components/EstadoBadge'
import { EstadoCarga } from '@/shared/components/EstadoCarga'
import { EstadoVacio } from '@/shared/components/EstadoVacio'
import { useToast } from '@/shared/hooks/useToast'

const TAMANO_PAGINA = 20

export function ProveedoresPage() {
  const queryClient = useQueryClient()
  const { mostrarExito, mostrarError } = useToast()
  const [busqueda, setBusqueda] = useState('')
  const [pagina, setPagina] = useState(1)
  const [creando, setCreando] = useState(false)
  const [editando, setEditando] = useState<Proveedor | null>(null)
  const [cambiandoEstado, setCambiandoEstado] = useState<Proveedor | null>(null)

  const {
    data: listado,
    isLoading,
    error: errorListado,
  } = useQuery({
    queryKey: ['proveedores', busqueda, pagina],
    queryFn: () => proveedoresApi.buscar(busqueda, pagina, TAMANO_PAGINA),
  })

  useEffect(() => {
    if (errorListado) mostrarError('No se pudo cargar la lista de proveedores.')
  }, [errorListado, mostrarError])

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
      mostrarExito('Proveedor registrado correctamente.')
    },
    onError: (e) => mostrarError(e instanceof ApiError ? e.message : 'No se pudo registrar el proveedor.'),
  })

  const mutacionEditar = useMutation({
    mutationFn: ({ id, datos }: { id: string; datos: ReturnType<typeof normalizar> }) =>
      proveedoresApi.editar(id, datos),
    onSuccess: () => {
      invalidar()
      setEditando(null)
      mostrarExito('Proveedor actualizado correctamente.')
    },
    onError: (e) => mostrarError(e instanceof ApiError ? e.message : 'No se pudo editar el proveedor.'),
  })

  const mutacionCambiarEstado = useMutation({
    mutationFn: ({ id, activo }: { id: string; activo: boolean }) => proveedoresApi.cambiarEstado(id, activo),
    onSuccess: () => {
      invalidar()
      setCambiandoEstado(null)
      mostrarExito('Estado del proveedor actualizado.')
    },
    onError: (e) => mostrarError(e instanceof ApiError ? e.message : 'No se pudo cambiar el estado del proveedor.'),
  })

  return (
    <div>
      <div className="mb-4 flex items-center justify-between">
        <h1 className="text-xl font-semibold text-slate-800 dark:text-slate-100">Proveedores</h1>
        <button
          type="button"
          onClick={() => setCreando(true)}
          className="rounded bg-[var(--color-principal)] px-4 py-2 text-sm text-white hover:brightness-90 dark:bg-[var(--color-principal)] dark:hover:brightness-110"
        >
          Nuevo proveedor
        </button>
      </div>

      <input
        value={busqueda}
        onChange={(e) => {
          setBusqueda(e.target.value)
          setPagina(1)
        }}
        placeholder="Buscar por nombre o documento..."
        className="mb-4 w-full max-w-md rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
      />

      {isLoading ? (
        <EstadoCarga />
      ) : listado?.datos.length === 0 ? (
        <EstadoVacio mensaje="No se encontraron proveedores." />
      ) : (
        <>
          <div className="overflow-x-auto">
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
                  <td className="px-4 py-2">
                    <EstadoBadge estado={proveedor.estado} />
                  </td>
                  <td className="px-4 py-2">
                    <button
                      type="button"
                      onClick={() => setEditando(proveedor)}
                      className="mr-3 text-[var(--color-apoyo)] underline hover:text-slate-900 dark:text-slate-300"
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
          </div>
          <ControlesPaginacion pagina={pagina} tamanoPagina={TAMANO_PAGINA} total={listado?.total ?? 0} onCambiarPagina={setPagina} />
        </>
      )}

      {creando && (
        <ProveedorDialog
          onGuardar={(datos) => mutacionRegistrar.mutate(normalizar(datos))}
          onCancelar={() => setCreando(false)}
          guardando={mutacionRegistrar.isPending}
        />
      )}

      {editando && (
        <ProveedorDialog
          proveedor={editando}
          onGuardar={(datos) => mutacionEditar.mutate({ id: editando.id, datos: normalizar(datos) })}
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
