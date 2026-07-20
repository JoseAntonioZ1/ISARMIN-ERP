import { zodResolver } from '@hookform/resolvers/zod'
import { useFieldArray, useForm } from 'react-hook-form'
import { z } from 'zod'
import type { DatosRegistrarCompra } from '@/modules/compras/api/comprasApi'
import type { Producto } from '@/modules/productos/api/productosApi'
import type { Proveedor } from '@/modules/proveedores/api/proveedoresApi'

const numeroPositivo = (mensaje: string) =>
  z
    .string()
    .min(1, mensaje)
    .refine((v) => !Number.isNaN(Number(v)) && Number(v) > 0, 'Debe ser un número mayor a cero.')

const numeroNoNegativo = (mensaje: string) =>
  z
    .string()
    .min(1, mensaje)
    .refine((v) => !Number.isNaN(Number(v)) && Number(v) >= 0, 'Debe ser un número mayor o igual a 0.')

const esquemaDetalle = z.object({
  productoId: z.string().min(1, 'Selecciona un producto.'),
  cantidad: numeroPositivo('La cantidad es obligatoria.'),
  costoUnitario: numeroNoNegativo('El costo unitario es obligatorio.'),
})

const esquemaRegistrarCompra = z.object({
  proveedorId: z.string().min(1, 'Selecciona un proveedor.'),
  fecha: z.string().min(1, 'La fecha es obligatoria.'),
  documentoCompraTipo: z.string().min(1, 'El tipo de documento es obligatorio.').max(30),
  documentoCompraNumero: z.string().min(1, 'El número de documento es obligatorio.').max(50),
  detalles: z.array(esquemaDetalle).min(1, 'Agrega al menos un producto.'),
})

type FormularioRegistrarCompra = z.infer<typeof esquemaRegistrarCompra>

interface RegistrarCompraDialogProps {
  proveedores: Proveedor[]
  productos: Producto[]
  onGuardar: (datos: DatosRegistrarCompra) => void
  onCancelar: () => void
  guardando?: boolean
}

export function RegistrarCompraDialog({
  proveedores,
  productos,
  onGuardar,
  onCancelar,
  guardando,
}: RegistrarCompraDialogProps) {
  const {
    register,
    control,
    handleSubmit,
    formState: { errors },
  } = useForm<FormularioRegistrarCompra>({
    resolver: zodResolver(esquemaRegistrarCompra),
    defaultValues: {
      proveedorId: '',
      fecha: new Date().toISOString().slice(0, 10),
      documentoCompraTipo: 'Factura',
      documentoCompraNumero: '',
      detalles: [{ productoId: '', cantidad: '', costoUnitario: '' }],
    },
  })

  const { fields, append, remove } = useFieldArray({ control, name: 'detalles' })

  const onSubmit = (datos: FormularioRegistrarCompra) =>
    onGuardar({
      proveedorId: datos.proveedorId,
      fecha: datos.fecha,
      documentoCompraTipo: datos.documentoCompraTipo,
      documentoCompraNumero: datos.documentoCompraNumero,
      detalles: datos.detalles.map((d) => ({
        productoId: d.productoId,
        cantidad: Number(d.cantidad),
        costoUnitario: Number(d.costoUnitario),
      })),
    })

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
      <form
        onSubmit={handleSubmit(onSubmit)}
        className="max-h-[90vh] w-full max-w-2xl overflow-y-auto rounded-lg bg-white p-6 shadow-lg dark:bg-slate-800"
      >
        <h2 className="mb-4 text-lg font-semibold text-slate-800 dark:text-slate-100">Registrar compra</h2>

        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700 dark:text-slate-300">Proveedor</label>
            <select
              {...register('proveedorId')}
              className="w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
            >
              <option value="">Seleccionar...</option>
              {proveedores.map((p) => (
                <option key={p.id} value={p.id}>
                  {p.nombreRazonSocial}
                </option>
              ))}
            </select>
            {errors.proveedorId && <p className="mt-1 text-sm text-red-600">{errors.proveedorId.message}</p>}
          </div>

          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700 dark:text-slate-300">Fecha</label>
            <input
              type="date"
              {...register('fecha')}
              className="w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
            />
            {errors.fecha && <p className="mt-1 text-sm text-red-600">{errors.fecha.message}</p>}
          </div>
        </div>

        <div className="mt-3 grid grid-cols-2 gap-3">
          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700 dark:text-slate-300">
              Tipo de documento
            </label>
            <input
              {...register('documentoCompraTipo')}
              placeholder="Factura, Boleta, Otro..."
              className="w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
            />
            {errors.documentoCompraTipo && (
              <p className="mt-1 text-sm text-red-600">{errors.documentoCompraTipo.message}</p>
            )}
          </div>
          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700 dark:text-slate-300">
              Número de documento
            </label>
            <input
              {...register('documentoCompraNumero')}
              className="w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
            />
            {errors.documentoCompraNumero && (
              <p className="mt-1 text-sm text-red-600">{errors.documentoCompraNumero.message}</p>
            )}
          </div>
        </div>

        <div className="mt-4">
          <div className="mb-2 flex items-center justify-between">
            <h3 className="text-sm font-semibold text-slate-700 dark:text-slate-300">Productos</h3>
            <button
              type="button"
              onClick={() => append({ productoId: '', cantidad: '', costoUnitario: '' })}
              className="text-sm text-slate-700 underline hover:text-slate-900 dark:text-slate-300"
            >
              Agregar producto
            </button>
          </div>
          {errors.detalles?.root && <p className="mb-2 text-sm text-red-600">{errors.detalles.root.message}</p>}

          {fields.map((field, index) => (
            <div key={field.id} className="mb-2 grid grid-cols-[2fr_1fr_1fr_auto] gap-2">
              <div>
                <select
                  {...register(`detalles.${index}.productoId`)}
                  className="w-full rounded border border-slate-300 px-2 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
                >
                  <option value="">Producto...</option>
                  {productos.map((p) => (
                    <option key={p.id} value={p.id}>
                      {p.codigoInterno} — {p.nombre}
                    </option>
                  ))}
                </select>
                {errors.detalles?.[index]?.productoId && (
                  <p className="mt-1 text-xs text-red-600">{errors.detalles[index]?.productoId?.message}</p>
                )}
              </div>
              <div>
                <input
                  type="number"
                  step="0.001"
                  placeholder="Cantidad"
                  {...register(`detalles.${index}.cantidad`)}
                  className="w-full rounded border border-slate-300 px-2 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
                />
                {errors.detalles?.[index]?.cantidad && (
                  <p className="mt-1 text-xs text-red-600">{errors.detalles[index]?.cantidad?.message}</p>
                )}
              </div>
              <div>
                <input
                  type="number"
                  step="0.01"
                  placeholder="Costo unitario"
                  {...register(`detalles.${index}.costoUnitario`)}
                  className="w-full rounded border border-slate-300 px-2 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
                />
                {errors.detalles?.[index]?.costoUnitario && (
                  <p className="mt-1 text-xs text-red-600">{errors.detalles[index]?.costoUnitario?.message}</p>
                )}
              </div>
              <button
                type="button"
                onClick={() => remove(index)}
                disabled={fields.length === 1}
                className="text-red-600 underline hover:text-red-800 disabled:cursor-not-allowed disabled:opacity-40"
              >
                Quitar
              </button>
            </div>
          ))}
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
            {guardando ? 'Guardando...' : 'Registrar compra'}
          </button>
        </div>
      </form>
    </div>
  )
}
