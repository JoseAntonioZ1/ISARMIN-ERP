import { zodResolver } from '@hookform/resolvers/zod'
import { useForm } from 'react-hook-form'
import { z } from 'zod'
import type { Cliente } from '@/modules/clientes/api/clientesApi'

const esquemaRecepcion = z.object({
  clienteId: z.string().min(1, 'Selecciona un cliente.'),
  equipoDescripcion: z.string().min(1, 'La descripción del equipo es obligatoria.').max(255),
  fallaReportada: z.string().min(1, 'La falla reportada es obligatoria.'),
})

type FormularioRecepcion = z.infer<typeof esquemaRecepcion>

interface RegistrarRecepcionDialogProps {
  clientes: Cliente[]
  onGuardar: (datos: FormularioRecepcion) => void
  onCancelar: () => void
  guardando?: boolean
}

export function RegistrarRecepcionDialog({ clientes, onGuardar, onCancelar, guardando }: RegistrarRecepcionDialogProps) {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FormularioRecepcion>({
    resolver: zodResolver(esquemaRecepcion),
    defaultValues: { clienteId: '', equipoDescripcion: '', fallaReportada: '' },
  })

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
      <form
        onSubmit={handleSubmit(onGuardar)}
        className="w-full max-w-md rounded-lg bg-white p-6 shadow-lg dark:bg-slate-800"
      >
        <h2 className="mb-4 text-lg font-semibold text-slate-800 dark:text-slate-100">Registrar recepción de equipo</h2>

        <label className="mb-1 block text-sm font-medium text-slate-700 dark:text-slate-300">Cliente</label>
        <select
          {...register('clienteId')}
          className="mb-1 w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        >
          <option value="">Seleccionar...</option>
          {clientes.map((c) => (
            <option key={c.id} value={c.id}>
              {c.nombreRazonSocial}
            </option>
          ))}
        </select>
        {errors.clienteId && <p className="mb-2 text-sm text-red-600">{errors.clienteId.message}</p>}

        <label className="mb-1 mt-3 block text-sm font-medium text-slate-700 dark:text-slate-300">
          Descripción del equipo
        </label>
        <input
          {...register('equipoDescripcion')}
          placeholder="Taladro Bosch 1/2"
          className="mb-1 w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />
        {errors.equipoDescripcion && <p className="mb-2 text-sm text-red-600">{errors.equipoDescripcion.message}</p>}

        <label className="mb-1 mt-3 block text-sm font-medium text-slate-700 dark:text-slate-300">
          Falla reportada
        </label>
        <textarea
          {...register('fallaReportada')}
          rows={3}
          className="mb-1 w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />
        {errors.fallaReportada && <p className="mb-2 text-sm text-red-600">{errors.fallaReportada.message}</p>}

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
            {guardando ? 'Guardando...' : 'Registrar'}
          </button>
        </div>
      </form>
    </div>
  )
}
