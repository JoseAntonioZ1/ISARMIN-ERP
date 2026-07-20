import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useState } from 'react'
import type { MedioPago } from '@/modules/catalogos/api/catalogosApi'
import type { Producto } from '@/modules/productos/api/productosApi'
import { type DetalleConsumoCampoInput, serviciosCampoApi } from '@/modules/serviciosCampo/api/serviciosCampoApi'
import type { Usuario } from '@/modules/usuarios/api/usuariosApi'
import { usuariosApi } from '@/modules/usuarios/api/usuariosApi'
import { ApiError } from '@/shared/api/httpClient'

interface ServicioCampoDetalleDialogProps {
  servicioCampoId: string
  productos: Producto[]
  mediosPago: MedioPago[]
  onCerrar: () => void
}

export function ServicioCampoDetalleDialog({ servicioCampoId, productos, mediosPago, onCerrar }: ServicioCampoDetalleDialogProps) {
  const queryClient = useQueryClient()
  const [error, setError] = useState<string | null>(null)

  const { data: servicio, isLoading } = useQuery({
    queryKey: ['servicios-campo', servicioCampoId],
    queryFn: () => serviciosCampoApi.obtener(servicioCampoId),
  })

  const { data: usuarios } = useQuery({ queryKey: ['usuarios', 'todos'], queryFn: () => usuariosApi.listar(1, 200) })

  const invalidar = () => {
    queryClient.invalidateQueries({ queryKey: ['servicios-campo', servicioCampoId] })
    queryClient.invalidateQueries({ queryKey: ['servicios-campo', 'lista'] })
  }

  const onError = (e: unknown, mensaje: string) => setError(e instanceof ApiError ? e.message : mensaje)

  const mutacionCotizar = useMutation({
    mutationFn: (montoEstimado: number) => serviciosCampoApi.cotizar(servicioCampoId, montoEstimado),
    onSuccess: invalidar,
    onError: (e) => onError(e, 'No se pudo registrar la cotización.'),
  })

  const mutacionCerrar = useMutation({
    mutationFn: ({
      consumos,
      estadoFinal,
      observaciones,
    }: {
      consumos: DetalleConsumoCampoInput[]
      estadoFinal: string
      observaciones: string | null
    }) => serviciosCampoApi.cerrar(servicioCampoId, consumos, estadoFinal, observaciones),
    onSuccess: invalidar,
    onError: (e) => onError(e, 'No se pudo cerrar el servicio.'),
  })

  const mutacionCobrar = useMutation({
    mutationFn: ({
      medioPagoId,
      montoPagado,
      saldoPendiente,
      usuarioAutorizoSaldoId,
    }: {
      medioPagoId: string
      montoPagado: number
      saldoPendiente: number | null
      usuarioAutorizoSaldoId: string | null
    }) => serviciosCampoApi.cobrar(servicioCampoId, medioPagoId, montoPagado, saldoPendiente, usuarioAutorizoSaldoId),
    onSuccess: invalidar,
    onError: (e) => onError(e, 'No se pudo registrar el cobro.'),
  })

  const nombreProducto = (id: string) => {
    const producto = productos.find((p) => p.id === id)
    return producto ? `${producto.codigoInterno} — ${producto.nombre}` : id
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
      <div className="max-h-[90vh] w-full max-w-2xl overflow-y-auto rounded-lg bg-white p-6 shadow-lg dark:bg-slate-800">
        {isLoading || !servicio ? (
          <p className="text-slate-500">Cargando...</p>
        ) : (
          <>
            <div className="mb-4 flex items-center justify-between">
              <h2 className="text-lg font-semibold text-slate-800 dark:text-slate-100">Servicio de campo</h2>
              <span className="rounded bg-slate-100 px-2 py-1 text-xs font-medium text-slate-700 dark:bg-slate-700 dark:text-slate-200">
                {servicio.estado}
              </span>
            </div>

            {error && <p className="mb-4 text-sm text-red-600">{error}</p>}

            <p className="mb-1 text-sm text-slate-600 dark:text-slate-400">
              <span className="font-semibold">Trabajo:</span> {servicio.descripcionTrabajo}
            </p>
            <p className="mb-4 text-sm text-slate-600 dark:text-slate-400">
              <span className="font-semibold">Solicitud:</span> {new Date(servicio.fechaSolicitud).toLocaleString()}
            </p>

            {servicio.montoEstimado != null && (
              <p className="mb-2 text-sm">
                <span className="font-semibold">Monto estimado:</span> S/ {servicio.montoEstimado.toFixed(2)}
              </p>
            )}

            {servicio.detalles.length > 0 && (
              <div className="mb-2 text-sm">
                <span className="font-semibold">Materiales consumidos:</span>
                <ul className="ml-4 list-disc">
                  {servicio.detalles.map((d) => (
                    <li key={d.id}>
                      {nombreProducto(d.productoId)} — {d.cantidad}
                    </li>
                  ))}
                </ul>
              </div>
            )}

            {servicio.estadoFinal && (
              <p className="mb-2 text-sm">
                <span className="font-semibold">Estado final:</span> {servicio.estadoFinal}
              </p>
            )}

            {servicio.observaciones && (
              <p className="mb-2 text-sm">
                <span className="font-semibold">Observaciones:</span> {servicio.observaciones}
              </p>
            )}

            {servicio.montoPagado != null && (
              <p className="mb-2 text-sm">
                <span className="font-semibold">Cobro:</span> S/ {servicio.montoPagado.toFixed(2)}
                {servicio.saldoPendiente ? ` — Saldo pendiente: S/ ${servicio.saldoPendiente.toFixed(2)}` : ''}
              </p>
            )}

            <div className="mt-4 border-t border-slate-200 pt-4 dark:border-slate-700">
              {servicio.estado === 'Solicitado' && (
                <div className="space-y-4">
                  <FormularioCotizar
                    guardando={mutacionCotizar.isPending}
                    onGuardar={(monto) => {
                      setError(null)
                      mutacionCotizar.mutate(monto)
                    }}
                  />
                  <FormularioCerrar
                    productos={productos}
                    guardando={mutacionCerrar.isPending}
                    onGuardar={(datos) => {
                      setError(null)
                      mutacionCerrar.mutate(datos)
                    }}
                  />
                </div>
              )}

              {servicio.estado === 'Cerrado' && servicio.montoPagado == null && (
                <FormularioCobrar
                  mediosPago={mediosPago}
                  usuarios={usuarios?.datos ?? []}
                  guardando={mutacionCobrar.isPending}
                  onGuardar={(datos) => {
                    setError(null)
                    mutacionCobrar.mutate(datos)
                  }}
                />
              )}
            </div>
          </>
        )}

        <div className="mt-5 flex justify-end">
          <button
            type="button"
            onClick={onCerrar}
            className="rounded border border-slate-300 px-4 py-2 text-sm hover:bg-slate-100 dark:border-slate-600 dark:hover:bg-slate-700"
          >
            Cerrar
          </button>
        </div>
      </div>
    </div>
  )
}

