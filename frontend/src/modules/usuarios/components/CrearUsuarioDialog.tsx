import { zodResolver } from '@hookform/resolvers/zod'
import { useForm } from 'react-hook-form'
import { z } from 'zod'
import { SelectorRoles } from '@/modules/usuarios/components/SelectorRoles'

const esquemaCrearUsuario = z.object({
  nombre: z.string().min(1, 'El nombre es obligatorio.').max(150),
  nombreUsuario: z.string().min(1, 'El usuario es obligatorio.').max(50),
  credencialInicial: z.string().min(8, 'Mínimo 8 caracteres.'),
  rolIds: z.array(z.string()).min(1, 'Debe asignarse al menos un rol.'),
})

export type FormularioCrearUsuario = z.infer<typeof esquemaCrearUsuario>

interface CrearUsuarioDialogProps {
  onGuardar: (datos: FormularioCrearUsuario) => void
  onCancelar: () => void
  guardando?: boolean
}

export function CrearUsuarioDialog({ onGuardar, onCancelar, guardando }: CrearUsuarioDialogProps) {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FormularioCrearUsuario>({ resolver: zodResolver(esquemaCrearUsuario) })

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
      <form
        onSubmit={handleSubmit(onGuardar)}
        className="w-full max-w-md rounded-lg bg-white p-6 shadow-lg dark:bg-slate-800"
      >
        <h2 className="mb-4 text-lg font-semibold text-slate-800 dark:text-slate-100">Nuevo usuario</h2>

        <label className="mb-1 block text-sm font-medium text-slate-700 dark:text-slate-300">Nombre completo</label>
        <input
          {...register('nombre')}
          className="mb-1 w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />
        {errors.nombre && <p className="mb-2 text-sm text-red-600">{errors.nombre.message}</p>}

        <label className="mb-1 mt-3 block text-sm font-medium text-slate-700 dark:text-slate-300">
          Usuario (login)
        </label>
        <input
          {...register('nombreUsuario')}
          className="mb-1 w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />
        {errors.nombreUsuario && <p className="mb-2 text-sm text-red-600">{errors.nombreUsuario.message}</p>}

        <label className="mb-1 mt-3 block text-sm font-medium text-slate-700 dark:text-slate-300">
          Contraseña inicial
        </label>
        <input
          type="password"
          {...register('credencialInicial')}
          className="mb-1 w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />
        {errors.credencialInicial && <p className="mb-2 text-sm text-red-600">{errors.credencialInicial.message}</p>}

        <SelectorRoles register={register} error={errors.rolIds?.message} />

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
