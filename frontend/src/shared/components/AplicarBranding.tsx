import { useEffect } from 'react'
import { useBranding } from '@/shared/hooks/useBranding'

const COLOR_ACENTO_PREDETERMINADO = '#1e293b' // slate-800 — el mismo tono que ya usaba el sistema antes de ser configurable

/** Sin salida visual — solo sincroniza título de pestaña, favicon y color de acento con la configuración de la empresa. */
export function AplicarBranding() {
  const { data: branding } = useBranding()

  useEffect(() => {
    if (!branding) return

    document.title = `${branding.razonSocial} — ERP`

    if (branding.logo) {
      let icono = document.querySelector<HTMLLinkElement>('link[rel="icon"]')
      if (!icono) {
        icono = document.createElement('link')
        icono.rel = 'icon'
        document.head.appendChild(icono)
      }
      icono.removeAttribute('type')
      icono.href = branding.logo
    }

    document.documentElement.style.setProperty('--color-acento', branding.colorAcento ?? COLOR_ACENTO_PREDETERMINADO)
  }, [branding])

  return null
}
