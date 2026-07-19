import { zodResolver } from '@hookform/resolvers/zod'
import { useForm } from 'react-hook-form'
import { z } from 'zod'
import type { Proveedor } from '@/modules/proveedores/api/proveedoresApi'

const esquemaProveedor = z.object({
  nombreRazonSocial: z.string().min(1, 'El nombre o razón social es obligatorio.').max(200),
  documento: z.string().max(20).optional(),
  telefono: z.string().max(30).optional(),
  direccion: z.string().max(255).optional(),
})

export type FormularioProveedor = z.infer<typeof esquemaProveedor>

interface ProveedorDialogProps {
  proveedor?: Proveedor
  onGuardar: (datos: FormularioProveedor) => void
  onCancelar: () => void
  guardando?: boolean
}

export function ProveedorDialog({ proveedor, onGuardar, onCancelar, guardando }: ProveedorDialogProps) {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FormularioProveedor>({
    resolver: zodResolver(esquemaProveedor),
    defaultValues: {
      nombreRazonSocial: proveedor?.nombreRazonSocial ?? '',
      documento: proveedor?.documento ?? '',
      telefono: proveedor?.telefono ?? '',
      direccion: proveedor?.direccion ?? '',
    },
  })

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
      <form
        onSubmit={handleSubmit(onGuardar)}
        className="w-full max-w-md rounded-lg bg-white p-6 shadow-lg dark:bg-slate-800"
      >
        <h2 className="mb-4 text-lg font-semibold text-slate-800 dark:text-slate-100">
          {proveedor ? 'Editar proveedor' : 'Nuevo proveedor'}
        </h2>

        <label className="mb-1 block text-sm font-medium text-slate-700 dark:text-slate-300">
          Nombre / Razón social
        </label>
        <input
          {...register('nombreRazonSocial')}
          className="mb-1 w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />
        {errors.nombreRazonSocial && <p className="mb-2 text-sm text-red-600">{errors.nombreRazonSocial.message}</p>}

        <label className="mb-1 mt-3 block text-sm font-medium text-slate-700 dark:text-slate-300">
          Documento (opcional)
        </label>
        <input
          {...register('documento')}
          className="mb-1 w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />

        <label className="mb-1 mt-3 block text-sm font-medium text-slate-700 dark:text-slate-300">
          Teléfono (opcional)
        </label>
        <input
          {...register('telefono')}
          className="mb-1 w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />

        <label className="mb-1 mt-3 block text-sm font-medium text-slate-700 dark:text-slate-300">
          Dirección (opcional)
        </label>
        <input
          {...register('direccion')}
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
            className="rounded bg-slate-800 px-4 py-2 text-sm text-white hover:bg-slate-700 disabled:opacity-50 dark:bg-slate-600 dark:hover:bg-slate-500"
          >
            {guardando ? 'Guardando...' : 'Guardar'}
          </button>
        </div>
      </form>
    </div>
  )
}
