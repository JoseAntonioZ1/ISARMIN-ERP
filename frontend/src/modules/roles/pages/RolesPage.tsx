import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useEffect, useState } from 'react'
import { type Permiso, type Rol, rolesApi } from '@/modules/roles/api/rolesApi'
import { CrearRolDialog, type FormularioCrearRol } from '@/modules/roles/components/CrearRolDialog'
import { EditarRolDialog } from '@/modules/roles/components/EditarRolDialog'
import { ApiError } from '@/shared/api/httpClient'
import { EstadoCarga } from '@/shared/components/EstadoCarga'
import { EstadoVacio } from '@/shared/components/EstadoVacio'
import { useToast } from '@/shared/hooks/useToast'

export function RolesPage() {
  const queryClient = useQueryClient()
  const { mostrarExito, mostrarError } = useToast()
  const [creandoRol, setCreandoRol] = useState(false)
  const [rolEditando, setRolEditando] = useState<Rol | null>(null)

  const {
    data: roles,
    isLoading,
    error: errorListado,
  } = useQuery({
    queryKey: ['roles-detalle'],
    queryFn: rolesApi.listarConPermisos,
  })

  useEffect(() => {
    if (errorListado) mostrarError('No se pudo cargar la lista de roles.')
  }, [errorListado, mostrarError])

  const invalidarRoles = () => {
    queryClient.invalidateQueries({ queryKey: ['roles-detalle'] })
    // Tambien invalida el listado resumido usado por SelectorRoles (Usuarios) — antes un rol nuevo
    // no aparecia ahi hasta que otra accion disparara un refetch de esa query por separado.
    queryClient.invalidateQueries({ queryKey: ['roles'] })
  }

  const mutacionCrear = useMutation({
    mutationFn: rolesApi.crear,
    onSuccess: () => {
      invalidarRoles()
      setCreandoRol(false)
      mostrarExito('Rol creado correctamente.')
    },
    onError: (e) => mostrarError(e instanceof ApiError ? e.message : 'No se pudo crear el rol.'),
  })

  const mutacionEditarDatos = useMutation({
    mutationFn: ({ id, datos }: { id: string; datos: { nombre: string; descripcion?: string } }) =>
      rolesApi.editar(id, datos),
    onSuccess: (rolActualizado) => {
      invalidarRoles()
      setRolEditando(rolActualizado)
      mostrarExito('Rol actualizado correctamente.')
    },
    onError: (e) => mostrarError(e instanceof ApiError ? e.message : 'No se pudo editar el rol.'),
  })

  const mutacionAsignarPermisos = useMutation({
    mutationFn: ({ id, permisos }: { id: string; permisos: Permiso[] }) => rolesApi.asignarPermisos(id, permisos),
    onSuccess: (rolActualizado) => {
      invalidarRoles()
      setRolEditando(rolActualizado)
      mostrarExito('Permisos actualizados correctamente.')
    },
    onError: (e) => mostrarError(e instanceof ApiError ? e.message : 'No se pudieron guardar los permisos.'),
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
          className="rounded bg-[var(--color-principal)] px-4 py-2 text-sm text-white hover:brightness-90 dark:bg-[var(--color-principal)] dark:hover:brightness-110"
        >
          Nuevo rol
        </button>
      </div>

      {isLoading ? (
        <EstadoCarga />
      ) : roles?.length === 0 ? (
        <EstadoVacio mensaje="No hay roles registrados." />
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
                    className="text-[var(--color-apoyo)] underline hover:text-slate-900 dark:text-slate-300"
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
          onGuardar={(datos: FormularioCrearRol) => mutacionCrear.mutate(datos)}
          onCancelar={() => setCreandoRol(false)}
          guardando={mutacionCrear.isPending}
        />
      )}

      {rolEditando && (
        <EditarRolDialog
          rol={rolEditando}
          modulosConocidos={modulosConocidos}
          onGuardarDatos={(datos) => mutacionEditarDatos.mutate({ id: rolEditando.id, datos })}
          onGuardarPermisos={(permisos) => mutacionAsignarPermisos.mutate({ id: rolEditando.id, permisos })}
          onCancelar={() => setRolEditando(null)}
          guardandoDatos={mutacionEditarDatos.isPending}
          guardandoPermisos={mutacionAsignarPermisos.isPending}
        />
      )}
    </div>
  )
}
