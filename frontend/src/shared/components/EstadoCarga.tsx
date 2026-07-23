import { Loader2 } from 'lucide-react'

/** Reemplaza los `<p>Cargando...</p>` sueltos repetidos en 23+ archivos. */
export function EstadoCarga({ mensaje = 'Cargando...' }: { mensaje?: string }) {
  return (
    <div className="flex items-center gap-2 py-6 text-[var(--color-terciario)]">
      <Loader2 className="h-4 w-4 animate-spin" />
      <span>{mensaje}</span>
    </div>
  )
}
