import { zodResolver } from '@hookform/resolvers/zod'
import { useForm } from 'react-hook-form'
import { z } from 'zod'
import type { Producto } from '@/modules/productos/api/productosApi'

const esquemaAjuste = z.object({
  cantidadAjuste: z
    .string()
    .min(1, 'La cantidad de ajuste es obligatoria.')
    .refine((v) => !Number.isNaN(Number(v)), 'Debe ser un número.')
    .refine((v) => Number(v) !== 0, 'El ajuste debe ser distinto de cero.'),
  motivo: z.string().min(1, 'El motivo es obligatorio.').max(500),
})

type FormularioAjuste = z.infer<typeof esquemaAjuste>

interface AjustarInventarioDialogProps {
  producto: Producto
  onGuardar: (datos: { cantidadAjuste: number; motivo: string }) => void
  onCancelar: () => void
  guardando?: boolean
}

export function AjustarInventarioDialog({ producto, onGuardar, onCancelar, guardando }: AjustarInventarioDialogProps) {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FormularioAjuste>({
    resolver: zodResolver(esquemaAjuste),
    defaultValues: { cantidadAjuste: '', motivo: '' },
  })

  const onSubmit = (datos: FormularioAjuste) =>
    onGuardar({ cantidadAjuste: Number(datos.cantidadAjuste), motivo: datos.motivo })

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
      <form
        onSubmit={handleSubmit(onSubmit)}
        className="w-full max-w-md rounded-lg bg-white p-6 shadow-lg dark:bg-slate-800"
      >
        <h2 className="mb-1 text-lg font-semibold text-slate-800 dark:text-slate-100">Ajustar inventario</h2>
        <p className="mb-4 text-sm text-[var(--color-terciario)] dark:text-slate-400">
          {producto.nombre} — stock actual: {producto.stockActual}
        </p>

        <label className="mb-1 block text-sm font-medium text-[var(--color-apoyo)] dark:text-slate-300">
          Cantidad de ajuste (positivo suma, negativo resta)
        </label>
        <input
          type="number"
          step="0.001"
          {...register('cantidadAjuste')}
          className="mb-1 w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />
        {errors.cantidadAjuste && <p className="mb-2 text-sm text-red-600">{errors.cantidadAjuste.message}</p>}

        <label className="mb-1 mt-3 block text-sm font-medium text-[var(--color-apoyo)] dark:text-slate-300">Motivo</label>
        <textarea
          {...register('motivo')}
          rows={3}
          className="mb-1 w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />
        {errors.motivo && <p className="mb-2 text-sm text-red-600">{errors.motivo.message}</p>}

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
            className="rounded bg-[var(--color-principal)] px-4 py-2 text-sm text-white hover:brightness-90 disabled:opacity-50 dark:bg-[var(--color-principal)] dark:hover:brightness-110"
          >
            {guardando ? 'Guardando...' : 'Ajustar'}
          </button>
        </div>
      </form>
    </div>
  )
}
