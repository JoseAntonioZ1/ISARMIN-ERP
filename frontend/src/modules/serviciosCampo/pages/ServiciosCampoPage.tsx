import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useState } from 'react'
import { mediosPagoApi } from '@/modules/catalogos/api/catalogosApi'
import { clientesApi } from '@/modules/clientes/api/clientesApi'
import { productosApi } from '@/modules/productos/api/productosApi'
import { ServicioCampoDetalleDialog } from '@/modules/serviciosCampo/components/ServicioCampoDetalleDialog'
import { SolicitarServicioCampoDialog } from '@/modules/serviciosCampo/components/SolicitarServicioCampoDialog'
import { serviciosCampoApi } from '@/modules/serviciosCampo/api/serviciosCampoApi'
import { usuariosApi } from '@/modules/usuarios/api/usuariosApi'
import { ApiError } from '@/shared/api/httpClient'

export function ServiciosCampoPage() {
  const queryClient = useQueryClient()
  const [solicitando, setSolicitando] = useState(false)
  const [verDetalle, setVerDetalle] = useState<string | null>(null)
  const [error, setError] = useState<string | null>(null)

  const { data: listado, isLoading } = useQuery({
    queryKey: ['servicios-campo', 'lista'],
    queryFn: () => serviciosCampoApi.buscar(),
  })

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
    },
    onError: (e) => setError(e instanceof ApiError ? e.message : 'No se pudo solicitar el servicio de campo.'),
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

      {error && <p className="mb-4 text-sm text-red-600">{error}</p>}

      {isLoading ? (
        <p className="text-[var(--color-terciario)]">Cargando...</p>
      ) : (
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
            {listado?.datos.map((servicio) => (
              <tr key={servicio.id} className="border-t border-slate-200 dark:border-slate-700">
                <td className="px-4 py-2">{new Date(servicio.fechaSolicitud).toLocaleDateString()}</td>
                <td className="px-4 py-2">{nombreCliente(servicio.clienteId)}</td>
                <td className="px-4 py-2">{servicio.descripcionTrabajo}</td>
                <td className="px-4 py-2">{servicio.estado}</td>
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
            ))}
          </tbody>
        </table>
      )}

      {solicitando && (
        <SolicitarServicioCampoDialog
          clientes={clientes?.datos ?? []}
          usuarios={usuarios?.datos ?? []}
          onGuardar={(datos) => {
            setError(null)
            mutacionSolicitar.mutate(datos)
          }}
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
