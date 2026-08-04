import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useEffect, useState } from 'react'
import { usuariosApi, type Usuario } from '@/modules/usuarios/api/usuariosApi'
import { CrearUsuarioDialog, type FormularioCrearUsuario } from '@/modules/usuarios/components/CrearUsuarioDialog'
import { EditarUsuarioDialog, type FormularioEditarUsuario } from '@/modules/usuarios/components/EditarUsuarioDialog'
import { ApiError } from '@/shared/api/httpClient'
import { ConfirmDialog } from '@/shared/components/ConfirmDialog'
import { ControlesPaginacion } from '@/shared/components/ControlesPaginacion'
import { EstadoBadge } from '@/shared/components/EstadoBadge'
import { EstadoCarga } from '@/shared/components/EstadoCarga'
import { EstadoVacio } from '@/shared/components/EstadoVacio'
import { useToast } from '@/shared/hooks/useToast'

const TAMANO_PAGINA = 20

export function UsuariosPage() {
  const queryClient = useQueryClient()
  const { mostrarExito, mostrarError } = useToast()
  const [pagina, setPagina] = useState(1)
  const [creandoUsuario, setCreandoUsuario] = useState(false)
  const [usuarioEditando, setUsuarioEditando] = useState<Usuario | null>(null)
  const [usuarioACambiarEstado, setUsuarioACambiarEstado] = useState<Usuario | null>(null)

  const {
    data: listado,
    isLoading,
    error: errorListado,
  } = useQuery({
    queryKey: ['usuarios', pagina],
    queryFn: () => usuariosApi.listar(pagina, TAMANO_PAGINA),
  })

  useEffect(() => {
    if (errorListado) mostrarError('No se pudo cargar la lista de usuarios.')
  }, [errorListado, mostrarError])

  const invalidarUsuarios = () => queryClient.invalidateQueries({ queryKey: ['usuarios'] })

  const mutacionCrear = useMutation({
    mutationFn: usuariosApi.crear,
    onSuccess: () => {
      invalidarUsuarios()
      setCreandoUsuario(false)
      mostrarExito('Usuario creado correctamente.')
    },
    onError: (e) => mostrarError(e instanceof ApiError ? e.message : 'No se pudo crear el usuario.'),
  })

  const mutacionEditar = useMutation({
    mutationFn: ({ id, datos }: { id: string; datos: FormularioEditarUsuario }) => usuariosApi.editar(id, datos),
    onSuccess: () => {
      invalidarUsuarios()
      setUsuarioEditando(null)
      mostrarExito('Usuario actualizado correctamente.')
    },
    onError: (e) => mostrarError(e instanceof ApiError ? e.message : 'No se pudo editar el usuario.'),
  })

  const mutacionCambiarEstado = useMutation({
    mutationFn: ({ id, activo }: { id: string; activo: boolean }) => usuariosApi.cambiarEstado(id, activo),
    onSuccess: () => {
      invalidarUsuarios()
      setUsuarioACambiarEstado(null)
      mostrarExito('Estado del usuario actualizado.')
    },
    onError: (e) => mostrarError(e instanceof ApiError ? e.message : 'No se pudo cambiar el estado del usuario.'),
  })

  const handleCrear = (datos: FormularioCrearUsuario) => mutacionCrear.mutate(datos)

  const handleEditar = (datos: FormularioEditarUsuario) => {
    if (!usuarioEditando) return
    mutacionEditar.mutate({ id: usuarioEditando.id, datos })
  }

  return (
    <div>
      <div className="mb-4 flex items-center justify-between">
        <h1 className="text-xl font-semibold text-slate-800 dark:text-slate-100">Usuarios</h1>
        <button
          type="button"
          onClick={() => setCreandoUsuario(true)}
          className="rounded bg-[var(--color-principal)] px-4 py-2 text-sm text-white hover:brightness-90 dark:bg-[var(--color-principal)] dark:hover:brightness-110"
        >
          Nuevo usuario
        </button>
      </div>

      {isLoading ? (
        <EstadoCarga />
      ) : listado?.datos.length === 0 ? (
        <EstadoVacio mensaje="No hay usuarios registrados." />
      ) : (
        <>
          <div className="overflow-x-auto">
          <table className="w-full border-collapse overflow-hidden rounded-lg bg-white text-left text-sm shadow-sm dark:bg-slate-800">
            <thead className="bg-slate-100 dark:bg-slate-700">
              <tr>
                <th className="px-4 py-2">Nombre</th>
                <th className="px-4 py-2">Usuario</th>
                <th className="px-4 py-2">Roles</th>
                <th className="px-4 py-2">Estado</th>
                <th className="px-4 py-2">Acciones</th>
              </tr>
            </thead>
            <tbody>
              {listado?.datos.map((usuario) => (
                <tr key={usuario.id} className="border-t border-slate-200 dark:border-slate-700">
                  <td className="px-4 py-2">{usuario.nombre}</td>
                  <td className="px-4 py-2">{usuario.nombreUsuario}</td>
                  <td className="px-4 py-2">{usuario.roles.map((r) => r.nombre).join(', ')}</td>
                  <td className="px-4 py-2">
                    <EstadoBadge estado={usuario.estado} />
                  </td>
                  <td className="px-4 py-2">
                    <button
                      type="button"
                      onClick={() => setUsuarioEditando(usuario)}
                      className="mr-3 text-[var(--color-apoyo)] underline hover:text-slate-900 dark:text-slate-300"
                    >
                      Editar
                    </button>
                    <button
                      type="button"
                      onClick={() => setUsuarioACambiarEstado(usuario)}
                      className="text-red-600 underline hover:text-red-800"
                    >
                      {usuario.estado === 'Activo' ? 'Desactivar' : 'Activar'}
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

      {creandoUsuario && (
        <CrearUsuarioDialog
          onGuardar={handleCrear}
          onCancelar={() => setCreandoUsuario(false)}
          guardando={mutacionCrear.isPending}
        />
      )}

      {usuarioEditando && (
        <EditarUsuarioDialog
          usuario={usuarioEditando}
          onGuardar={handleEditar}
          onCancelar={() => setUsuarioEditando(null)}
          guardando={mutacionEditar.isPending}
        />
      )}

      {usuarioACambiarEstado && (
        <ConfirmDialog
          titulo={usuarioACambiarEstado.estado === 'Activo' ? 'Desactivar usuario' : 'Activar usuario'}
          mensaje={`¿Confirmas ${usuarioACambiarEstado.estado === 'Activo' ? 'desactivar' : 'activar'} a "${usuarioACambiarEstado.nombre}"?`}
          confirmando={mutacionCambiarEstado.isPending}
          onConfirmar={() =>
            mutacionCambiarEstado.mutate({
              id: usuarioACambiarEstado.id,
              activo: usuarioACambiarEstado.estado !== 'Activo',
            })
          }
          onCancelar={() => setUsuarioACambiarEstado(null)}
        />
      )}
    </div>
  )
}