function FormularioCotizar({ onGuardar, guardando }: { onGuardar: (monto: number) => void; guardando?: boolean }) {
  const [monto, setMonto] = useState('')
  return (
    <div>
      <h3 className="mb-2 text-sm font-semibold text-slate-700 dark:text-slate-300">Cotizar</h3>
      <div className="flex gap-2">
        <input
          type="number"
          step="0.01"
          value={monto}
          onChange={(e) => setMonto(e.target.value)}
          placeholder="Monto estimado"
          className="flex-1 rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />
        <button
          type="button"
          disabled={!monto || Number(monto) < 0 || guardando}
          onClick={() => onGuardar(Number(monto))}
          className="rounded bg-slate-800 px-4 py-2 text-sm text-white hover:bg-slate-700 disabled:opacity-50 dark:bg-slate-600 dark:hover:bg-slate-500"
        >
          {guardando ? 'Guardando...' : 'Cotizar'}
        </button>
      </div>
    </div>
  )
}

function FormularioCerrar({
  productos,
  onGuardar,
  guardando,
}: {
  productos: Producto[]
  onGuardar: (datos: { consumos: DetalleConsumoCampoInput[]; estadoFinal: string; observaciones: string | null }) => void
  guardando?: boolean
}) {
  const [filas, setFilas] = useState<{ productoId: string; cantidad: string }[]>([])
  const [estadoFinal, setEstadoFinal] = useState('')
  const [observaciones, setObservaciones] = useState('')

  const agregarFila = () => setFilas([...filas, { productoId: '', cantidad: '' }])
  const quitarFila = (index: number) => setFilas(filas.filter((_, i) => i !== index))
  const actualizarFila = (index: number, campo: 'productoId' | 'cantidad', valor: string) =>
    setFilas(filas.map((f, i) => (i === index ? { ...f, [campo]: valor } : f)))

  const handleGuardar = () => {
    const consumos = filas
      .filter((f) => f.productoId && f.cantidad)
      .map((f) => ({ productoId: f.productoId, cantidad: Number(f.cantidad) }))
    onGuardar({ consumos, estadoFinal: estadoFinal.trim(), observaciones: observaciones.trim() || null })
  }

  return (
    <div>
      <h3 className="mb-2 text-sm font-semibold text-slate-700 dark:text-slate-300">Cerrar servicio</h3>

      <div className="mb-2 flex items-center justify-between">
        <span className="text-xs text-slate-500">Materiales consumidos (opcional)</span>
        <button type="button" onClick={agregarFila} className="text-xs text-slate-700 underline dark:text-slate-300">
          Agregar producto
        </button>
      </div>

      {filas.map((fila, index) => (
        <div key={index} className="mb-2 grid grid-cols-[2fr_1fr_auto] gap-2">
          <select
            value={fila.productoId}
            onChange={(e) => actualizarFila(index, 'productoId', e.target.value)}
            className="rounded border border-slate-300 px-2 py-1 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
          >
            <option value="">Producto...</option>
            {productos.map((p) => (
              <option key={p.id} value={p.id}>
                {p.codigoInterno} — {p.nombre}
              </option>
            ))}
          </select>
          <input
            type="number"
            step="0.001"
            value={fila.cantidad}
            onChange={(e) => actualizarFila(index, 'cantidad', e.target.value)}
            placeholder="Cantidad"
            className="rounded border border-slate-300 px-2 py-1 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
          />
          <button type="button" onClick={() => quitarFila(index)} className="text-red-600 underline">
            Quitar
          </button>
        </div>
      ))}

      <label className="mb-1 mt-2 block text-xs text-slate-500">Estado final</label>
      <input
        value={estadoFinal}
        onChange={(e) => setEstadoFinal(e.target.value)}
        placeholder="Completado, Pendiente de repuesto, etc."
        className="mb-2 w-full rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
      />

      <label className="mb-1 block text-xs text-slate-500">Observaciones (opcional)</label>
      <textarea
        value={observaciones}
        onChange={(e) => setObservaciones(e.target.value)}
        rows={2}
        className="mb-2 w-full rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
      />

      <button
        type="button"
        disabled={!estadoFinal.trim() || guardando}
        onClick={handleGuardar}
        className="rounded bg-slate-800 px-4 py-2 text-sm text-white hover:bg-slate-700 disabled:opacity-50 dark:bg-slate-600 dark:hover:bg-slate-500"
      >
        {guardando ? 'Guardando...' : 'Cerrar servicio'}
      </button>
    </div>
  )
}

