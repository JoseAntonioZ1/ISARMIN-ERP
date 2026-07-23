import { Inbox } from 'lucide-react'

/** Mensaje para cuando una lista/búsqueda no tiene resultados — antes ninguna de las páginas de
 * catálogo mostraba nada en este caso, solo una tabla con encabezado y cero filas. */
export function EstadoVacio({ mensaje }: { mensaje: string }) {
  return (
    <div className="flex flex-col items-center gap-2 py-10 text-center text-[var(--color-terciario)]">
      <Inbox className="h-8 w-8" />
      <p className="text-sm">{mensaje}</p>
    </div>
  )
}
