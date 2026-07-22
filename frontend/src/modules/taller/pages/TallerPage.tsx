import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useState } from 'react'
import { clientesApi } from '@/modules/clientes/api/clientesApi'
import { productosApi } from '@/modules/productos/api/productosApi'
import { ordenesTrabajoApi } from '@/modules/taller/api/ordenesTrabajoApi'
import { OrdenTrabajoDetalleDialog } from '@/modules/taller/components/OrdenTrabajoDetalleDialog'
import { RegistrarRecepcionDialog } from '@/modules/taller/components/RegistrarRecepcionDialog'
import { ApiError } from '@/shared/api/httpClient'

export function TallerPage() {
  const queryClient = useQueryClient()
  const [registrando, setRegistrando] = useState(false)
  const [verDetalle, setVerDetalle] = useState<string | null>(null)
  const [error, setError] = useState<string | null>(null)

  const { data: listado, isLoading } = useQuery({
    queryKey: ['ordenes-trabajo', 'lista'],
    queryFn: () => ordenesTrabajoApi.buscar(),
  })

  const { data: clientes } = useQuery({
    queryKey: ['clientes', 'todos'],
    queryFn: () => clientesApi.buscar(undefined, 1, 200),
  })

  const { data: productos } = useQuery({
    queryKey: ['productos', 'todos'],
    queryFn: () => productosApi.buscar(undefined, 1, 200),
  })

  const nombreCliente = (id: string) => clientes?.datos.find((c) => c.id === id)?.nombreRazonSocial ?? '—'

  const mutacionRegistrar = useMutation({
    mutationFn: ({ clienteId, equipoDescripcion, fallaReportada }: { clienteId: string; equipoDescripcion: string; fallaReportada: string }) =>
      ordenesTrabajoApi.registrarRecepcion(clienteId, equipoDescripcion, fallaReportada),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['ordenes-trabajo', 'lista'] })
      setRegistrando(false)
    },
    onError: (e) => setError(e instanceof ApiError ? e.message : 'No se pudo registrar la recepción.'),
  })

  return (
    <div>
      <div className="mb-4 flex items-center justify-between">
        <h1 className="text-xl font-semibold text-slate-800 dark:text-slate-100">Taller</h1>
        <button
          type="button"
          onClick={() => setRegistrando(true)}
          className="rounded bg-[var(--color-principal)] px-4 py-2 text-sm text-white hover:brightness-90 dark:bg-[var(--color-principal)] dark:hover:brightness-110"
        >
          Nueva recepción
        </button>
      </div>

      {error && <p className="mb-4 text-sm text-red-600">{error}</p>}

      {isLoading ? (
        <p className="text-[var(--color-terciario)]">Cargando...</p>
      ) : (
        <table className="w-full border-collapse overflow-hidden rounded-lg bg-white text-left text-sm shadow-sm dark:bg-slate-800">
          <thead className="bg-slate-100 dark:bg-slate-700">
            <tr>
              <th className="px-4 py-2">Recepción</th>
              <th className="px-4 py-2">Cliente</th>
              <th className="px-4 py-2">Equipo</th>
              <th className="px-4 py-2">Estado</th>
              <th className="px-4 py-2">Acciones</th>
            </tr>
          </thead>
          <tbody>
            {listado?.datos.map((ot) => (
              <tr key={ot.id} className="border-t border-slate-200 dark:border-slate-700">
                <td className="px-4 py-2">{new Date(ot.fechaRecepcion).toLocaleDateString()}</td>
                <td className="px-4 py-2">{nombreCliente(ot.clienteId)}</td>
                <td className="px-4 py-2">{ot.equipoDescripcion}</td>
                <td className="px-4 py-2">{ot.estado}</td>
                <td className="px-4 py-2">
                  <button
                    type="button"
                    onClick={() => setVerDetalle(ot.id)}
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

      {registrando && (
        <RegistrarRecepcionDialog
          clientes={clientes?.datos ?? []}
          onGuardar={(datos) => {
            setError(null)
            mutacionRegistrar.mutate(datos)
          }}
          onCancelar={() => setRegistrando(false)}
          guardando={mutacionRegistrar.isPending}
        />
      )}

      {verDetalle && (
        <OrdenTrabajoDetalleDialog
          ordenTrabajoId={verDetalle}
          productos={productos?.datos ?? []}
          onCerrar={() => setVerDetalle(null)}
        />
      )}
    </div>
  )
}
