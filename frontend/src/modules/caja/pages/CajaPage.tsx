import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useEffect, useState } from 'react'
import { cajaApi } from '@/modules/caja/api/cajaApi'
import { AbrirCajaDialog } from '@/modules/caja/components/AbrirCajaDialog'
import { CerrarCajaDialog } from '@/modules/caja/components/CerrarCajaDialog'
import { RegistrarMovimientoCajaDialog } from '@/modules/caja/components/RegistrarMovimientoCajaDialog'
import { ApiError } from '@/shared/api/httpClient'
import { EstadoCarga } from '@/shared/components/EstadoCarga'
import { useToast } from '@/shared/hooks/useToast'

export function CajaPage() {
  const queryClient = useQueryClient()
  const { mostrarExito, mostrarError } = useToast()
  const [abriendo, setAbriendo] = useState(false)
  const [cerrando, setCerrando] = useState(false)
  const [registrando, setRegistrando] = useState(false)

  const {
    data: caja,
    isLoading,
    error: errorCaja,
  } = useQuery({ queryKey: ['caja', 'actual'], queryFn: cajaApi.obtenerActual })

  useEffect(() => {
    if (errorCaja) mostrarError('No se pudo cargar el estado de la caja.')
  }, [errorCaja, mostrarError])

  const { data: movimientos } = useQuery({
    queryKey: ['caja', 'movimientos', caja?.id],
    queryFn: () => cajaApi.listarMovimientos(caja?.id),
    enabled: !!caja,
  })

  const invalidar = () => {
    queryClient.invalidateQueries({ queryKey: ['caja'] })
  }

  const mutacionAbrir = useMutation({
    mutationFn: cajaApi.abrir,
    onSuccess: () => {
      invalidar()
      setAbriendo(false)
      mostrarExito('Caja abierta correctamente.')
    },
    onError: (e) => mostrarError(e instanceof ApiError ? e.message : 'No se pudo abrir la caja.'),
  })

  const mutacionCerrar = useMutation({
    mutationFn: cajaApi.cerrar,
    onSuccess: () => {
      invalidar()
      setCerrando(false)
      mostrarExito('Caja cerrada correctamente.')
    },
    onError: (e) => mostrarError(e instanceof ApiError ? e.message : 'No se pudo cerrar la caja.'),
  })

  const mutacionRegistrar = useMutation({
    mutationFn: ({ concepto, monto, descripcion }: { concepto: Parameters<typeof cajaApi.registrarMovimiento>[0]; monto: number; descripcion: string | null }) =>
      cajaApi.registrarMovimiento(concepto, monto, descripcion),
    onSuccess: () => {
      invalidar()
      setRegistrando(false)
      mostrarExito('Movimiento registrado correctamente.')
    },
    onError: (e) => mostrarError(e instanceof ApiError ? e.message : 'No se pudo registrar el movimiento.'),
  })

  if (isLoading) {
    return <EstadoCarga />
  }

  return (
    <div>
      <div className="mb-4 flex items-center justify-between">
        <h1 className="text-xl font-semibold text-slate-800 dark:text-slate-100">Caja</h1>
        {caja?.estado === 'Abierta' ? (
          <div className="flex gap-3">
            <button
              type="button"
              onClick={() => setRegistrando(true)}
              className="rounded border border-slate-300 px-4 py-2 text-sm hover:bg-slate-100 dark:border-slate-600 dark:hover:bg-slate-700"
            >
              Registrar movimiento
            </button>
            <button
              type="button"
              onClick={() => setCerrando(true)}
              className="rounded bg-[var(--color-principal)] px-4 py-2 text-sm text-white hover:brightness-90 dark:bg-[var(--color-principal)] dark:hover:brightness-110"
            >
              Cerrar caja
            </button>
          </div>
        ) : (
          <button
            type="button"
            onClick={() => setAbriendo(true)}
            className="rounded bg-[var(--color-principal)] px-4 py-2 text-sm text-white hover:brightness-90 dark:bg-[var(--color-principal)] dark:hover:brightness-110"
          >
            Abrir caja
          </button>
        )}
      </div>

      {!caja ? (
        <p className="text-[var(--color-terciario)]">Nunca se ha abierto una caja. Ábrela para empezar a registrar movimientos.</p>
      ) : (
        <div className="mb-6 rounded-lg bg-white p-4 shadow-sm dark:bg-slate-800">
          <p className="mb-1 text-sm">
            <span className="font-semibold">Estado:</span>{' '}
            <span
              className={`inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium ${
                caja.estado === 'Abierta'
                  ? 'bg-emerald-50 text-emerald-700 dark:bg-emerald-950/40 dark:text-emerald-400'
                  : 'bg-slate-100 text-[var(--color-terciario)] dark:bg-slate-700 dark:text-slate-400'
              }`}
            >
              {caja.estado}
            </span>
          </p>
          <p className="mb-1 text-sm">
            <span className="font-semibold">Apertura:</span> {new Date(caja.fechaApertura).toLocaleString()} — S/{' '}
            {caja.montoApertura.toFixed(2)}
          </p>
          {caja.estado === 'Cerrada' && (
            <>
              <p className="mb-1 text-sm">
                <span className="font-semibold">Cierre:</span> {caja.fechaCierre && new Date(caja.fechaCierre).toLocaleString()}
              </p>
              <p className="mb-1 text-sm">
                <span className="font-semibold">Monto teórico:</span> S/ {caja.montoTeoricoCierre?.toFixed(2)}
              </p>
              <p className="mb-1 text-sm">
                <span className="font-semibold">Monto físico declarado:</span> S/ {caja.montoFisicoDeclarado?.toFixed(2)}
              </p>
              <p className="text-sm">
                <span className="font-semibold">Diferencia:</span>{' '}
                <span className={caja.diferencia && caja.diferencia !== 0 ? 'font-semibold text-red-600' : ''}>
                  S/ {caja.diferencia?.toFixed(2)}
                </span>
              </p>
            </>
          )}
        </div>
      )}

      {caja && (
        <div className="overflow-x-auto">
        <table className="w-full border-collapse overflow-hidden rounded-lg bg-white text-left text-sm shadow-sm dark:bg-slate-800">
          <thead className="bg-slate-100 dark:bg-slate-700">
            <tr>
              <th className="px-4 py-2">Fecha</th>
              <th className="px-4 py-2">Tipo</th>
              <th className="px-4 py-2">Concepto</th>
              <th className="px-4 py-2">Monto</th>
              <th className="px-4 py-2">Descripción</th>
            </tr>
          </thead>
          <tbody>
            {movimientos?.map((movimiento) => (
              <tr key={movimiento.id} className="border-t border-slate-200 dark:border-slate-700">
                <td className="px-4 py-2">{new Date(movimiento.fecha).toLocaleString()}</td>
                <td className="px-4 py-2">
                  <span className={movimiento.tipo === 'Ingreso' ? 'text-emerald-600' : 'text-red-600'}>{movimiento.tipo}</span>
                </td>
                <td className="px-4 py-2">{movimiento.concepto}</td>
                <td className="px-4 py-2">S/ {movimiento.monto.toFixed(2)}</td>
                <td className="px-4 py-2">{movimiento.descripcion ?? '—'}</td>
              </tr>
            ))}
            {movimientos?.length === 0 && (
              <tr>
                <td colSpan={5} className="px-4 py-2 text-[var(--color-terciario)]">
                  Aún no hay movimientos registrados en esta caja.
                </td>
              </tr>
            )}
          </tbody>
        </table>
        </div>
      )}

      {abriendo && (
        <AbrirCajaDialog
          onGuardar={(monto) => mutacionAbrir.mutate(monto)}
          onCancelar={() => setAbriendo(false)}
          guardando={mutacionAbrir.isPending}
        />
      )}

      {cerrando && (
        <CerrarCajaDialog
          onGuardar={(monto) => mutacionCerrar.mutate(monto)}
          onCancelar={() => setCerrando(false)}
          guardando={mutacionCerrar.isPending}
        />
      )}

      {registrando && (
        <RegistrarMovimientoCajaDialog
          onGuardar={(datos) => mutacionRegistrar.mutate(datos)}
          onCancelar={() => setRegistrando(false)}
          guardando={mutacionRegistrar.isPending}
        />
      )}
    </div>
  )
}
