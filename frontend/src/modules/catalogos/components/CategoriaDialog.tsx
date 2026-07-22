import { zodResolver } from '@hookform/resolvers/zod'
import { useForm } from 'react-hook-form'
import { z } from 'zod'
import type { Categoria } from '@/modules/catalogos/api/catalogosApi'

const esquemaCategoria = z.object({
  nombre: z.string().min(1, 'El nombre es obligatorio.').max(100),
  categoriaPadreId: z.string(),
})

export type FormularioCategoria = z.infer<typeof esquemaCategoria>

interface CategoriaDialogProps {
  categoria?: Categoria
  categorias: Categoria[]
  onGuardar: (datos: { nombre: string; categoriaPadreId: string | null }) => void
  onCancelar: () => void
  guardando?: boolean
}

export function CategoriaDialog({ categoria, categorias, onGuardar, onCancelar, guardando }: CategoriaDialogProps) {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FormularioCategoria>({
    resolver: zodResolver(esquemaCategoria),
    defaultValues: {
      nombre: categoria?.nombre ?? '',
      categoriaPadreId: categoria?.categoriaPadreId ?? '',
    },
  })

  const opcionesPadre = categorias.filter((c) => c.id !== categoria?.id)

  const onSubmit = (datos: FormularioCategoria) =>
    onGuardar({ nombre: datos.nombre, categoriaPadreId: datos.categoriaPadreId || null })

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
      <form
        onSubmit={handleSubmit(onSubmit)}
        className="w-full max-w-md rounded-lg bg-white p-6 shadow-lg dark:bg-slate-800"
      >
        <h2 className="mb-4 text-lg font-semibold text-slate-800 dark:text-slate-100">
          {categoria ? 'Editar categoría' : 'Nueva categoría'}
        </h2>

        <label className="mb-1 block text-sm font-medium text-[var(--color-apoyo)] dark:text-slate-300">Nombre</label>
        <input
          {...register('nombre')}
          className="mb-1 w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />
        {errors.nombre && <p className="mb-2 text-sm text-red-600">{errors.nombre.message}</p>}

        <label className="mb-1 mt-3 block text-sm font-medium text-[var(--color-apoyo)] dark:text-slate-300">
          Categoría padre (opcional)
        </label>
        <select
          {...register('categoriaPadreId')}
          className="mb-1 w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        >
          <option value="">Ninguna</option>
          {opcionesPadre.map((c) => (
            <option key={c.id} value={c.id}>
              {c.nombre}
            </option>
          ))}
        </select>

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
