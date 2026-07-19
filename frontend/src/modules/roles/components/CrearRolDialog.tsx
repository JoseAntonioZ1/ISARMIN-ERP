import { zodResolver } from '@hookform/resolvers/zod'
import { useForm } from 'react-hook-form'
import { z } from 'zod'

const esquemaCrearRol = z.object({
  nombre: z.string().min(1, 'El nombre es obligatorio.').max(100),
  descripcion: z.string().max(500).optional(),
})

export type FormularioCrearRol = z.infer<typeof esquemaCrearRol>

interface CrearRolDialogProps {
  onGuardar: (datos: FormularioCrearRol) => void
  onCancelar: () => void
  guardando?: boolean
}

export function CrearRolDialog({ onGuardar, onCancelar, guardando }: CrearRolDialogProps) {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FormularioCrearRol>({ resolver: zodResolver(esquemaCrearRol) })

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
      <form
        onSubmit={handleSubmit(onGuardar)}
        className="w-full max-w-md rounded-lg bg-white p-6 shadow-lg dark:bg-slate-800"
      >
        <h2 className="mb-4 text-lg font-semibold text-slate-800 dark:text-slate-100">Nuevo rol</h2>

        <label className="mb-1 block text-sm font-medium text-slate-700 dark:text-slate-300">Nombre</label>
        <input
          {...register('nombre')}
          className="mb-1 w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />
        {errors.nombre && <p className="mb-2 text-sm text-red-600">{errors.nombre.message}</p>}

        <label className="mb-1 mt-3 block text-sm font-medium text-slate-700 dark:text-slate-300">
          Descripción (opcional)
        </label>
        <textarea
          {...register('descripcion')}
          rows={2}
          className="mb-1 w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />

        <p className="mt-3 text-sm text-slate-500 dark:text-slate-400">
          Los permisos se asignan después de crear el rol, desde la pantalla de edición.
        </p>

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
            {guardando ? 'Guardando...' : 'Guardar'}
          </button>
        </div>
      </form>
    </div>
  )
}
