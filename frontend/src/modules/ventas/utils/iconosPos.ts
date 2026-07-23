import { Banknote, CreditCard, Landmark, type LucideIcon, Smartphone, Wallet } from 'lucide-react'

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
