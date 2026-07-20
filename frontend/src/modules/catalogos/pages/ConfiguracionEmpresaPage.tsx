import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useEffect, useState } from 'react'
import { configuracionEmpresaApi } from '@/modules/catalogos/api/catalogosApi'
import { ApiError } from '@/shared/api/httpClient'

export function ConfiguracionEmpresaPage() {
  const queryClient = useQueryClient()
  const [error, setError] = useState<string | null>(null)
  const [exito, setExito] = useState(false)

  const [razonSocial, setRazonSocial] = useState('')
  const [ruc, setRuc] = useState('')
  const [direccion, setDireccion] = useState('')
  const [logo, setLogo] = useState('')
  const [montoAperturaCaja, setMontoAperturaCaja] = useState('')

  const { data: configuracion, isLoading } = useQuery({
    queryKey: ['configuracion-empresa'],
    queryFn: configuracionEmpresaApi.obtener,
  })

  useEffect(() => {
    if (!configuracion) return
    setRazonSocial(configuracion.razonSocial)
    setRuc(configuracion.ruc ?? '')
    setDireccion(configuracion.direccion ?? '')
    setLogo(configuracion.logo ?? '')
    setMontoAperturaCaja(configuracion.montoAperturaCajaPredeterminado?.toString() ?? '')
  }, [configuracion])

  const mutacionActualizar = useMutation({
    mutationFn: () =>
      configuracionEmpresaApi.actualizar({
        razonSocial: razonSocial.trim(),
        ruc: ruc.trim() || null,
        direccion: direccion.trim() || null,
        logo: logo.trim() || null,
        montoAperturaCajaPredeterminado: montoAperturaCaja ? Number(montoAperturaCaja) : null,
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['configuracion-empresa'] })
      setExito(true)
      setError(null)
    },
    onError: (e) => {
      setExito(false)
      setError(e instanceof ApiError ? e.message : 'No se pudo actualizar la configuración.')
    },
  })

  const handleGuardar = () => {
    if (!razonSocial.trim()) return
    setExito(false)
    mutacionActualizar.mutate()
  }

  if (isLoading) {
    return <p className="text-slate-500">Cargando...</p>
  }

  return (
    <div>
      <h1 className="mb-4 text-xl font-semibold text-slate-800 dark:text-slate-100">Datos de la Empresa</h1>

      {error && <p className="mb-4 text-sm text-red-600">{error}</p>}
      {exito && <p className="mb-4 text-sm text-emerald-600">Configuración actualizada.</p>}

      <div className="max-w-lg space-y-3 rounded-lg bg-white p-4 shadow-sm dark:bg-slate-800">
        <div>
          <label className="mb-1 block text-sm font-medium text-slate-700 dark:text-slate-300">Razón social</label>
          <input
            value={razonSocial}
            onChange={(e) => setRazonSocial(e.target.value)}
            className="w-full rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
          />
        </div>

        <div>
          <label className="mb-1 block text-sm font-medium text-slate-700 dark:text-slate-300">RUC (opcional)</label>
          <input
            value={ruc}
            onChange={(e) => setRuc(e.target.value)}
            maxLength={11}
            className="w-full rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
          />
        </div>

        <div>
          <label className="mb-1 block text-sm font-medium text-slate-700 dark:text-slate-300">Dirección (opcional)</label>
          <input
            value={direccion}
            onChange={(e) => setDireccion(e.target.value)}
            className="w-full rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
          />
        </div>

        <div>
          <label className="mb-1 block text-sm font-medium text-slate-700 dark:text-slate-300">Logo — URL (opcional)</label>
          <input
            value={logo}
            onChange={(e) => setLogo(e.target.value)}
            className="w-full rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
          />
        </div>

        <div>
          <label className="mb-1 block text-sm font-medium text-slate-700 dark:text-slate-300">
            Monto de apertura de caja predeterminado (opcional)
          </label>
          <input
            type="number"
            step="0.01"
            value={montoAperturaCaja}
            onChange={(e) => setMontoAperturaCaja(e.target.value)}
            placeholder="Se usará para prellenar la apertura de caja"
            className="w-full rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
          />
        </div>

        <button
          type="button"
          disabled={!razonSocial.trim() || mutacionActualizar.isPending}
          onClick={handleGuardar}
          className="rounded bg-slate-800 px-4 py-2 text-sm text-white hover:bg-slate-700 disabled:opacity-50 dark:bg-slate-600 dark:hover:bg-slate-500"
        >
          {mutacionActualizar.isPending ? 'Guardando...' : 'Guardar'}
        </button>
      </div>
    </div>
  )
}
