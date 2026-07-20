import { zodResolver } from '@hookform/resolvers/zod'
import { useForm } from 'react-hook-form'
import { z } from 'zod'
import { CONCEPTOS_MOVIMIENTO, type ConceptoMovimiento } from '@/modules/caja/api/cajaApi'

const ETIQUETAS_CONCEPTO: Record<ConceptoMovimiento, string> = {
  GastoOperativo: 'Gasto operativo (egreso)',
  RetiroPropietario: 'Retiro del propietario (egreso)',
  AporteCapital: 'Aporte de capital (ingreso)',
}

const esquemaMovimiento = z.object({
  concepto: z.enum(CONCEPTOS_MOVIMIENTO),
  monto: z
    .string()
    .min(1, 'El monto es obligatorio.')
    .refine((v) => !Number.isNaN(Number(v)) && Number(v) > 0, 'Debe ser un número mayor a cero.'),
  descripcion: z.string().max(255).optional(),
})

type FormularioMovimiento = z.infer<typeof esquemaMovimiento>

interface RegistrarMovimientoCajaDialogProps {
  onGuardar: (datos: { concepto: ConceptoMovimiento; monto: number; descripcion: string | null }) => void
  onCancelar: () => void
  guardando?: boolean
}

export function RegistrarMovimientoCajaDialog({ onGuardar, onCancelar, guardando }: RegistrarMovimientoCajaDialogProps) {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FormularioMovimiento>({
    resolver: zodResolver(esquemaMovimiento),
    defaultValues: { concepto: 'GastoOperativo', monto: '', descripcion: '' },
  })

  const onSubmit = (datos: FormularioMovimiento) =>
    onGuardar({ concepto: datos.concepto, monto: Number(datos.monto), descripcion: datos.descripcion || null })

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
      <form
        onSubmit={handleSubmit(onSubmit)}
        className="w-full max-w-sm rounded-lg bg-white p-6 shadow-lg dark:bg-slate-800"
      >
        <h2 className="mb-4 text-lg font-semibold text-slate-800 dark:text-slate-100">Registrar movimiento</h2>

        <label className="mb-1 block text-sm font-medium text-slate-700 dark:text-slate-300">Concepto</label>
        <select
          {...register('concepto')}
          className="mb-1 w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        >
          {CONCEPTOS_MOVIMIENTO.map((concepto) => (
            <option key={concepto} value={concepto}>
              {ETIQUETAS_CONCEPTO[concepto]}
            </option>
          ))}
        </select>

        <label className="mb-1 mt-3 block text-sm font-medium text-slate-700 dark:text-slate-300">Monto</label>
        <input
          type="number"
          step="0.01"
          {...register('monto')}
          className="mb-1 w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />
        {errors.monto && <p className="mb-2 text-sm text-red-600">{errors.monto.message}</p>}

        <label className="mb-1 mt-3 block text-sm font-medium text-slate-700 dark:text-slate-300">
          Descripción (opcional)
        </label>
        <input
          {...register('descripcion')}
          className="mb-1 w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />

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
            className="rounded bg-[var(--color-acento)] px-4 py-2 text-sm text-white hover:brightness-90 disabled:opacity-50 dark:bg-[var(--color-acento)] dark:hover:brightness-110"
          >
            {guardando ? 'Guardando...' : 'Registrar'}
          </button>
        </div>
      </form>
    </div>
  )
}
