import { zodResolver } from '@hookform/resolvers/zod'
import { useForm } from 'react-hook-form'
import { z } from 'zod'
import type { Categoria, UnidadMedida } from '@/modules/catalogos/api/catalogosApi'
import type { DatosRegistrarProducto } from '@/modules/productos/api/productosApi'

const numeroObligatorio = (mensaje: string) =>
  z
    .string()
    .min(1, mensaje)
    .refine((v) => !Number.isNaN(Number(v)), 'Debe ser un número.')
    .refine((v) => Number(v) >= 0, 'No puede ser negativo.')

const numeroOpcional = z
  .string()
  .optional()
  .refine((v) => !v || (!Number.isNaN(Number(v)) && Number(v) >= 0), 'Debe ser un número mayor o igual a 0.')

const esquemaCrearProducto = z.object({
  codigoInterno: z.string().min(1, 'El código interno es obligatorio.').max(50),
  nombre: z.string().min(1, 'El nombre es obligatorio.').max(200),
  categoriaId: z.string().min(1, 'La categoría es obligatoria.'),
  unidadMedidaId: z.string().min(1, 'La unidad de medida es obligatoria.'),
  marca: z.string().max(100).optional(),
  costoReferencia: numeroObligatorio('El costo es obligatorio.'),
  precioVenta: numeroObligatorio('El precio es obligatorio.'),
  stockInicial: numeroObligatorio('El stock inicial es obligatorio.'),
  codigoBarras: z.string().max(50).optional(),
  stockMinimo: numeroOpcional,
})

type FormularioCrearProducto = z.infer<typeof esquemaCrearProducto>

interface CrearProductoDialogProps {
  categorias: Categoria[]
  unidadesMedida: UnidadMedida[]
  onGuardar: (datos: DatosRegistrarProducto) => void
  onCancelar: () => void
  guardando?: boolean
}

export function CrearProductoDialog({
  categorias,
  unidadesMedida,
  onGuardar,
  onCancelar,
  guardando,
}: CrearProductoDialogProps) {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FormularioCrearProducto>({
    resolver: zodResolver(esquemaCrearProducto),
    defaultValues: {
      codigoInterno: '',
      nombre: '',
      categoriaId: '',
      unidadMedidaId: '',
      marca: '',
      costoReferencia: '0',
      precioVenta: '0',
      stockInicial: '0',
      codigoBarras: '',
      stockMinimo: '',
    },
  })

  const onSubmit = (datos: FormularioCrearProducto) =>
    onGuardar({
      codigoInterno: datos.codigoInterno,
      nombre: datos.nombre,
      categoriaId: datos.categoriaId,
      unidadMedidaId: datos.unidadMedidaId,
      marca: datos.marca || null,
      costoReferencia: Number(datos.costoReferencia),
      precioVenta: Number(datos.precioVenta),
      stockInicial: Number(datos.stockInicial),
      codigoBarras: datos.codigoBarras || null,
      stockMinimo: datos.stockMinimo ? Number(datos.stockMinimo) : null,
    })

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
      <form
        onSubmit={handleSubmit(onSubmit)}
        className="max-h-[90vh] w-full max-w-lg overflow-y-auto rounded-lg bg-white p-6 shadow-lg dark:bg-slate-800"
      >
        <h2 className="mb-4 text-lg font-semibold text-slate-800 dark:text-slate-100">Nuevo producto</h2>

        <label className="mb-1 block text-sm font-medium text-slate-700 dark:text-slate-300">Código interno</label>
        <input
          {...register('codigoInterno')}
          className="mb-1 w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />
        {errors.codigoInterno && <p className="mb-2 text-sm text-red-600">{errors.codigoInterno.message}</p>}

        <label className="mb-1 mt-3 block text-sm font-medium text-slate-700 dark:text-slate-300">Nombre</label>
        <input
          {...register('nombre')}
          className="mb-1 w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />
        {errors.nombre && <p className="mb-2 text-sm text-red-600">{errors.nombre.message}</p>}

        <div className="mt-3 grid grid-cols-2 gap-3">
          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700 dark:text-slate-300">Categoría</label>
            <select
              {...register('categoriaId')}
              className="w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
            >
              <option value="">Seleccionar...</option>
              {categorias.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.nombre}
                </option>
              ))}
            </select>
            {errors.categoriaId && <p className="mt-1 text-sm text-red-600">{errors.categoriaId.message}</p>}
          </div>

          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700 dark:text-slate-300">
              Unidad de medida
            </label>
            <select
              {...register('unidadMedidaId')}
              className="w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
            >
              <option value="">Seleccionar...</option>
              {unidadesMedida.map((u) => (
                <option key={u.id} value={u.id}>
                  {u.nombre}
                </option>
              ))}
            </select>
            {errors.unidadMedidaId && <p className="mt-1 text-sm text-red-600">{errors.unidadMedidaId.message}</p>}
          </div>
        </div>

        <label className="mb-1 mt-3 block text-sm font-medium text-slate-700 dark:text-slate-300">
          Marca (opcional)
        </label>
        <input
          {...register('marca')}
          className="mb-1 w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />

        <div className="mt-3 grid grid-cols-3 gap-3">
          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700 dark:text-slate-300">
              Costo de adquisición
            </label>
            <input
              type="number"
              step="0.01"
              {...register('costoReferencia')}
              className="w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
            />
            {errors.costoReferencia && <p className="mt-1 text-sm text-red-600">{errors.costoReferencia.message}</p>}
          </div>
          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700 dark:text-slate-300">
              Precio de venta
            </label>
            <input
              type="number"
              step="0.01"
              {...register('precioVenta')}
              className="w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
            />
            {errors.precioVenta && <p className="mt-1 text-sm text-red-600">{errors.precioVenta.message}</p>}
          </div>
          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700 dark:text-slate-300">
              Stock inicial
            </label>
            <input
              type="number"
              step="0.001"
              {...register('stockInicial')}
              className="w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
            />
            {errors.stockInicial && <p className="mt-1 text-sm text-red-600">{errors.stockInicial.message}</p>}
          </div>
        </div>

        <div className="mt-3 grid grid-cols-2 gap-3">
          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700 dark:text-slate-300">
              Código de barras (opcional)
            </label>
            <input
              {...register('codigoBarras')}
              className="w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
            />
          </div>
          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700 dark:text-slate-300">
              Stock mínimo (opcional)
            </label>
            <input
              type="number"
              step="0.001"
              {...register('stockMinimo')}
              className="w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
            />
          </div>
        </div>

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
            {guardando ? 'Guardando...' : 'Guardar'}
          </button>
        </div>
      </form>
    </div>
  )
}
