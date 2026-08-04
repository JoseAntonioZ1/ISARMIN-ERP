import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useEffect, useState } from 'react'
import { unidadesMedidaApi } from '@/modules/catalogos/api/catalogosApi'
import { ApiError } from '@/shared/api/httpClient'
import { EstadoCarga } from '@/shared/components/EstadoCarga'
import { EstadoVacio } from '@/shared/components/EstadoVacio'
import { useToast } from '@/shared/hooks/useToast'

export function UnidadesMedidaPage() {
  const queryClient = useQueryClient()
  const { mostrarExito, mostrarError } = useToast()
  const [nombreNuevo, setNombreNuevo] = useState('')
  const [editandoId, setEditandoId] = useState<string | null>(null)
  const [nombreEditado, setNombreEditado] = useState('')

  const {
    data: unidadesMedida,
    isLoading,
    error: errorListado,
  } = useQuery({
    queryKey: ['unidades-medida'],
    queryFn: unidadesMedidaApi.listar,
  })

  useEffect(() => {
    if (errorListado) mostrarError('No se pudo cargar la lista de unidades de medida.')
  }, [errorListado, mostrarError])

  const invalidar = () => queryClient.invalidateQueries({ queryKey: ['unidades-medida'] })

  const mutacionCrear = useMutation({
    mutationFn: unidadesMedidaApi.crear,
    onSuccess: () => {
      invalidar()
      setNombreNuevo('')
      mostrarExito('Unidad de medida creada correctamente.')
    },
    onError: (e) => mostrarError(e instanceof ApiError ? e.message : 'No se pudo crear la unidad de medida.'),
  })

  const mutacionEditar = useMutation({
    mutationFn: ({ id, nombre }: { id: string; nombre: string }) => unidadesMedidaApi.editar(id, nombre),
    onSuccess: () => {
      invalidar()
      setEditandoId(null)
      mostrarExito('Unidad de medida actualizada correctamente.')
    },
    onError: (e) => mostrarError(e instanceof ApiError ? e.message : 'No se pudo editar la unidad de medida.'),
  })

  const handleCrear = () => {
    if (!nombreNuevo.trim()) return
    mutacionCrear.mutate(nombreNuevo.trim())
  }

  return (
    <div>
      <h1 className="mb-4 text-xl font-semibold text-slate-800 dark:text-slate-100">Unidades de Medida</h1>

      <div className="mb-4 flex gap-2">
        <input
          value={nombreNuevo}
          onChange={(e) => setNombreNuevo(e.target.value)}
          placeholder="Nombre de la nueva unidad (ej. Galón)"
          className="flex-1 rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />
        <button
          type="button"
          onClick={handleCrear}
          disabled={mutacionCrear.isPending}
          className="rounded bg-[var(--color-principal)] px-4 py-2 text-sm text-white hover:brightness-90 disabled:opacity-50 dark:bg-[var(--color-principal)] dark:hover:brightness-110"
        >
          Agregar
        </button>
      </div>

      {isLoading ? (
        <EstadoCarga />
      ) : unidadesMedida?.length === 0 ? (
        <EstadoVacio mensaje="No hay unidades de medida registradas." />
      ) : (
        <div className="overflow-x-auto">
        <table className="w-full border-collapse overflow-hidden rounded-lg bg-white text-left text-sm shadow-sm dark:bg-slate-800">
          <thead className="bg-slate-100 dark:bg-slate-700">
            <tr>
              <th className="px-4 py-2">Nombre</th>
              <th className="px-4 py-2">Acciones</th>
            </tr>
          </thead>
          <tbody>
            {unidadesMedida?.map((unidadMedida) => (
              <tr key={unidadMedida.id} className="border-t border-slate-200 dark:border-slate-700">
                <td className="px-4 py-2">
                  {editandoId === unidadMedida.id ? (
                    <input
                      value={nombreEditado}
                      onChange={(e) => setNombreEditado(e.target.value)}
                      className="w-full rounded border border-slate-300 px-2 py-1 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
                    />
                  ) : (
                    unidadMedida.nombre
                  )}
                </td>
                <td className="px-4 py-2">
                  {editandoId === unidadMedida.id ? (
                    <>
                      <button
                        type="button"
                        onClick={() => mutacionEditar.mutate({ id: unidadMedida.id, nombre: nombreEditado.trim() })}
                        disabled={mutacionEditar.isPending}
                        className="mr-3 text-[var(--color-apoyo)] underline hover:text-slate-900 dark:text-slate-300"
                      >
                        Guardar
                      </button>
                      <button
                        type="button"
                        onClick={() => setEditandoId(null)}
                        className="text-[var(--color-terciario)] underline hover:text-[var(--color-apoyo)]"
                      >
                        Cancelar
                      </button>
                    </>
                  ) : (
                    <button
                      type="button"
                      onClick={() => {
                        setEditandoId(unidadMedida.id)
                        setNombreEditado(unidadMedida.nombre)
                      }}
                      className="text-[var(--color-apoyo)] underline hover:text-slate-900 dark:text-slate-300"
                    >
                      Editar
                    </button>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        </div>
      )}
    </div>
  )
}
