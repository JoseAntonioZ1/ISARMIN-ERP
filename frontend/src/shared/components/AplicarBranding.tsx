import { useEffect } from 'react'
import { useBranding } from '@/shared/hooks/useBranding'

const COLOR_PRINCIPAL_PREDETERMINADO = '#EE2027' // rojo de marca ISARMIN — valor por defecto hasta que se configure otro

/** Sin salida visual — solo sincroniza título de pestaña, favicon y color principal con la configuración de la empresa. */
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

    document.documentElement.style.setProperty('--color-principal', branding.colorAcento ?? COLOR_PRINCIPAL_PREDETERMINADO)
  }, [branding])

  return null
}
