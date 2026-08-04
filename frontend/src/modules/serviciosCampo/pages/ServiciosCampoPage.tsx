import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useEffect, useState } from 'react'
import { mediosPagoApi } from '@/modules/catalogos/api/catalogosApi'
import { clientesApi } from '@/modules/clientes/api/clientesApi'
import { productosApi } from '@/modules/productos/api/productosApi'
import { ServicioCampoDetalleDialog } from '@/modules/serviciosCampo/components/ServicioCampoDetalleDialog'
import { SolicitarServicioCampoDialog } from '@/modules/serviciosCampo/components/SolicitarServicioCampoDialog'
import { type EstadoServicioCampo, serviciosCampoApi } from '@/modules/serviciosCampo/api/serviciosCampoApi'
import { ESTADOS_SERVICIO_CAMPO_FILTRABLES, ESTADOS_SERVICIO_CAMPO_VISUAL } from '@/modules/serviciosCampo/utils/estadoServicioCampoVisual'
import { usuariosApi } from '@/modules/usuarios/api/usuariosApi'
import { ApiError } from '@/shared/api/httpClient'
import { ControlesPaginacion } from '@/shared/components/ControlesPaginacion'
import { EstadoCarga } from '@/shared/components/EstadoCarga'
import { EstadoVacio } from '@/shared/components/EstadoVacio'
import { useToast } from '@/shared/hooks/useToast'

const TAMANO_PAGINA = 20

export function ServiciosCampoPage() {
  const queryClient = useQueryClient()
  const { mostrarExito, mostrarError } = useToast()
  const [filtroEstado, setFiltroEstado] = useState<EstadoServicioCampo | ''>('')
  const [pagina, setPagina] = useState(1)
  const [solicitando, setSolicitando] = useState(false)
  const [verDetalle, setVerDetalle] = useState<string | null>(null)

  const {
    data: listado,
    isLoading,
    error: errorListado,
  } = useQuery({
    queryKey: ['servicios-campo', 'lista', filtroEstado, pagina],
    queryFn: () => serviciosCampoApi.buscar(filtroEstado || undefined, undefined, pagina, TAMANO_PAGINA),
  })

  useEffect(() => {
    if (errorListado) mostrarError('No se pudo cargar la lista de servicios de campo.')
  }, [errorListado, mostrarError])

  const { data: clientes } = useQuery({
    queryKey: ['clientes', 'todos'],
    queryFn: () => clientesApi.buscar(undefined, 1, 200),
  })

  const { data: productos } = useQuery({
    queryKey: ['productos', 'todos'],
    queryFn: () => productosApi.buscar(undefined, 1, 200),
  })

  const { data: usuarios } = useQuery({ queryKey: ['usuarios', 'todos'], queryFn: () => usuariosApi.listar(1, 200) })

  const { data: mediosPago } = useQuery({ queryKey: ['medios-pago'], queryFn: mediosPagoApi.listar })

  const nombreCliente = (id: string) => clientes?.datos.find((c) => c.id === id)?.nombreRazonSocial ?? '—'

  const mutacionSolicitar = useMutation({
    mutationFn: ({
      clienteId,
      descripcionTrabajo,
      tecnicoAsignadoId,
    }: {
      clienteId: string
      descripcionTrabajo: string
      tecnicoAsignadoId: string | null
    }) => serviciosCampoApi.solicitar(clienteId, descripcionTrabajo, tecnicoAsignadoId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['servicios-campo', 'lista'] })
      setSolicitando(false)
      mostrarExito('Servicio de campo solicitado correctamente.')
    },
    onError: (e) => mostrarError(e instanceof ApiError ? e.message : 'No se pudo solicitar el servicio de campo.'),
  })

  return (
    <div>
      <div className="mb-4 flex items-center justify-between">
        <h1 className="text-xl font-semibold text-slate-800 dark:text-slate-100">Servicios de Campo</h1>
        <button
          type="button"
          onClick={() => setSolicitando(true)}
          className="rounded bg-[var(--color-principal)] px-4 py-2 text-sm text-white hover:brightness-90 dark:bg-[var(--color-principal)] dark:hover:brightness-110"
        >
          Nueva solicitud
        </button>
      </div>

      <select
        value={filtroEstado}
        onChange={(e) => {
          setFiltroEstado(e.target.value as EstadoServicioCampo | '')
          setPagina(1)
        }}
        className="mb-4 rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
      >
        <option value="">Todos los estados</option>
        {ESTADOS_SERVICIO_CAMPO_FILTRABLES.map((estado) => (
          <option key={estado} value={estado}>
            {ESTADOS_SERVICIO_CAMPO_VISUAL[estado].etiqueta}
          </option>
        ))}
      </select>

      {isLoading ? (
        <EstadoCarga />
      ) : listado?.datos.length === 0 ? (
        <EstadoVacio mensaje="No se encontraron servicios de campo." />
      ) : (
        <>
          <div className="overflow-x-auto">
          <table className="w-full border-collapse overflow-hidden rounded-lg bg-white text-left text-sm shadow-sm dark:bg-slate-800">
            <thead className="bg-slate-100 dark:bg-slate-700">
              <tr>
                <th className="px-4 py-2">Solicitud</th>
                <th className="px-4 py-2">Cliente</th>
                <th className="px-4 py-2">Trabajo</th>
                <th className="px-4 py-2">Estado</th>
                <th className="px-4 py-2">Acciones</th>
              </tr>
            </thead>
            <tbody>
              {listado?.datos.map((servicio) => {
                const visual = ESTADOS_SERVICIO_CAMPO_VISUAL[servicio.estado]
                const Icono = visual.icono
                return (
                  <tr key={servicio.id} className="border-t border-slate-200 dark:border-slate-700">
                    <td className="px-4 py-2">{new Date(servicio.fechaSolicitud).toLocaleDateString()}</td>
                    <td className="px-4 py-2">{nombreCliente(servicio.clienteId)}</td>
                    <td className="px-4 py-2">{servicio.descripcionTrabajo}</td>
                    <td className="px-4 py-2">
                      <span className={`inline-flex items-center gap-1 rounded border px-2 py-0.5 text-xs font-medium ${visual.clase}`}>
                        <Icono className="h-3 w-3" />
                        {visual.etiqueta}
                      </span>
                    </td>
                    <td className="px-4 py-2">
                      <button
                        type="button"
                        onClick={() => setVerDetalle(servicio.id)}
                        className="text-[var(--color-apoyo)] underline hover:text-slate-900 dark:text-slate-300"
                      >
                        Ver / Gestionar
                      </button>
                    </td>
                  </tr>
                )
              })}
            </tbody>
          </table>
          </div>
          <ControlesPaginacion pagina={pagina} tamanoPagina={TAMANO_PAGINA} total={listado?.total ?? 0} onCambiarPagina={setPagina} />
        </>
      )}

      {solicitando && (
        <SolicitarServicioCampoDialog
          clientes={clientes?.datos ?? []}
          usuarios={usuarios?.datos ?? []}
          onGuardar={(datos) => mutacionSolicitar.mutate(datos)}
          onCancelar={() => setSolicitando(false)}
          guardando={mutacionSolicitar.isPending}
        />
      )}

      {verDetalle && (
        <ServicioCampoDetalleDialog
          servicioCampoId={verDetalle}
          productos={productos?.datos ?? []}
          mediosPago={mediosPago ?? []}
          onCerrar={() => setVerDetalle(null)}
        />
      )}
    </div>
  )
}
