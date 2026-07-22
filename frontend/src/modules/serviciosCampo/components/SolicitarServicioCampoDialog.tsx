import { zodResolver } from '@hookform/resolvers/zod'
import { useForm } from 'react-hook-form'
import { z } from 'zod'
import type { Cliente } from '@/modules/clientes/api/clientesApi'
import type { Usuario } from '@/modules/usuarios/api/usuariosApi'

const esquemaSolicitud = z.object({
  clienteId: z.string().min(1, 'Selecciona un cliente.'),
  descripcionTrabajo: z.string().min(1, 'La descripción del trabajo es obligatoria.'),
  tecnicoAsignadoId: z.string(),
})

type FormularioSolicitud = z.infer<typeof esquemaSolicitud>

interface SolicitarServicioCampoDialogProps {
  clientes: Cliente[]
  usuarios: Usuario[]
  onGuardar: (datos: { clienteId: string; descripcionTrabajo: string; tecnicoAsignadoId: string | null }) => void
  onCancelar: () => void
  guardando?: boolean
}

export function SolicitarServicioCampoDialog({
  clientes,
  usuarios,
  onGuardar,
  onCancelar,
  guardando,
}: SolicitarServicioCampoDialogProps) {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FormularioSolicitud>({
    resolver: zodResolver(esquemaSolicitud),
    defaultValues: { clienteId: '', descripcionTrabajo: '', tecnicoAsignadoId: '' },
  })

  const enviar = (datos: FormularioSolicitud) =>
    onGuardar({
      clienteId: datos.clienteId,
      descripcionTrabajo: datos.descripcionTrabajo,
      tecnicoAsignadoId: datos.tecnicoAsignadoId || null,
    })

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
      <form onSubmit={handleSubmit(enviar)} className="w-full max-w-md rounded-lg bg-white p-6 shadow-lg dark:bg-slate-800">
        <h2 className="mb-4 text-lg font-semibold text-slate-800 dark:text-slate-100">Solicitar servicio de campo</h2>

        <label className="mb-1 block text-sm font-medium text-[var(--color-apoyo)] dark:text-slate-300">Cliente</label>
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

        <label className="mb-1 mt-3 block text-sm font-medium text-[var(--color-apoyo)] dark:text-slate-300">
          Descripción del trabajo
        </label>
        <textarea
          {...register('descripcionTrabajo')}
          rows={3}
          className="mb-1 w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />
        {errors.descripcionTrabajo && <p className="mb-2 text-sm text-red-600">{errors.descripcionTrabajo.message}</p>}

        <label className="mb-1 mt-3 block text-sm font-medium text-[var(--color-apoyo)] dark:text-slate-300">
          Técnico asignado (opcional)
        </label>
        <select
          {...register('tecnicoAsignadoId')}
          className="mb-1 w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        >
          <option value="">Sin asignar</option>
          {usuarios.map((u) => (
            <option key={u.id} value={u.id}>
              {u.nombre}
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
            {guardando ? 'Guardando...' : 'Solicitar'}
          </button>
        </div>
      </form>
    </div>
  )
}
