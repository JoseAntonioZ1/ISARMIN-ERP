import { useBranding } from '@/shared/hooks/useBranding'

export function PaginaInicio() {
  const { data: branding } = useBranding()

  return (
    <main className="flex min-h-screen items-center justify-center bg-[var(--color-fondo)] dark:bg-slate-900">
      <div className="text-center">
        {branding?.logo && <img src={branding.logo} alt="Logo" className="mx-auto mb-4 h-20 w-20 object-contain" />}
        <h1 className="text-2xl font-semibold text-slate-800 dark:text-slate-100">{branding?.razonSocial ?? 'ISARMIN ERP'}</h1>
        <p className="mt-2 text-[var(--color-terciario)] dark:text-slate-400">
          {branding?.mensajeBienvenida ?? 'Entorno base listo. Los módulos se incorporarán de forma incremental.'}
        </p>
      </div>
    </main>
  )
}
