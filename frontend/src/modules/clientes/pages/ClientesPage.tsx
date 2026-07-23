import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useEffect, useState } from 'react'
import { type Cliente, clientesApi } from '@/modules/clientes/api/clientesApi'
import { ClienteDialog, type FormularioCliente } from '@/modules/clientes/components/ClienteDialog'
import { ApiError } from '@/shared/api/httpClient'
import { ConfirmDialog } from '@/shared/components/ConfirmDialog'
import { ControlesPaginacion } from '@/shared/components/ControlesPaginacion'
import { EstadoBadge } from '@/shared/components/EstadoBadge'
import { EstadoCarga } from '@/shared/components/EstadoCarga'
import { EstadoVacio } from '@/shared/components/EstadoVacio'
import { useToast } from '@/shared/hooks/useToast'

const TAMANO_PAGINA = 20

export function ClientesPage() {
  const queryClient = useQueryClient()
  const { mostrarExito, mostrarError } = useToast()
  const [busqueda, setBusqueda] = useState('')
  const [pagina, setPagina] = useState(1)
  const [creando, setCreando] = useState(false)
  const [editando, setEditando] = useState<Cliente | null>(null)
  const [cambiandoEstado, setCambiandoEstado] = useState<Cliente | null>(null)

  const {
    data: listado,
    isLoading,
    error: errorListado,
  } = useQuery({
    queryKey: ['clientes', busqueda, pagina],
    queryFn: () => clientesApi.buscar(busqueda, pagina, TAMANO_PAGINA),
  })

  useEffect(() => {
    if (errorListado) mostrarError('No se pudo cargar la lista de clientes.')
  }, [errorListado, mostrarError])

  const invalidar = () => queryClient.invalidateQueries({ queryKey: ['clientes'] })

  const normalizar = (datos: FormularioCliente) => ({
    nombreRazonSocial: datos.nombreRazonSocial,
    telefono: datos.telefono,
    direccion: datos.direccion || null,
    tipoDocumento: datos.tipoDocumento || null,
    numeroDocumento: datos.numeroDocumento || null,
  })

  const mutacionRegistrar = useMutation({
    mutationFn: clientesApi.registrar,
    onSuccess: () => {
      invalidar()
      setCreando(false)
      mostrarExito('Cliente registrado correctamente.')
    },
    onError: (e) => mostrarError(e instanceof ApiError ? e.message : 'No se pudo registrar el cliente.'),
  })

  const mutacionEditar = useMutation({
    mutationFn: ({ id, datos }: { id: string; datos: ReturnType<typeof normalizar> }) => clientesApi.editar(id, datos),
    onSuccess: () => {
      invalidar()
      setEditando(null)
      mostrarExito('Cliente actualizado correctamente.')
    },
    onError: (e) => mostrarError(e instanceof ApiError ? e.message : 'No se pudo editar el cliente.'),
  })

  const mutacionCambiarEstado = useMutation({
    mutationFn: ({ id, activo }: { id: string; activo: boolean }) => clientesApi.cambiarEstado(id, activo),
    onSuccess: () => {
      invalidar()
      setCambiandoEstado(null)
      mostrarExito('Estado del cliente actualizado.')
    },
    onError: (e) => mostrarError(e instanceof ApiError ? e.message : 'No se pudo cambiar el estado del cliente.'),
  })

  return (
    <div>
      <div className="mb-4 flex items-center justify-between">
        <h1 className="text-xl font-semibold text-slate-800 dark:text-slate-100">Clientes</h1>
        <button
          type="button"
          onClick={() => setCreando(true)}
          className="rounded bg-[var(--color-principal)] px-4 py-2 text-sm text-white hover:brightness-90 dark:bg-[var(--color-principal)] dark:hover:brightness-110"
        >
          Nuevo cliente
        </button>
      </div>

      <input
        value={busqueda}
        onChange={(e) => {
          setBusqueda(e.target.value)
          setPagina(1)
        }}
        placeholder="Buscar por nombre o número de documento..."
        className="mb-4 w-full max-w-md rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
      />

      {isLoading ? (
        <EstadoCarga />
      ) : listado?.datos.length === 0 ? (
        <EstadoVacio mensaje="No se encontraron clientes." />
      ) : (
        <>
          <table className="w-full border-collapse overflow-hidden rounded-lg bg-white text-left text-sm shadow-sm dark:bg-slate-800">
            <thead className="bg-slate-100 dark:bg-slate-700">
              <tr>
                <th className="px-4 py-2">Nombre / Razón social</th>
                <th className="px-4 py-2">Teléfono</th>
                <th className="px-4 py-2">Documento</th>
                <th className="px-4 py-2">Estado</th>
                <th className="px-4 py-2">Acciones</th>
              </tr>
            </thead>
            <tbody>
              {listado?.datos.map((cliente) => (
                <tr key={cliente.id} className="border-t border-slate-200 dark:border-slate-700">
                  <td className="px-4 py-2">{cliente.nombreRazonSocial}</td>
                  <td className="px-4 py-2">{cliente.telefono}</td>
                  <td className="px-4 py-2">
                    {cliente.tipoDocumento ? `${cliente.tipoDocumento} ${cliente.numeroDocumento}` : '—'}
                  </td>
                  <td className="px-4 py-2">
                    <EstadoBadge estado={cliente.estado} />
                  </td>
                  <td className="px-4 py-2">
                    <button
                      type="button"
                      onClick={() => setEditando(cliente)}
                      className="mr-3 text-[var(--color-apoyo)] underline hover:text-slate-900 dark:text-slate-300"
                    >
                      Editar
                    </button>
                    <button
                      type="button"
                      onClick={() => setCambiandoEstado(cliente)}
                      className="text-red-600 underline hover:text-red-800"
                    >
                      {cliente.estado === 'Activo' ? 'Desactivar' : 'Activar'}
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
          <ControlesPaginacion pagina={pagina} tamanoPagina={TAMANO_PAGINA} total={listado?.total ?? 0} onCambiarPagina={setPagina} />
        </>
      )}

      {creando && (
        <ClienteDialog
          onGuardar={(datos) => mutacionRegistrar.mutate(normalizar(datos))}
          onCancelar={() => setCreando(false)}
          guardando={mutacionRegistrar.isPending}
        />
      )}

      {editando && (
        <ClienteDialog
          cliente={editando}
          onGuardar={(datos) => mutacionEditar.mutate({ id: editando.id, datos: normalizar(datos) })}
          onCancelar={() => setEditando(null)}
          guardando={mutacionEditar.isPending}
        />
      )}

      {cambiandoEstado && (
        <ConfirmDialog
          titulo={cambiandoEstado.estado === 'Activo' ? 'Desactivar cliente' : 'Activar cliente'}
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
