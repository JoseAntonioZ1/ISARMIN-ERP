import { zodResolver } from '@hookform/resolvers/zod'
import { useForm } from 'react-hook-form'
import { z } from 'zod'

const esquemaCerrarCaja = z.object({
  montoFisicoDeclarado: z
    .string()
    .min(1, 'El monto contado es obligatorio.')
    .refine((v) => !Number.isNaN(Number(v)) && Number(v) >= 0, 'Debe ser un número mayor o igual a 0.'),
})

type FormularioCerrarCaja = z.infer<typeof esquemaCerrarCaja>

interface CerrarCajaDialogProps {
  onGuardar: (montoFisicoDeclarado: number) => void
  onCancelar: () => void
  guardando?: boolean
}

export function CerrarCajaDialog({ onGuardar, onCancelar, guardando }: CerrarCajaDialogProps) {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FormularioCerrarCaja>({
    resolver: zodResolver(esquemaCerrarCaja),
    defaultValues: { montoFisicoDeclarado: '' },
  })

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
      <form
        onSubmit={handleSubmit((datos) => onGuardar(Number(datos.montoFisicoDeclarado)))}
        className="w-full max-w-sm rounded-lg bg-white p-6 shadow-lg dark:bg-slate-800"
      >
        <h2 className="mb-1 text-lg font-semibold text-slate-800 dark:text-slate-100">Cerrar caja</h2>
        <p className="mb-4 text-sm text-slate-500 dark:text-slate-400">
          Cuenta el dinero físico en caja y registra el monto exacto.
        </p>

        <label className="mb-1 block text-sm font-medium text-slate-700 dark:text-slate-300">
          Monto físico contado
        </label>
        <input
          type="number"
          step="0.01"
          {...register('montoFisicoDeclarado')}
          className="mb-1 w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />
        {errors.montoFisicoDeclarado && (
          <p className="mb-2 text-sm text-red-600">{errors.montoFisicoDeclarado.message}</p>
        )}

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
            {guardando ? 'Cerrando...' : 'Cerrar caja'}
          </button>
        </div>
      </form>
    </div>
  )
}
