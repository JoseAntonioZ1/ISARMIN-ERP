import {
  Banknote,
  Bolt,
  CreditCard,
  Droplet,
  Hammer,
  Landmark,
  type LucideIcon,
  Package,
  PaintBucket,
  Smartphone,
  Wallet,
  Wrench,
} from 'lucide-react'

/** Heurística por palabras clave en el nombre — no existe un campo de ícono en Categoria (backend),
 * así que se infiere para que la barra lateral del POS no se vea genérica. Si más adelante se quiere
 * un ícono elegido a mano por categoría, eso sí requeriría un campo nuevo en el backend. */
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

const PALABRAS_CLAVE_MEDIO_PAGO: [RegExp, LucideIcon][] = [
  [/efectivo/i, Banknote],
  [/yape|plin/i, Smartphone],
  [/transferencia|dep[oó]sito/i, Landmark],
  [/tarjeta/i, CreditCard],
]

/** RN: se asume que el medio de pago es en efectivo si su nombre lo indica — determina si se muestra
 * el campo "Monto recibido"/vuelto (solo tiene sentido para pagos en efectivo). */
export function esMedioPagoEfectivo(nombre: string): boolean {
  return /efectivo/i.test(nombre)
}

export function iconoParaMedioPago(nombre: string): LucideIcon {
  const coincidencia = PALABRAS_CLAVE_MEDIO_PAGO.find(([patron]) => patron.test(nombre))
  return coincidencia ? coincidencia[1] : Wallet
}
