import { zodResolver } from '@hookform/resolvers/zod'
import { useForm } from 'react-hook-form'
import { z } from 'zod'
import { TIPOS_DOCUMENTO, type Cliente } from '@/modules/clientes/api/clientesApi'

const esquemaCliente = z
  .object({
    nombreRazonSocial: z.string().min(1, 'El nombre o razón social es obligatorio.').max(200),
    telefono: z.string().min(1, 'El teléfono es obligatorio.').max(30),
    direccion: z.string().max(255).optional(),
    tipoDocumento: z.string().optional(),
    numeroDocumento: z.string().max(20).optional(),
  })
  .refine((datos) => !!datos.tipoDocumento === !!datos.numeroDocumento, {
    message: 'El tipo y el número de documento deben proporcionarse juntos.',
    path: ['numeroDocumento'],
  })

export type FormularioCliente = z.infer<typeof esquemaCliente>

interface ClienteDialogProps {
  cliente?: Cliente
  onGuardar: (datos: FormularioCliente) => void
  onCancelar: () => void
  guardando?: boolean
}

const ETIQUETAS_TIPO_DOCUMENTO: Record<string, string> = {
  Dni: 'DNI',
  Ruc: 'RUC',
  CarneExtranjeria: 'Carné de Extranjería',
  Pasaporte: 'Pasaporte',
}

export function ClienteDialog({ cliente, onGuardar, onCancelar, guardando }: ClienteDialogProps) {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FormularioCliente>({
    resolver: zodResolver(esquemaCliente),
    defaultValues: {
      nombreRazonSocial: cliente?.nombreRazonSocial ?? '',
      telefono: cliente?.telefono ?? '',
      direccion: cliente?.direccion ?? '',
      tipoDocumento: cliente?.tipoDocumento ?? '',
      numeroDocumento: cliente?.numeroDocumento ?? '',
    },
  })

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
      <form
        onSubmit={handleSubmit(onGuardar)}
        className="w-full max-w-md rounded-lg bg-white p-6 shadow-lg dark:bg-slate-800"
      >
        <h2 className="mb-4 text-lg font-semibold text-slate-800 dark:text-slate-100">
          {cliente ? 'Editar cliente' : 'Nuevo cliente'}
        </h2>

        <label className="mb-1 block text-sm font-medium text-[var(--color-apoyo)] dark:text-slate-300">
          Nombre / Razón social
        </label>
        <input
          {...register('nombreRazonSocial')}
          className="mb-1 w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />
        {errors.nombreRazonSocial && <p className="mb-2 text-sm text-red-600">{errors.nombreRazonSocial.message}</p>}

        <label className="mb-1 mt-3 block text-sm font-medium text-[var(--color-apoyo)] dark:text-slate-300">Teléfono</label>
        <input
          {...register('telefono')}
          className="mb-1 w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />
        {errors.telefono && <p className="mb-2 text-sm text-red-600">{errors.telefono.message}</p>}

        <label className="mb-1 mt-3 block text-sm font-medium text-[var(--color-apoyo)] dark:text-slate-300">
          Dirección (opcional)
        </label>
        <input
          {...register('direccion')}
          className="mb-1 w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />

        <div className="mt-3 flex gap-3">
          <div className="flex-1">
            <label className="mb-1 block text-sm font-medium text-[var(--color-apoyo)] dark:text-slate-300">
              Tipo de documento (opcional)
            </label>
            <select
              {...register('tipoDocumento')}
              className="w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
            >
              <option value="">Ninguno</option>
              {TIPOS_DOCUMENTO.map((tipo) => (
                <option key={tipo} value={tipo}>
                  {ETIQUETAS_TIPO_DOCUMENTO[tipo]}
                </option>
              ))}
            </select>
          </div>
          <div className="flex-1">
            <label className="mb-1 block text-sm font-medium text-[var(--color-apoyo)] dark:text-slate-300">Número</label>
            <input
              {...register('numeroDocumento')}
              className="w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
            />
          </div>
        </div>
        {errors.numeroDocumento && <p className="mb-2 mt-1 text-sm text-red-600">{errors.numeroDocumento.message}</p>}

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
