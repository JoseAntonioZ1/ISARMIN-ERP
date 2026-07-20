import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useState } from 'react'
import { cajaApi } from '@/modules/caja/api/cajaApi'
import { AbrirCajaDialog } from '@/modules/caja/components/AbrirCajaDialog'
import { CerrarCajaDialog } from '@/modules/caja/components/CerrarCajaDialog'
import { RegistrarMovimientoCajaDialog } from '@/modules/caja/components/RegistrarMovimientoCajaDialog'
import { ApiError } from '@/shared/api/httpClient'

export function CajaPage() {
  const queryClient = useQueryClient()
  const [abriendo, setAbriendo] = useState(false)
  const [cerrando, setCerrando] = useState(false)
  const [registrando, setRegistrando] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const { data: caja, isLoading } = useQuery({ queryKey: ['caja', 'actual'], queryFn: cajaApi.obtenerActual })

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
    },
    onError: (e) => setError(e instanceof ApiError ? e.message : 'No se pudo abrir la caja.'),
  })

  const mutacionCerrar = useMutation({
    mutationFn: cajaApi.cerrar,
    onSuccess: () => {
      invalidar()
      setCerrando(false)
    },
    onError: (e) => setError(e instanceof ApiError ? e.message : 'No se pudo cerrar la caja.'),
  })

  const mutacionRegistrar = useMutation({
    mutationFn: ({ concepto, monto, descripcion }: { concepto: Parameters<typeof cajaApi.registrarMovimiento>[0]; monto: number; descripcion: string | null }) =>
      cajaApi.registrarMovimiento(concepto, monto, descripcion),
    onSuccess: () => {
      invalidar()
      setRegistrando(false)
    },
    onError: (e) => setError(e instanceof ApiError ? e.message : 'No se pudo registrar el movimiento.'),
  })

  if (isLoading) {
    return <p className="text-slate-500">Cargando...</p>
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
              className="rounded bg-[var(--color-acento)] px-4 py-2 text-sm text-white hover:brightness-90 dark:bg-[var(--color-acento)] dark:hover:brightness-110"
            >
              Cerrar caja
            </button>
          </div>
        ) : (
          <button
            type="button"
            onClick={() => setAbriendo(true)}
            className="rounded bg-[var(--color-acento)] px-4 py-2 text-sm text-white hover:brightness-90 dark:bg-[var(--color-acento)] dark:hover:brightness-110"
          >
            Abrir caja
          </button>
        )}
      </div>

      {error && <p className="mb-4 text-sm text-red-600">{error}</p>}

      {!caja ? (
        <p className="text-slate-500">Nunca se ha abierto una caja. Ábrela para empezar a registrar movimientos.</p>
      ) : (
        <div className="mb-6 rounded-lg bg-white p-4 shadow-sm dark:bg-slate-800">
          <p className="mb-1 text-sm">
            <span className="font-semibold">Estado:</span> {caja.estado}
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
                <span className={caja.diferencia && caja.diferencia !== 0 ? 'text-red-600' : ''}>
                  S/ {caja.diferencia?.toFixed(2)}
                </span>
              </p>
            </>
          )}
        </div>
      )}

      {caja && (
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
                <td className="px-4 py-2">{movimiento.tipo}</td>
                <td className="px-4 py-2">{movimiento.concepto}</td>
                <td className="px-4 py-2">{movimiento.monto.toFixed(2)}</td>
                <td className="px-4 py-2">{movimiento.descripcion ?? '—'}</td>
              </tr>
            ))}
            {movimientos?.length === 0 && (
              <tr>
                <td colSpan={5} className="px-4 py-2 text-slate-500">
                  Aún no hay movimientos registrados en esta caja.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      )}

      {abriendo && (
        <AbrirCajaDialog
          onGuardar={(monto) => {
            setError(null)
            mutacionAbrir.mutate(monto)
          }}
          onCancelar={() => setAbriendo(false)}
          guardando={mutacionAbrir.isPending}
        />
      )}

      {cerrando && (
        <CerrarCajaDialog
          onGuardar={(monto) => {
            setError(null)
            mutacionCerrar.mutate(monto)
          }}
          onCancelar={() => setCerrando(false)}
          guardando={mutacionCerrar.isPending}
        />
      )}

      {registrando && (
        <RegistrarMovimientoCajaDialog
          onGuardar={(datos) => {
            setError(null)
            mutacionRegistrar.mutate(datos)
          }}
          onCancelar={() => setRegistrando(false)}
          guardando={mutacionRegistrar.isPending}
        />
      )}
    </div>
  )
}
