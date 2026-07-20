import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useState } from 'react'
import { mediosPagoApi } from '@/modules/catalogos/api/catalogosApi'
import { ApiError } from '@/shared/api/httpClient'

export function MediosPagoPage() {
  const queryClient = useQueryClient()
  const [nombreNuevo, setNombreNuevo] = useState('')
  const [error, setError] = useState<string | null>(null)

  const { data: mediosPago, isLoading } = useQuery({
    queryKey: ['medios-pago'],
    queryFn: mediosPagoApi.listar,
  })

  const invalidar = () => queryClient.invalidateQueries({ queryKey: ['medios-pago'] })

  const mutacionCrear = useMutation({
    mutationFn: mediosPagoApi.crear,
    onSuccess: () => {
      invalidar()
      setNombreNuevo('')
    },
    onError: (e) => setError(e instanceof ApiError ? e.message : 'No se pudo crear el medio de pago.'),
  })

  const mutacionCambiarEstado = useMutation({
    mutationFn: ({ id, activo }: { id: string; activo: boolean }) => mediosPagoApi.cambiarEstado(id, activo),
    onSuccess: invalidar,
    onError: (e) => setError(e instanceof ApiError ? e.message : 'No se pudo cambiar el estado.'),
  })

  const handleCrear = () => {
    if (!nombreNuevo.trim()) return
    setError(null)
    mutacionCrear.mutate(nombreNuevo.trim())
  }

  return (
    <div>
      <h1 className="mb-4 text-xl font-semibold text-slate-800 dark:text-slate-100">Medios de Pago</h1>

      {error && <p className="mb-4 text-sm text-red-600">{error}</p>}

      <div className="mb-4 flex gap-2">
        <input
          value={nombreNuevo}
          onChange={(e) => setNombreNuevo(e.target.value)}
          placeholder="Nombre del nuevo medio de pago (ej. Tarjeta de crédito)"
          className="flex-1 rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />
        <button
          type="button"
          onClick={handleCrear}
          disabled={mutacionCrear.isPending}
          className="rounded bg-[var(--color-acento)] px-4 py-2 text-sm text-white hover:brightness-90 disabled:opacity-50 dark:bg-[var(--color-acento)] dark:hover:brightness-110"
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
              <th className="px-4 py-2">Estado</th>
              <th className="px-4 py-2">Acciones</th>
            </tr>
          </thead>
          <tbody>
            {mediosPago?.map((medioPago) => (
              <tr key={medioPago.id} className="border-t border-slate-200 dark:border-slate-700">
                <td className="px-4 py-2">{medioPago.nombre}</td>
                <td className="px-4 py-2">{medioPago.activo ? 'Activo' : 'Inactivo'}</td>
                <td className="px-4 py-2">
                  <button
                    type="button"
                    onClick={() => mutacionCambiarEstado.mutate({ id: medioPago.id, activo: !medioPago.activo })}
                    className="text-red-600 underline hover:text-red-800"
                  >
                    {medioPago.activo ? 'Desactivar' : 'Activar'}
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  )
}
