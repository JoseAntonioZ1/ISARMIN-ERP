import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Search } from 'lucide-react'
import { useState } from 'react'
import { clientesApi } from '@/modules/clientes/api/clientesApi'
import { productosApi } from '@/modules/productos/api/productosApi'
import { ordenesTrabajoApi } from '@/modules/taller/api/ordenesTrabajoApi'
import type { EstadoOt, OrdenTrabajo } from '@/modules/taller/api/ordenesTrabajoApi'
import { OrdenTrabajoDetalleDialog } from '@/modules/taller/components/OrdenTrabajoDetalleDialog'
import { RegistrarRecepcionDialog } from '@/modules/taller/components/RegistrarRecepcionDialog'
import { generarComprobanteRecepcionPdf } from '@/modules/taller/utils/comprobanteRecepcionPdf'
import { COLUMNAS_KANBAN_OT, ESTADOS_OT_VISUAL } from '@/modules/taller/utils/estadoOtVisual'
import { ApiError } from '@/shared/api/httpClient'
import { useBranding } from '@/shared/hooks/useBranding'

export function TallerPage() {
  const queryClient = useQueryClient()
  const [registrando, setRegistrando] = useState(false)
  const [clienteInicialId, setClienteInicialId] = useState<string | undefined>(undefined)
  const [verDetalle, setVerDetalle] = useState<string | null>(null)
  const [busqueda, setBusqueda] = useState('')
  const [error, setError] = useState<string | null>(null)

  const { data: listado, isLoading } = useQuery({
    queryKey: ['ordenes-trabajo', 'lista'],
    queryFn: () => ordenesTrabajoApi.buscar(undefined, undefined, 1, 200),
  })

  const { data: clientes } = useQuery({
    queryKey: ['clientes', 'todos'],
    queryFn: () => clientesApi.buscar(undefined, 1, 200),
  })

  const { data: productos } = useQuery({
    queryKey: ['productos', 'todos'],
    queryFn: () => productosApi.buscar(undefined, 1, 200),
  })

  const { data: branding } = useBranding()

  const nombreCliente = (id: string) => clientes?.datos.find((c) => c.id === id)?.nombreRazonSocial ?? '—'

  const ordenesFiltradas =
    listado?.datos.filter((ot) => {
      if (!busqueda.trim()) return true
      const texto = busqueda.toLowerCase()
      return (
        ot.equipoDescripcion.toLowerCase().includes(texto) ||
        ot.fallaReportada.toLowerCase().includes(texto) ||
        nombreCliente(ot.clienteId).toLowerCase().includes(texto)
      )
    }) ?? []

  const ordenesPorEstado = (estado: EstadoOt) => ordenesFiltradas.filter((ot) => ot.estado === estado)

  const mutacionRegistrar = useMutation({
    mutationFn: ({ clienteId, equipoDescripcion, fallaReportada }: { clienteId: string; equipoDescripcion: string; fallaReportada: string }) =>
      ordenesTrabajoApi.registrarRecepcion(clienteId, equipoDescripcion, fallaReportada),
    onSuccess: (ordenTrabajo) => {
      queryClient.invalidateQueries({ queryKey: ['ordenes-trabajo', 'lista'] })

      const cliente = clientes?.datos.find((c) => c.id === ordenTrabajo.clienteId) ?? null
      generarComprobanteRecepcionPdf({ ordenTrabajo, cliente, branding: branding ?? null }).save(
        `recepcion-equipo-${ordenTrabajo.id.slice(0, 8)}.pdf`,
      )

      setRegistrando(false)
      setClienteInicialId(undefined)
    },
    onError: (e) => setError(e instanceof ApiError ? e.message : 'No se pudo registrar la recepción.'),
  })

  return (
    <div className="flex h-full flex-col gap-4">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold text-slate-800 dark:text-slate-100">Taller</h1>
        <button
          type="button"
          onClick={() => setRegistrando(true)}
          className="rounded bg-[var(--color-principal)] px-4 py-2 text-sm text-white hover:brightness-90 dark:bg-[var(--color-principal)] dark:hover:brightness-110"
        >
          Nueva recepción
        </button>
      </div>

      <div className="relative max-w-sm">
        <Search className="pointer-events-none absolute left-3 top-2.5 h-4 w-4 text-[var(--color-terciario)]" />
        <input
          value={busqueda}
          onChange={(e) => setBusqueda(e.target.value)}
          placeholder="Buscar por cliente, equipo o falla..."
          className="w-full rounded border border-slate-300 py-2 pl-9 pr-3 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />
      </div>

      {error && <p className="text-sm text-red-600">{error}</p>}

      {isLoading ? (
        <p className="text-[var(--color-terciario)]">Cargando...</p>
      ) : (
        <div className="flex min-h-0 flex-1 gap-3 overflow-x-auto pb-2">
          {[...COLUMNAS_KANBAN_OT, 'Rechazado' as const].map((estado) => {
            const visual = ESTADOS_OT_VISUAL[estado]
            const Icono = visual.icono
            const ordenes = ordenesPorEstado(estado)
            return (
              <div
                key={estado}
                className={`flex w-64 flex-shrink-0 flex-col rounded-lg border ${estado === 'Rechazado' ? 'opacity-80' : ''} border-slate-200 bg-slate-50 dark:border-slate-700 dark:bg-slate-900/50`}
              >
                <div className="flex items-center gap-2 border-b border-slate-200 p-3 dark:border-slate-700">
                  <Icono className="h-4 w-4 flex-shrink-0" />
                  <span className="flex-1 text-sm font-semibold text-slate-700 dark:text-slate-200">{visual.etiqueta}</span>
                  <span className="rounded-full bg-slate-200 px-2 py-0.5 text-xs text-slate-600 dark:bg-slate-700 dark:text-slate-300">
                    {ordenes.length}
                  </span>
                </div>
                <div className="flex-1 space-y-2 overflow-y-auto p-2">
                  {ordenes.map((ot) => (
                    <TarjetaOrdenTrabajo key={ot.id} ot={ot} nombreCliente={nombreCliente(ot.clienteId)} onClick={() => setVerDetalle(ot.id)} />
                  ))}
                  {ordenes.length === 0 && <p className="px-2 py-3 text-center text-xs text-[var(--color-terciario)]">Sin órdenes</p>}
                </div>
              </div>
            )
          })}
        </div>
      )}

      {registrando && (
        <RegistrarRecepcionDialog
          clientes={clientes?.datos ?? []}
          clienteInicialId={clienteInicialId}
          onGuardar={(datos) => {
            setError(null)
            mutacionRegistrar.mutate(datos)
          }}
          onCancelar={() => {
            setRegistrando(false)
            setClienteInicialId(undefined)
          }}
          guardando={mutacionRegistrar.isPending}
        />
      )}

      {verDetalle && (
        <OrdenTrabajoDetalleDialog
          ordenTrabajoId={verDetalle}
          productos={productos?.datos ?? []}
          onCerrar={() => setVerDetalle(null)}
          onNuevaOrdenParaCliente={(clienteId) => {
            setVerDetalle(null)
            setClienteInicialId(clienteId)
            setRegistrando(true)
          }}
        />
      )}
    </div>
  )
}

function TarjetaOrdenTrabajo({ ot, nombreCliente, onClick }: { ot: OrdenTrabajo; nombreCliente: string; onClick: () => void }) {
  return (
    <button
      type="button"
      onClick={onClick}
      className="w-full rounded-lg border border-slate-200 bg-white p-3 text-left shadow-sm transition-colors hover:border-[var(--color-principal)] dark:border-slate-700 dark:bg-slate-800"
    >
      <p className="line-clamp-2 text-sm font-medium text-slate-800 dark:text-slate-100">{ot.equipoDescripcion}</p>
      <p className="mt-1 truncate text-xs text-[var(--color-apoyo)] dark:text-slate-300">{nombreCliente}</p>
      <p className="mt-1 text-xs text-[var(--color-terciario)]">{new Date(ot.fechaRecepcion).toLocaleDateString()}</p>
    </button>
  )
}
