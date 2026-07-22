import { zodResolver } from '@hookform/resolvers/zod'
import { useForm } from 'react-hook-form'
import { z } from 'zod'
import type { Usuario } from '@/modules/usuarios/api/usuariosApi'
import { SelectorRoles } from '@/modules/usuarios/components/SelectorRoles'

const esquemaEditarUsuario = z.object({
  nombre: z.string().min(1, 'El nombre es obligatorio.').max(150),
  rolIds: z.array(z.string()).min(1, 'Debe asignarse al menos un rol.'),
})

export type FormularioEditarUsuario = z.infer<typeof esquemaEditarUsuario>

interface EditarUsuarioDialogProps {
  usuario: Usuario
  onGuardar: (datos: FormularioEditarUsuario) => void
  onCancelar: () => void
  guardando?: boolean
}

export function EditarUsuarioDialog({ usuario, onGuardar, onCancelar, guardando }: EditarUsuarioDialogProps) {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FormularioEditarUsuario>({
    resolver: zodResolver(esquemaEditarUsuario),
    defaultValues: {
      nombre: usuario.nombre,
      rolIds: usuario.roles.map((r) => r.id),
    },
  })

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
      <form
        onSubmit={handleSubmit(onGuardar)}
        className="w-full max-w-md rounded-lg bg-white p-6 shadow-lg dark:bg-slate-800"
      >
        <h2 className="mb-4 text-lg font-semibold text-slate-800 dark:text-slate-100">Editar usuario</h2>

        <label className="mb-1 block text-sm font-medium text-[var(--color-apoyo)] dark:text-slate-300">Nombre completo</label>
        <input
          {...register('nombre')}
          className="mb-1 w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />
        {errors.nombre && <p className="mb-2 text-sm text-red-600">{errors.nombre.message}</p>}

        <p className="mt-3 text-sm text-[var(--color-terciario)] dark:text-slate-400">
          Usuario (login): <span className="font-medium">{usuario.nombreUsuario}</span> — no editable aquí.
        </p>

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
            className="rounded bg-[var(--color-principal)] px-4 py-2 text-sm text-white hover:brightness-90 disabled:opacity-50 dark:bg-[var(--color-principal)] dark:hover:brightness-110"
          >
            {guardando ? 'Guardando...' : 'Guardar'}
          </button>
        </div>
      </form>
    </div>
  )
}