function FormularioCobrar({
  mediosPago,
  usuarios,
  onGuardar,
  guardando,
}: {
  mediosPago: MedioPago[]
  usuarios: Usuario[]
  onGuardar: (datos: {
    medioPagoId: string
    montoPagado: number
    saldoPendiente: number | null
    usuarioAutorizoSaldoId: string | null
  }) => void
  guardando?: boolean
}) {
  const [medioPagoId, setMedioPagoId] = useState('')
  const [montoPagado, setMontoPagado] = useState('')
  const [conSaldoPendiente, setConSaldoPendiente] = useState(false)
  const [saldoPendiente, setSaldoPendiente] = useState('')
  const [usuarioAutorizoId, setUsuarioAutorizoId] = useState('')

  return (
    <div>
      <h3 className="mb-2 text-sm font-semibold text-slate-700 dark:text-slate-300">Registrar cobro</h3>

      <label className="mb-1 block text-xs text-slate-500">Medio de pago</label>
      <select
        value={medioPagoId}
        onChange={(e) => setMedioPagoId(e.target.value)}
        className="mb-2 w-full rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
      >
        <option value="">Seleccionar...</option>
        {mediosPago.map((m) => (
          <option key={m.id} value={m.id}>
            {m.nombre}
          </option>
        ))}
      </select>

      <label className="mb-1 block text-xs text-slate-500">Monto pagado</label>
      <input
        type="number"
        step="0.01"
        value={montoPagado}
        onChange={(e) => setMontoPagado(e.target.value)}
        className="mb-2 w-full rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
      />

      <label className="mb-2 flex items-center gap-2 text-xs text-slate-500">
        <input type="checkbox" checked={conSaldoPendiente} onChange={(e) => setConSaldoPendiente(e.target.checked)} />
        Queda saldo pendiente
      </label>

      {conSaldoPendiente && (
        <>
          <label className="mb-1 block text-xs text-slate-500">Saldo pendiente</label>
          <input
            type="number"
            step="0.01"
            value={saldoPendiente}
            onChange={(e) => setSaldoPendiente(e.target.value)}
            className="mb-2 w-full rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
          />

          <label className="mb-1 block text-xs text-slate-500">Usuario Administrador/Propietario que autoriza</label>
          <select
            value={usuarioAutorizoId}
            onChange={(e) => setUsuarioAutorizoId(e.target.value)}
            className="mb-2 w-full rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
          >
            <option value="">Seleccionar...</option>
            {usuarios.map((u) => (
              <option key={u.id} value={u.id}>
                {u.nombre}
              </option>
            ))}
          </select>
        </>
      )}

      <button
        type="button"
        disabled={guardando || !medioPagoId || !montoPagado || (conSaldoPendiente && (!saldoPendiente || !usuarioAutorizoId))}
        onClick={() =>
          onGuardar({
            medioPagoId,
            montoPagado: Number(montoPagado),
            saldoPendiente: conSaldoPendiente ? Number(saldoPendiente) : null,
            usuarioAutorizoSaldoId: conSaldoPendiente ? usuarioAutorizoId : null,
          })
        }
        className="rounded bg-slate-800 px-4 py-2 text-sm text-white hover:bg-slate-700 disabled:opacity-50 dark:bg-slate-600 dark:hover:bg-slate-500"
      >
        {guardando ? 'Guardando...' : 'Registrar cobro'}
      </button>
    </div>
  )
}
