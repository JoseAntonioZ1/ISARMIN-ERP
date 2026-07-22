import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useState } from 'react'
import { usuariosApi, type Usuario } from '@/modules/usuarios/api/usuariosApi'
import { CrearUsuarioDialog, type FormularioCrearUsuario } from '@/modules/usuarios/components/CrearUsuarioDialog'
import { EditarUsuarioDialog, type FormularioEditarUsuario } from '@/modules/usuarios/components/EditarUsuarioDialog'
import { ApiError } from '@/shared/api/httpClient'
import { ConfirmDialog } from '@/shared/components/ConfirmDialog'

export function UsuariosPage() {
  const queryClient = useQueryClient()
  const [creandoUsuario, setCreandoUsuario] = useState(false)
  const [usuarioEditando, setUsuarioEditando] = useState<Usuario | null>(null)
  const [usuarioACambiarEstado, setUsuarioACambiarEstado] = useState<Usuario | null>(null)
  const [error, setError] = useState<string | null>(null)

  const { data: listado, isLoading } = useQuery({
    queryKey: ['usuarios'],
    queryFn: () => usuariosApi.listar(),
  })

  const invalidarUsuarios = () => queryClient.invalidateQueries({ queryKey: ['usuarios'] })

  const mutacionCrear = useMutation({
    mutationFn: usuariosApi.crear,
    onSuccess: () => {
      invalidarUsuarios()
      setCreandoUsuario(false)
    },
    onError: (e) => setError(e instanceof ApiError ? e.message : 'No se pudo crear el usuario.'),
  })

  const mutacionEditar = useMutation({
    mutationFn: ({ id, datos }: { id: string; datos: FormularioEditarUsuario }) => usuariosApi.editar(id, datos),
    onSuccess: () => {
      invalidarUsuarios()
      setUsuarioEditando(null)
    },
    onError: (e) => setError(e instanceof ApiError ? e.message : 'No se pudo editar el usuario.'),
  })

  const mutacionCambiarEstado = useMutation({
    mutationFn: ({ id, activo }: { id: string; activo: boolean }) => usuariosApi.cambiarEstado(id, activo),
    onSuccess: () => {
      invalidarUsuarios()
      setUsuarioACambiarEstado(null)
    },
    onError: (e) => setError(e instanceof ApiError ? e.message : 'No se pudo cambiar el estado del usuario.'),
  })

  const handleCrear = (datos: FormularioCrearUsuario) => {
    setError(null)
    mutacionCrear.mutate(datos)
  }

  const handleEditar = (datos: FormularioEditarUsuario) => {
    if (!usuarioEditando) return
    setError(null)
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

      {error && <p className="mb-4 text-sm text-red-600">{error}</p>}

      {isLoading ? (
        <p className="text-[var(--color-terciario)]">Cargando...</p>
      ) : (
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
                <td className="px-4 py-2">{usuario.estado}</td>
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
