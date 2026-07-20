import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useEffect, useState } from 'react'
import type { ChangeEvent } from 'react'
import { configuracionEmpresaApi } from '@/modules/catalogos/api/catalogosApi'
import { ApiError } from '@/shared/api/httpClient'

const TAMANO_MAXIMO_LOGO_BYTES = 1.5 * 1024 * 1024 // 1.5 MB — el logo queda embebido (Base64) en la base de datos, no como archivo aparte
const COLOR_ACENTO_PREDETERMINADO = '#1e293b' // mismo tono (slate-800) que ya usa el sistema por defecto

function archivoABase64(archivo: File): Promise<string> {
  return new Promise((resolve, reject) => {
    const lector = new FileReader()
    lector.onload = () => resolve(lector.result as string)
    lector.onerror = () => reject(lector.error)
    lector.readAsDataURL(archivo)
  })
}

export function ConfiguracionEmpresaPage() {
  const queryClient = useQueryClient()
  const [error, setError] = useState<string | null>(null)
  const [exito, setExito] = useState(false)

  const [razonSocial, setRazonSocial] = useState('')
  const [ruc, setRuc] = useState('')
  const [direccion, setDireccion] = useState('')
  const [logo, setLogo] = useState('')
  const [montoAperturaCaja, setMontoAperturaCaja] = useState('')
  const [colorAcento, setColorAcento] = useState(COLOR_ACENTO_PREDETERMINADO)
  const [mensajeBienvenida, setMensajeBienvenida] = useState('')

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
    setColorAcento(configuracion.colorAcento ?? COLOR_ACENTO_PREDETERMINADO)
    setMensajeBienvenida(configuracion.mensajeBienvenida ?? '')
  }, [configuracion])

  const mutacionActualizar = useMutation({
    mutationFn: () =>
      configuracionEmpresaApi.actualizar({
        razonSocial: razonSocial.trim(),
        ruc: ruc.trim() || null,
        direccion: direccion.trim() || null,
        logo: logo.trim() || null,
        montoAperturaCajaPredeterminado: montoAperturaCaja ? Number(montoAperturaCaja) : null,
        colorAcento: colorAcento || null,
        mensajeBienvenida: mensajeBienvenida.trim() || null,
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

  const handleSeleccionarLogo = async (e: ChangeEvent<HTMLInputElement>) => {
    const archivo = e.target.files?.[0]
    e.target.value = ''
    if (!archivo) return

    if (archivo.size > TAMANO_MAXIMO_LOGO_BYTES) {
      setError('El logo no puede pesar más de 1.5 MB. Usa una imagen más liviana (recomendado: PNG o JPG comprimido).')
      return
    }

    setError(null)
    const base64 = await archivoABase64(archivo)
    setLogo(base64)
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
          <label className="mb-1 block text-sm font-medium text-slate-700 dark:text-slate-300">Logo (opcional)</label>
          <p className="mb-2 text-xs text-slate-500">
            Se guarda embebido en el sistema (no como enlace externo), así se muestra aunque no haya internet.
          </p>

          {logo && (
            <div className="mb-2 flex items-center gap-3">
              <img src={logo} alt="Logo actual" className="h-16 w-16 rounded border border-slate-300 object-contain dark:border-slate-600" />
              <button
                type="button"
                onClick={() => setLogo('')}
                className="text-xs text-red-600 underline hover:text-red-800"
              >
                Quitar logo
              </button>
            </div>
          )}

          <input
            type="file"
            accept="image/*"
            onChange={handleSeleccionarLogo}
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

        <div>
          <label className="mb-1 block text-sm font-medium text-slate-700 dark:text-slate-300">Color de acento</label>
          <div className="flex items-center gap-3">
            <input
              type="color"
              value={colorAcento}
              onChange={(e) => setColorAcento(e.target.value)}
              className="h-10 w-14 cursor-pointer rounded border border-slate-300 dark:border-slate-600"
            />
            <span className="text-xs text-slate-500">{colorAcento}</span>
          </div>
        </div>

        <div>
          <label className="mb-1 block text-sm font-medium text-slate-700 dark:text-slate-300">
            Mensaje de bienvenida (opcional)
          </label>
          <textarea
            value={mensajeBienvenida}
            onChange={(e) => setMensajeBienvenida(e.target.value)}
            rows={2}
            maxLength={500}
            placeholder="Se muestra en la pantalla de inicio para todo el equipo"
            className="w-full rounded border border-slate-300 px-3 py-2 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
          />
        </div>

        <button
          type="button"
          disabled={!razonSocial.trim() || mutacionActualizar.isPending}
          onClick={handleGuardar}
          className="rounded bg-[var(--color-acento)] px-4 py-2 text-sm text-white hover:brightness-90 disabled:opacity-50 dark:bg-[var(--color-acento)] dark:hover:brightness-110"
        >
          {mutacionActualizar.isPending ? 'Guardando...' : 'Guardar'}
        </button>
      </div>
    </div>
  )
}
