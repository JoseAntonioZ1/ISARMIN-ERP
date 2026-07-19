import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useState } from 'react'
import { type Permiso, type Rol, rolesApi } from '@/modules/roles/api/rolesApi'
import { CrearRolDialog, type FormularioCrearRol } from '@/modules/roles/components/CrearRolDialog'
import { EditarRolDialog } from '@/modules/roles/components/EditarRolDialog'
import { ApiError } from '@/shared/api/httpClient'

export function RolesPage() {
  const queryClient = useQueryClient()
  const [creandoRol, setCreandoRol] = useState(false)
  const [rolEditando, setRolEditando] = useState<Rol | null>(null)
  const [error, setError] = useState<string | null>(null)

  const { data: roles, isLoading } = useQuery({
    queryKey: ['roles-detalle'],
    queryFn: rolesApi.listarConPermisos,
  })

  const invalidarRoles = () => queryClient.invalidateQueries({ queryKey: ['roles-detalle'] })

  const mutacionCrear = useMutation({
    mutationFn: rolesApi.crear,
    onSuccess: () => {
      invalidarRoles()
      setCreandoRol(false)
    },
    onError: (e) => setError(e instanceof ApiError ? e.message : 'No se pudo crear el rol.'),
  })

  const mutacionEditarDatos = useMutation({
    mutationFn: ({ id, datos }: { id: string; datos: { nombre: string; descripcion?: string } }) =>
      rolesApi.editar(id, datos),
    onSuccess: (rolActualizado) => {
      invalidarRoles()
      setRolEditando(rolActualizado)
    },
    onError: (e) => setError(e instanceof ApiError ? e.message : 'No se pudo editar el rol.'),
  })

  const mutacionAsignarPermisos = useMutation({
    mutationFn: ({ id, permisos }: { id: string; permisos: Permiso[] }) => rolesApi.asignarPermisos(id, permisos),
    onSuccess: (rolActualizado) => {
      invalidarRoles()
      setRolEditando(rolActualizado)
    },
    onError: (e) => setError(e instanceof ApiError ? e.message : 'No se pudieron guardar los permisos.'),
  })

  const modulosConocidos = Array.from(
    new Set(roles?.flatMap((r) => r.permisos.map((p) => p.modulo)) ?? []),
  ).sort()

  return (
    <div>
      <div className="mb-4 flex items-center justify-between">
        <h1 className="text-xl font-semibold text-slate-800 dark:text-slate-100">Roles y Permisos</h1>
        <button
          type="button"
          onClick={() => setCreandoRol(true)}
          className="rounded bg-slate-800 px-4 py-2 text-sm text-white hover:bg-slate-700 dark:bg-slate-600 dark:hover:bg-slate-500"
        >
          Nuevo rol
        </button>
      </div>

      {error && <p className="mb-4 text-sm text-red-600">{error}</p>}

      {isLoading ? (
        <p className="text-slate-500">Cargando...</p>
      ) : (
        <table className="w-full border-collapse overflow-hidden rounded-lg bg-white text-left text-sm shadow-sm dark:bg-slate-800">
          <thead className="bg-slate-100 dark:bg-slate-700">
            <tr>
              <th className="px-4 py-2">Nombre</th>
              <th className="px-4 py-2">Descripción</th>
              <th className="px-4 py-2">Permisos</th>
              <th className="px-4 py-2">Acciones</th>
            </tr>
          </thead>
          <tbody>
            {roles?.map((rol) => (
              <tr key={rol.id} className="border-t border-slate-200 dark:border-slate-700">
                <td className="px-4 py-2">{rol.nombre}</td>
                <td className="px-4 py-2">{rol.descripcion}</td>
                <td className="px-4 py-2">{rol.permisos.length}</td>
                <td className="px-4 py-2">
                  <button
                    type="button"
                    onClick={() => setRolEditando(rol)}
                    className="text-slate-700 underline hover:text-slate-900 dark:text-slate-300"
                  >
                    Editar
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {creandoRol && (
        <CrearRolDialog
          onGuardar={(datos: FormularioCrearRol) => {
            setError(null)
            mutacionCrear.mutate(datos)
          }}
          onCancelar={() => setCreandoRol(false)}
          guardando={mutacionCrear.isPending}
        />
      )}

      {rolEditando && (
        <EditarRolDialog
          rol={rolEditando}
          modulosConocidos={modulosConocidos}
          onGuardarDatos={(datos) => {
            setError(null)
            mutacionEditarDatos.mutate({ id: rolEditando.id, datos })
          }}
          onGuardarPermisos={(permisos) => {
            setError(null)
            mutacionAsignarPermisos.mutate({ id: rolEditando.id, permisos })
          }}
          onCancelar={() => setRolEditando(null)}
          guardandoDatos={mutacionEditarDatos.isPending}
          guardandoPermisos={mutacionAsignarPermisos.isPending}
        />
      )}
    </div>
  )
}
