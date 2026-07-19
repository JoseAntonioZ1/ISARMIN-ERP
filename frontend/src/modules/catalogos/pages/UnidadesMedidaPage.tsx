import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useState } from 'react'
import { unidadesMedidaApi } from '@/modules/catalogos/api/catalogosApi'
import { ApiError } from '@/shared/api/httpClient'

export function UnidadesMedidaPage() {
  const queryClient = useQueryClient()
  const [nombreNuevo, setNombreNuevo] = useState('')
  const [editandoId, setEditandoId] = useState<string | null>(null)
  const [nombreEditado, setNombreEditado] = useState('')
  const [error, setError] = useState<string | null>(null)

  const { data: unidadesMedida, isLoading } = useQuery({
    queryKey: ['unidades-medida'],
    queryFn: unidadesMedidaApi.listar,
  })

  const invalidar = () => queryClient.invalidateQueries({ queryKey: ['unidades-medida'] })

  const mutacionCrear = useMutation({
    mutationFn: unidadesMedidaApi.crear,
    onSuccess: () => {
      invalidar()
      setNombreNuevo('')
    },
    onError: (e) => setError(e instanceof ApiError ? e.message : 'No se pudo crear la unidad de medida.'),
  })

  const mutacionEditar = useMutation({
    mutationFn: ({ id, nombre }: { id: string; nombre: string }) => unidadesMedidaApi.editar(id, nombre),
    onSuccess: () => {
      invalidar()
      setEditandoId(null)
    },
    onError: (e) => setError(e instanceof ApiError ? e.message : 'No se pudo editar la unidad de medida.'),
  })

  const handleCrear = () => {
    if (!nombreNuevo.trim()) return
    setError(null)
    mutacionCrear.mutate(nombreNuevo.trim())
  }

  return (
    <div>
      <h1 className="mb-4 text-xl font-semibold text-slate-800 dark:text-slate-100">Unidades de Medida</h1>

      {error && <p className="mb-4 text-sm text-red-600">{error}</p>}

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
          className="rounded bg-slate-800 px-4 py-2 text-sm text-white hover:bg-slate-700 disabled:opacity-50 dark:bg-slate-600 dark:hover:bg-slate-500"
        >
          Agregar
        </button>
      </div>

      {isLoading ? (
        <p className="text-slate-500">Cargando...</p>
      ) : (
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
                        onClick={() => {
                          setError(null)
                          mutacionEditar.mutate({ id: unidadMedida.id, nombre: nombreEditado.trim() })
                        }}
                        disabled={mutacionEditar.isPending}
                        className="mr-3 text-slate-700 underline hover:text-slate-900 dark:text-slate-300"
                      >
                        Guardar
                      </button>
                      <button
                        type="button"
                        onClick={() => setEditandoId(null)}
                        className="text-slate-500 underline hover:text-slate-700"
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
                      className="text-slate-700 underline hover:text-slate-900 dark:text-slate-300"
                    >
                      Editar
                    </button>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  )
}
