import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Check, Circle } from 'lucide-react'
import { useState } from 'react'
import {
  type DetalleConsumoInput,
  type EstadoPago,
  ESTADOS_PAGO,
  type OrdenTrabajoDetalle,
  ordenesTrabajoApi,
} from '@/modules/taller/api/ordenesTrabajoApi'
import type { Producto } from '@/modules/productos/api/productosApi'
import type { Usuario } from '@/modules/usuarios/api/usuariosApi'
import { usuariosApi } from '@/modules/usuarios/api/usuariosApi'
import { ESTADOS_OT_VISUAL } from '@/modules/taller/utils/estadoOtVisual'
import { ApiError } from '@/shared/api/httpClient'
import { EstadoCarga } from '@/shared/components/EstadoCarga'

interface OrdenTrabajoDetalleDialogProps {
  ordenTrabajoId: string
  productos: Producto[]
  onCerrar: () => void
  /** Atajo desde un estado Rechazado: cierra este diálogo y abre "Nueva recepción" con el cliente ya preseleccionado. */
  onNuevaOrdenParaCliente?: (clienteId: string) => void
}

export function OrdenTrabajoDetalleDialog({
  ordenTrabajoId,
  productos,
  onCerrar,
  onNuevaOrdenParaCliente,
}: OrdenTrabajoDetalleDialogProps) {
  const queryClient = useQueryClient()
  const [error, setError] = useState<string | null>(null)

  const { data: detalle, isLoading } = useQuery({
    queryKey: ['ordenes-trabajo', ordenTrabajoId],
    queryFn: () => ordenesTrabajoApi.obtener(ordenTrabajoId),
  })

  const { data: usuarios } = useQuery({ queryKey: ['usuarios', 'todos'], queryFn: () => usuariosApi.listar(1, 200) })

  const invalidar = () => {
    queryClient.invalidateQueries({ queryKey: ['ordenes-trabajo', ordenTrabajoId] })
    queryClient.invalidateQueries({ queryKey: ['ordenes-trabajo', 'lista'] })
  }

  const onError = (e: unknown, mensaje: string) => setError(e instanceof ApiError ? e.message : mensaje)

  const mutacionDiagnostico = useMutation({
    mutationFn: (descripcion: string) => ordenesTrabajoApi.registrarDiagnostico(ordenTrabajoId, descripcion),
    onSuccess: invalidar,
    onError: (e) => onError(e, 'No se pudo registrar el diagnóstico.'),
  })

  const mutacionCotizacion = useMutation({
    mutationFn: (montoEstimado: number) => ordenesTrabajoApi.generarCotizacion(ordenTrabajoId, montoEstimado),
    onSuccess: invalidar,
    onError: (e) => onError(e, 'No se pudo generar la cotización.'),
  })

  const mutacionDecision = useMutation({
    mutationFn: ({
      decision,
      cobroDiagnosticoRechazo,
      evidenciaAprobacion,
    }: {
      decision: 'Aprobada' | 'Rechazada'
      cobroDiagnosticoRechazo: number | null
      evidenciaAprobacion: string | null
    }) => ordenesTrabajoApi.registrarDecision(ordenTrabajoId, decision, cobroDiagnosticoRechazo, evidenciaAprobacion),
    onSuccess: invalidar,
    onError: (e) => onError(e, 'No se pudo registrar la decisión del cliente.'),
  })

  const mutacionReparacion = useMutation({
    mutationFn: ({ consumos, resultadoPruebas }: { consumos: DetalleConsumoInput[]; resultadoPruebas: string | null }) =>
      ordenesTrabajoApi.registrarReparacion(ordenTrabajoId, consumos, resultadoPruebas),
    onSuccess: invalidar,
    onError: (e) => onError(e, 'No se pudo registrar la reparación.'),
  })

  const mutacionEntrega = useMutation({
    mutationFn: ({
      estadoPago,
      montoPagado,
      saldoPendiente,
      usuarioAutorizoSaldoId,
    }: {
      estadoPago: EstadoPago
      montoPagado: number
      saldoPendiente: number | null
      usuarioAutorizoSaldoId: string | null
    }) => ordenesTrabajoApi.entregarEquipo(ordenTrabajoId, estadoPago, montoPagado, saldoPendiente, usuarioAutorizoSaldoId),
    onSuccess: invalidar,
    onError: (e) => onError(e, 'No se pudo entregar el equipo.'),
  })

  const mutacionGarantia = useMutation({
    mutationFn: ({ fechaInicio, fechaFin }: { fechaInicio: string; fechaFin: string }) =>
      ordenesTrabajoApi.registrarGarantia(ordenTrabajoId, fechaInicio, fechaFin),
    onSuccess: invalidar,
    onError: (e) => onError(e, 'No se pudo registrar la garantía.'),
  })

  const nombreProducto = (id: string) => {
    const producto = productos.find((p) => p.id === id)
    return producto ? `${producto.codigoInterno} — ${producto.nombre}` : id
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
      <div className="max-h-[90vh] w-full max-w-2xl overflow-y-auto rounded-lg bg-white p-6 shadow-lg dark:bg-slate-800">
        {isLoading || !detalle ? (
          <EstadoCarga />
        ) : (
          <>
            <div className="mb-4 flex items-center justify-between">
              <h2 className="text-lg font-semibold text-slate-800 dark:text-slate-100">
                {detalle.ordenTrabajo.equipoDescripcion}
              </h2>
              {(() => {
                const visual = ESTADOS_OT_VISUAL[detalle.ordenTrabajo.estado]
                const Icono = visual.icono
                return (
                  <span className={`flex items-center gap-1 rounded border px-2 py-1 text-xs font-medium ${visual.clase}`}>
                    <Icono className="h-3.5 w-3.5" />
                    {visual.etiqueta}
                  </span>
                )
              })()}
            </div>

            {error && <p className="mb-4 text-sm text-red-600">{error}</p>}

            <TimelineOrdenTrabajo detalle={detalle} />

            <p className="mb-1 text-sm text-slate-600 dark:text-slate-400">
              <span className="font-semibold">Falla reportada:</span> {detalle.ordenTrabajo.fallaReportada}
            </p>
            <p className="mb-4 text-sm text-slate-600 dark:text-slate-400">
              <span className="font-semibold">Recepción:</span> {new Date(detalle.ordenTrabajo.fechaRecepcion).toLocaleString()}
            </p>

            {detalle.ordenTrabajo.diagnostico && (
              <p className="mb-2 text-sm">
                <span className="font-semibold">Diagnóstico:</span> {detalle.ordenTrabajo.diagnostico.descripcion}
              </p>
            )}

            {detalle.ordenTrabajo.cotizacionReparacion && (
              <p className="mb-2 text-sm">
                <span className="font-semibold">Cotización:</span> S/ {detalle.ordenTrabajo.cotizacionReparacion.montoEstimado.toFixed(2)}
                {detalle.ordenTrabajo.cotizacionReparacion.decisionCliente &&
                  ` — ${detalle.ordenTrabajo.cotizacionReparacion.decisionCliente}`}
              </p>
            )}

            {detalle.ordenTrabajo.consumosRepuesto.length > 0 && (
              <div className="mb-2 text-sm">
                <span className="font-semibold">Repuestos consumidos:</span>
                <ul className="ml-4 list-disc">
                  {detalle.ordenTrabajo.consumosRepuesto.map((c) => (
                    <li key={c.id}>
                      {nombreProducto(c.productoId)} — {c.cantidad}
                    </li>
                  ))}
                </ul>
              </div>
            )}

            {detalle.ordenTrabajo.resultadoPruebas && (
              <p className="mb-2 text-sm">
                <span className="font-semibold">Resultado de pruebas:</span> {detalle.ordenTrabajo.resultadoPruebas}
              </p>
            )}

            {detalle.ordenTrabajo.estado === 'Entregado' && (
              <p className="mb-2 text-sm">
                <span className="font-semibold">Entrega:</span>{' '}
                {detalle.ordenTrabajo.fechaEntrega && new Date(detalle.ordenTrabajo.fechaEntrega).toLocaleString()} —{' '}
                {detalle.ordenTrabajo.estadoPago} (S/ {detalle.ordenTrabajo.montoPagado?.toFixed(2)})
                {detalle.ordenTrabajo.saldoPendiente ? ` — Saldo pendiente: S/ ${detalle.ordenTrabajo.saldoPendiente.toFixed(2)}` : ''}
              </p>
            )}

            {detalle.garantia && (
              <p className="mb-2 text-sm">
                <span className="font-semibold">Garantía:</span> {detalle.garantia.fechaInicio} a {detalle.garantia.fechaFin}
              </p>
            )}

            <div className="mt-4 border-t border-slate-200 pt-4 dark:border-slate-700">
              {detalle.ordenTrabajo.estado === 'Recibido' && (
                <FormularioDiagnostico
                  guardando={mutacionDiagnostico.isPending}
                  onGuardar={(descripcion) => {
                    setError(null)
                    mutacionDiagnostico.mutate(descripcion)
                  }}
                />
              )}

              {detalle.ordenTrabajo.estado === 'Diagnosticado' && (
                <FormularioCotizacion
                  guardando={mutacionCotizacion.isPending}
                  onGuardar={(monto) => {
                    setError(null)
                    mutacionCotizacion.mutate(monto)
                  }}
                />
              )}

              {detalle.ordenTrabajo.estado === 'Cotizado' && (
                <FormularioDecision
                  guardando={mutacionDecision.isPending}
                  onGuardar={(datos) => {
                    setError(null)
                    mutacionDecision.mutate(datos)
                  }}
                />
              )}

              {detalle.ordenTrabajo.estado === 'Aprobado' && (
                <FormularioReparacion
                  productos={productos}
                  guardando={mutacionReparacion.isPending}
                  onGuardar={(datos) => {
                    setError(null)
                    mutacionReparacion.mutate(datos)
                  }}
                />
              )}

              {detalle.ordenTrabajo.estado === 'ListoParaEntrega' && (
                <FormularioEntrega
                  usuarios={usuarios?.datos ?? []}
                  guardando={mutacionEntrega.isPending}
                  onGuardar={(datos) => {
                    setError(null)
                    mutacionEntrega.mutate(datos)
                  }}
                />
              )}

              {detalle.ordenTrabajo.estado === 'Entregado' && !detalle.garantia && (
                <FormularioGarantia
                  guardando={mutacionGarantia.isPending}
                  onGuardar={(datos) => {
                    setError(null)
                    mutacionGarantia.mutate(datos)
                  }}
                />
              )}

              {detalle.ordenTrabajo.estado === 'Rechazado' && (
                <div>
                  <p className="mb-3 text-sm text-[var(--color-terciario)]">El cliente rechazó la cotización. La OT queda cerrada.</p>
                  {onNuevaOrdenParaCliente && (
                    <button
                      type="button"
                      onClick={() => onNuevaOrdenParaCliente(detalle.ordenTrabajo.clienteId)}
                      className="rounded border border-slate-300 px-4 py-2 text-sm text-[var(--color-apoyo)] hover:bg-slate-100 dark:border-slate-600 dark:text-slate-300 dark:hover:bg-slate-700"
                    >
                      Registrar nueva orden para este cliente
                    </button>
                  )}
                </div>
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

type PasoEstado = 'completado' | 'actual' | 'pendiente' | 'omitido'

function TimelineOrdenTrabajo({ detalle }: { detalle: OrdenTrabajoDetalle }) {
  const ot = detalle.ordenTrabajo
  const decisionRechazada = ot.cotizacionReparacion?.decisionCliente === 'Rechazada'

  const pasos: { etiqueta: string; estado: PasoEstado; negativo?: boolean }[] = [
    { etiqueta: 'Recepción', estado: 'completado' },
    { etiqueta: 'Diagnóstico', estado: ot.diagnostico ? 'completado' : ot.estado === 'Recibido' ? 'actual' : 'pendiente' },
    { etiqueta: 'Cotización', estado: ot.cotizacionReparacion ? 'completado' : ot.estado === 'Diagnosticado' ? 'actual' : 'pendiente' },
    {
      etiqueta: 'Decisión',
      estado: ot.cotizacionReparacion?.decisionCliente ? 'completado' : ot.estado === 'Cotizado' ? 'actual' : 'pendiente',
      negativo: decisionRechazada,
    },
    {
      etiqueta: 'Reparación',
      estado: decisionRechazada
        ? 'omitido'
        : ot.estado === 'ListoParaEntrega' || ot.estado === 'Entregado'
          ? 'completado'
          : ot.estado === 'Aprobado'
            ? 'actual'
            : 'pendiente',
    },
    {
      etiqueta: 'Entrega',
      estado: decisionRechazada
        ? 'omitido'
        : ot.estado === 'Entregado'
          ? 'completado'
          : ot.estado === 'ListoParaEntrega'
            ? 'actual'
            : 'pendiente',
    },
    {
      etiqueta: 'Garantía',
      estado: decisionRechazada ? 'omitido' : detalle.garantia ? 'completado' : ot.estado === 'Entregado' ? 'actual' : 'pendiente',
    },
  ]

  return (
    <div className="mb-4 flex items-center overflow-x-auto pb-2">
      {pasos.map((paso, i) => (
        <div key={paso.etiqueta} className="flex flex-shrink-0 items-center">
          <div className="flex flex-col items-center gap-1">
            <div
              className={`flex h-6 w-6 items-center justify-center rounded-full border-2 ${
                paso.estado === 'completado'
                  ? paso.negativo
                    ? 'border-[var(--color-principal)] bg-[var(--color-principal)] text-white'
                    : 'border-emerald-500 bg-emerald-500 text-white'
                  : paso.estado === 'actual'
                    ? 'border-[var(--color-principal)] text-[var(--color-principal)]'
                    : 'border-slate-200 text-slate-300 dark:border-slate-700 dark:text-slate-600'
              }`}
            >
              {paso.estado === 'completado' ? <Check className="h-3.5 w-3.5" /> : <Circle className="h-2 w-2 fill-current" />}
            </div>
            <span
              className={`whitespace-nowrap text-[10px] ${
                paso.estado === 'pendiente' || paso.estado === 'omitido'
                  ? 'text-slate-300 dark:text-slate-600'
                  : 'text-slate-600 dark:text-slate-300'
              }`}
            >
              {paso.etiqueta}
            </span>
          </div>
          {i < pasos.length - 1 && <div className="mb-4 h-0.5 w-6 flex-shrink-0 bg-slate-200 dark:bg-slate-700" />}
        </div>
      ))}
    </div>
  )
}

function FormularioDiagnostico({ onGuardar, guardando }: { onGuardar: (descripcion: string) => void; guardando?: boolean }) {
  const [descripcion, setDescripcion] = useState('')
  return (
    <div>
      <h3 className="mb-2 text-sm font-semibold text-[var(--color-apoyo)] dark:text-slate-300">Registrar diagnóstico</h3>
      <textarea
        value={descripcion}
        onChange={(e) => setDescripcion(e.target.value)}
        rows={3}
        className="mb-2 w-full rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
      />
      <button
        type="button"
        disabled={!descripcion.trim() || guardando}
        onClick={() => onGuardar(descripcion.trim())}
        className="rounded bg-[var(--color-principal)] px-4 py-2 text-sm text-white hover:brightness-90 disabled:opacity-50 dark:bg-[var(--color-principal)] dark:hover:brightness-110"
      >
        {guardando ? 'Guardando...' : 'Registrar diagnóstico'}
      </button>
    </div>
  )
}

function FormularioCotizacion({ onGuardar, guardando }: { onGuardar: (monto: number) => void; guardando?: boolean }) {
  const [monto, setMonto] = useState('')
  return (
    <div>
      <h3 className="mb-2 text-sm font-semibold text-[var(--color-apoyo)] dark:text-slate-300">Generar cotización de reparación</h3>
      <input
        type="number"
        step="0.01"
        value={monto}
        onChange={(e) => setMonto(e.target.value)}
        placeholder="Monto estimado"
        className="mb-2 w-full rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
      />
      <button
        type="button"
        disabled={!monto || Number(monto) < 0 || guardando}
        onClick={() => onGuardar(Number(monto))}
        className="rounded bg-[var(--color-principal)] px-4 py-2 text-sm text-white hover:brightness-90 disabled:opacity-50 dark:bg-[var(--color-principal)] dark:hover:brightness-110"
      >
        {guardando ? 'Guardando...' : 'Generar cotización'}
      </button>
    </div>
  )
}

function FormularioDecision({
  onGuardar,
  guardando,
}: {
  onGuardar: (datos: { decision: 'Aprobada' | 'Rechazada'; cobroDiagnosticoRechazo: number | null; evidenciaAprobacion: string | null }) => void
  guardando?: boolean
}) {
  const [cobroDiagnostico, setCobroDiagnostico] = useState('')
  const [evidenciaAprobacion, setEvidenciaAprobacion] = useState('')

  return (
    <div>
      <h3 className="mb-2 text-sm font-semibold text-[var(--color-apoyo)] dark:text-slate-300">Decisión del cliente</h3>

      <label className="mb-1 block text-xs text-[var(--color-terciario)]">
        Evidencia de aprobación (opcional — ej. "aprobó por WhatsApp", "firmó en recibo físico")
      </label>
      <input
        value={evidenciaAprobacion}
        onChange={(e) => setEvidenciaAprobacion(e.target.value)}
        className="mb-2 w-full rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
      />

      <div className="mb-2 flex gap-3">
        <button
          type="button"
          disabled={guardando}
          onClick={() =>
            onGuardar({ decision: 'Aprobada', cobroDiagnosticoRechazo: null, evidenciaAprobacion: evidenciaAprobacion.trim() || null })
          }
          className="rounded bg-[var(--color-principal)] px-4 py-2 text-sm text-white hover:brightness-90 disabled:opacity-50 dark:bg-[var(--color-principal)] dark:hover:brightness-110"
        >
          Aprobar
        </button>
        <input
          type="number"
          step="0.01"
          value={cobroDiagnostico}
          onChange={(e) => setCobroDiagnostico(e.target.value)}
          placeholder="Cobro por diagnóstico (opcional)"
          className="flex-1 rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />
        <button
          type="button"
          disabled={guardando}
          onClick={() =>
            onGuardar({
              decision: 'Rechazada',
              cobroDiagnosticoRechazo: cobroDiagnostico ? Number(cobroDiagnostico) : null,
              evidenciaAprobacion: null,
            })
          }
          className="rounded border border-red-300 px-4 py-2 text-sm text-red-600 hover:bg-red-50 disabled:opacity-50"
        >
          Rechazar
        </button>
      </div>
    </div>
  )
}

function FormularioReparacion({
  productos,
  onGuardar,
  guardando,
}: {
  productos: Producto[]
  onGuardar: (datos: { consumos: DetalleConsumoInput[]; resultadoPruebas: string | null }) => void
  guardando?: boolean
}) {
  const [filas, setFilas] = useState<{ productoId: string; cantidad: string }[]>([])
  const [resultadoPruebas, setResultadoPruebas] = useState('')

  const agregarFila = () => setFilas([...filas, { productoId: '', cantidad: '' }])
  const quitarFila = (index: number) => setFilas(filas.filter((_, i) => i !== index))
  const actualizarFila = (index: number, campo: 'productoId' | 'cantidad', valor: string) =>
    setFilas(filas.map((f, i) => (i === index ? { ...f, [campo]: valor } : f)))

  const handleGuardar = () => {
    const consumos = filas
      .filter((f) => f.productoId && f.cantidad)
      .map((f) => ({ productoId: f.productoId, cantidad: Number(f.cantidad) }))
    onGuardar({ consumos, resultadoPruebas: resultadoPruebas.trim() || null })
  }

  return (
    <div>
      <h3 className="mb-2 text-sm font-semibold text-[var(--color-apoyo)] dark:text-slate-300">Registrar reparación</h3>

      <div className="mb-2 flex items-center justify-between">
        <span className="text-xs text-[var(--color-terciario)]">Repuestos consumidos (opcional)</span>
        <button type="button" onClick={agregarFila} className="text-xs text-[var(--color-apoyo)] underline dark:text-slate-300">
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

      <label className="mb-1 mt-2 block text-xs text-[var(--color-terciario)]">Resultado de pruebas (opcional)</label>
      <textarea
        value={resultadoPruebas}
        onChange={(e) => setResultadoPruebas(e.target.value)}
        rows={2}
        className="mb-2 w-full rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
      />

      <button
        type="button"
        disabled={guardando}
        onClick={handleGuardar}
        className="rounded bg-[var(--color-principal)] px-4 py-2 text-sm text-white hover:brightness-90 disabled:opacity-50 dark:bg-[var(--color-principal)] dark:hover:brightness-110"
      >
        {guardando ? 'Guardando...' : 'Registrar reparación'}
      </button>
    </div>
  )
}

function FormularioEntrega({
  usuarios,
  onGuardar,
  guardando,
}: {
  usuarios: Usuario[]
  onGuardar: (datos: {
    estadoPago: EstadoPago
    montoPagado: number
    saldoPendiente: number | null
    usuarioAutorizoSaldoId: string | null
  }) => void
  guardando?: boolean
}) {
  const [estadoPago, setEstadoPago] = useState<EstadoPago>('CompletoAlMomento')
  const [montoPagado, setMontoPagado] = useState('')
  const [saldoPendiente, setSaldoPendiente] = useState('')
  const [usuarioAutorizoId, setUsuarioAutorizoId] = useState('')

  const esSaldoPendiente = estadoPago === 'SaldoPendiente'

  return (
    <div>
      <h3 className="mb-2 text-sm font-semibold text-[var(--color-apoyo)] dark:text-slate-300">Entregar equipo</h3>

      <label className="mb-1 block text-xs text-[var(--color-terciario)]">Estado de pago</label>
      <select
        value={estadoPago}
        onChange={(e) => setEstadoPago(e.target.value as EstadoPago)}
        className="mb-2 w-full rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
      >
        {ESTADOS_PAGO.map((estado) => (
          <option key={estado} value={estado}>
            {estado}
          </option>
        ))}
      </select>

      <label className="mb-1 block text-xs text-[var(--color-terciario)]">Monto pagado</label>
      <input
        type="number"
        step="0.01"
        value={montoPagado}
        onChange={(e) => setMontoPagado(e.target.value)}
        className="mb-2 w-full rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
      />

      {esSaldoPendiente && (
        <>
          <label className="mb-1 block text-xs text-[var(--color-terciario)]">Saldo pendiente</label>
          <input
            type="number"
            step="0.01"
            value={saldoPendiente}
            onChange={(e) => setSaldoPendiente(e.target.value)}
            className="mb-2 w-full rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
          />

          <label className="mb-1 block text-xs text-[var(--color-terciario)]">
            Usuario Administrador/Propietario que autoriza
          </label>
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
        disabled={guardando || !montoPagado || (esSaldoPendiente && (!saldoPendiente || !usuarioAutorizoId))}
        onClick={() =>
          onGuardar({
            estadoPago,
            montoPagado: Number(montoPagado),
            saldoPendiente: esSaldoPendiente ? Number(saldoPendiente) : null,
            usuarioAutorizoSaldoId: esSaldoPendiente ? usuarioAutorizoId : null,
          })
        }
        className="rounded bg-[var(--color-principal)] px-4 py-2 text-sm text-white hover:brightness-90 disabled:opacity-50 dark:bg-[var(--color-principal)] dark:hover:brightness-110"
      >
        {guardando ? 'Guardando...' : 'Entregar equipo'}
      </button>
    </div>
  )
}

function FormularioGarantia({
  onGuardar,
  guardando,
}: {
  onGuardar: (datos: { fechaInicio: string; fechaFin: string }) => void
  guardando?: boolean
}) {
  const hoy = new Date().toISOString().slice(0, 10)
  const [fechaInicio, setFechaInicio] = useState(hoy)
  const [fechaFin, setFechaFin] = useState('')

  return (
    <div>
      <h3 className="mb-2 text-sm font-semibold text-[var(--color-apoyo)] dark:text-slate-300">Registrar garantía (opcional)</h3>
      <div className="mb-2 grid grid-cols-2 gap-2">
        <div>
          <label className="mb-1 block text-xs text-[var(--color-terciario)]">Fecha de inicio</label>
          <input
            type="date"
            value={fechaInicio}
            onChange={(e) => setFechaInicio(e.target.value)}
            className="w-full rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
          />
        </div>
        <div>
          <label className="mb-1 block text-xs text-[var(--color-terciario)]">Fecha de fin</label>
          <input
            type="date"
            value={fechaFin}
            onChange={(e) => setFechaFin(e.target.value)}
            className="w-full rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
          />
        </div>
      </div>
      <button
        type="button"
        disabled={guardando || !fechaFin}
        onClick={() => onGuardar({ fechaInicio, fechaFin })}
        className="rounded bg-[var(--color-principal)] px-4 py-2 text-sm text-white hover:brightness-90 disabled:opacity-50 dark:bg-[var(--color-principal)] dark:hover:brightness-110"
      >
        {guardando ? 'Guardando...' : 'Registrar garantía'}
      </button>
    </div>
  )
}
