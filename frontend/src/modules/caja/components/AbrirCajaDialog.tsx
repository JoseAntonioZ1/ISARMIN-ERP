import { zodResolver } from '@hookform/resolvers/zod'
import { useQuery } from '@tanstack/react-query'
import { useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { z } from 'zod'
import { configuracionEmpresaApi } from '@/modules/catalogos/api/catalogosApi'

const esquemaAbrirCaja = z.object({
  montoApertura: z
    .string()
    .min(1, 'El monto de apertura es obligatorio.')
    .refine((v) => !Number.isNaN(Number(v)) && Number(v) >= 0, 'Debe ser un número mayor o igual a 0.'),
})

type FormularioAbrirCaja = z.infer<typeof esquemaAbrirCaja>

interface AbrirCajaDialogProps {
  onGuardar: (montoApertura: number) => void
  onCancelar: () => void
  guardando?: boolean
}

export function AbrirCajaDialog({ onGuardar, onCancelar, guardando }: AbrirCajaDialogProps) {
  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors },
  } = useForm<FormularioAbrirCaja>({
    resolver: zodResolver(esquemaAbrirCaja),
    defaultValues: { montoApertura: '' },
  })

  const { data: configuracion } = useQuery({
    queryKey: ['configuracion-empresa'],
    queryFn: configuracionEmpresaApi.obtener,
  })

  useEffect(() => {
    if (configuracion?.montoAperturaCajaPredeterminado != null) {
      setValue('montoApertura', String(configuracion.montoAperturaCajaPredeterminado))
    }
  }, [configuracion, setValue])

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
      <form
        onSubmit={handleSubmit((datos) => onGuardar(Number(datos.montoApertura)))}
        className="w-full max-w-sm rounded-lg bg-white p-6 shadow-lg dark:bg-slate-800"
      >
        <h2 className="mb-4 text-lg font-semibold text-slate-800 dark:text-slate-100">Abrir caja</h2>

        <label className="mb-1 block text-sm font-medium text-slate-700 dark:text-slate-300">
          Monto inicial de apertura
        </label>
        <input
          type="number"
          step="0.01"
          {...register('montoApertura')}
          className="mb-1 w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />
        {errors.montoApertura && <p className="mb-2 text-sm text-red-600">{errors.montoApertura.message}</p>}

        <div className="mt-5 flex justify-end gap-3">
          <button
            type="button"
            onClick={onCancelar}
            className="rounded border border-slate-300 px-4 py-2 text-sm hover:bg-slate-100 dark:border-slate-600 dark:hover:bg-slate-700"
          >
            Cancelar
          </button>
          <button
            type="submit"
            disabled={guardando}
            className="rounded bg-slate-800 px-4 py-2 text-sm text-white hover:bg-slate-700 disabled:opacity-50 dark:bg-slate-600 dark:hover:bg-slate-500"
          >
            {guardando ? 'Abriendo...' : 'Abrir caja'}
          </button>
        </div>
      </form>
    </div>
  )
}
