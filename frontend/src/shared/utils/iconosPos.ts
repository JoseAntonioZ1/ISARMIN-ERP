import { Bolt, Droplet, Hammer, type LucideIcon, Package, PaintBucket, Wrench } from 'lucide-react'

/** Heurística por palabras clave en el nombre — no existe un campo de ícono en Categoria (backend),
 * así que se infiere para que las barras laterales de Ventas/Compras no se vean genéricas. Si más
 * adelante se quiere un ícono elegido a mano por categoría, eso sí requeriría un campo nuevo en backend. */
const PALABRAS_CLAVE_CATEGORIA: [RegExp, LucideIcon][] = [
  [/pintura|barniz|esmalte/i, PaintBucket],
  [/eléctric|electric|cable|foco|luminaria/i, Bolt],
  [/plomer|tubo|tuber|grifer|agua/i, Droplet],
  [/herramienta|taladro|martillo/i, Hammer],
  [/tornill|perno|fijaci/i, Wrench],
]

export function iconoParaCategoria(nombre: string): LucideIcon {
  const coincidencia = PALABRAS_CLAVE_CATEGORIA.find(([patron]) => patron.test(nombre))
  return coincidencia ? coincidencia[1] : Package
}
