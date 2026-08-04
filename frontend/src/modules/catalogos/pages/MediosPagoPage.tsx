import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useEffect, useState } from 'react'
import { type MedioPago, mediosPagoApi } from '@/modules/catalogos/api/catalogosApi'
import { ApiError } from '@/shared/api/httpClient'
import { ConfirmDialog } from '@/shared/components/ConfirmDialog'
import { EstadoBadge } from '@/shared/components/EstadoBadge'
import { EstadoCarga } from '@/shared/components/EstadoCarga'
import { EstadoVacio } from '@/shared/components/EstadoVacio'
import { useToast } from '@/shared/hooks/useToast'

export function MediosPagoPage() {
  const queryClient = useQueryClient()
  const { mostrarExito, mostrarError } = useToast()
  const [nombreNuevo, setNombreNuevo] = useState('')
  const [cambiandoEstado, setCambiandoEstado] = useState<MedioPago | null>(null)

  const {
    data: mediosPago,
    isLoading,
    error: errorListado,
  } = useQuery({
    queryKey: ['medios-pago'],
    queryFn: mediosPagoApi.listar,
  })

  useEffect(() => {
    if (errorListado) mostrarError('No se pudo cargar la lista de medios de pago.')
  }, [errorListado, mostrarError])

  const invalidar = () => queryClient.invalidateQueries({ queryKey: ['medios-pago'] })

  const mutacionCrear = useMutation({
    mutationFn: mediosPagoApi.crear,
    onSuccess: () => {
      invalidar()
      setNombreNuevo('')
      mostrarExito('Medio de pago creado correctamente.')
    },
    onError: (e) => mostrarError(e instanceof ApiError ? e.message : 'No se pudo crear el medio de pago.'),
  })

  const mutacionCambiarEstado = useMutation({
    mutationFn: ({ id, activo }: { id: string; activo: boolean }) => mediosPagoApi.cambiarEstado(id, activo),
    onSuccess: () => {
      invalidar()
      setCambiandoEstado(null)
      mostrarExito('Estado del medio de pago actualizado.')
    },
    onError: (e) => mostrarError(e instanceof ApiError ? e.message : 'No se pudo cambiar el estado.'),
  })

  const handleCrear = () => {
    if (!nombreNuevo.trim()) return
    mutacionCrear.mutate(nombreNuevo.trim())
  }

  return (
    <div>
      <h1 className="mb-4 text-xl font-semibold text-slate-800 dark:text-slate-100">Medios de Pago</h1>

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
          className="rounded bg-[var(--color-principal)] px-4 py-2 text-sm text-white hover:brightness-90 disabled:opacity-50 dark:bg-[var(--color-principal)] dark:hover:brightness-110"
        >
          Agregar
        </button>
      </div>

      {isLoading ? (
        <EstadoCarga />
      ) : mediosPago?.length === 0 ? (
        <EstadoVacio mensaje="No hay medios de pago registrados." />
      ) : (
        <div className="overflow-x-auto">
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
                <td className="px-4 py-2">
                  <EstadoBadge estado={medioPago.activo ? 'Activo' : 'Inactivo'} />
                </td>
                <td className="px-4 py-2">
                  <button
                    type="button"
                    onClick={() => setCambiandoEstado(medioPago)}
                    className="text-red-600 underline hover:text-red-800"
                  >
                    {medioPago.activo ? 'Desactivar' : 'Activar'}
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        </div>
      )}

      {cambiandoEstado && (
        <ConfirmDialog
          titulo={cambiandoEstado.activo ? 'Desactivar medio de pago' : 'Activar medio de pago'}
          mensaje={`¿Confirmas ${cambiandoEstado.activo ? 'desactivar' : 'activar'} "${cambiandoEstado.nombre}"?`}
          confirmando={mutacionCambiarEstado.isPending}
          onConfirmar={() => mutacionCambiarEstado.mutate({ id: cambiandoEstado.id, activo: !cambiandoEstado.activo })}
          onCancelar={() => setCambiandoEstado(null)}
        />
      )}
    </div>
  )
}
