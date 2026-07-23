import { jsPDF } from 'jspdf'
import type { MedioPago } from '@/modules/catalogos/api/catalogosApi'
import type { Cliente } from '@/modules/clientes/api/clientesApi'
import type { Venta } from '@/modules/ventas/api/ventasApi'
import type { Branding } from '@/shared/api/brandingApi'

/** Solo lo mínimo que necesita el comprobante — al finalizar una venta nueva (VentaPosPage) se arma
 * desde las líneas del carrito (sin codigoInterno); al reimprimir desde el historial se arma desde el
 * `Producto` completo ya cargado. */
interface ProductoParaComprobante {
  id: string
  nombre: string
  codigoInterno?: string
}

interface DatosComprobante {
  venta: Venta
  productos: ProductoParaComprobante[]
  cliente: Cliente | null
  mediosPago: MedioPago[]
  branding: Branding | null
}

/** Genera el PDF del comprobante de una venta a partir de sus datos ya persistidos — se puede llamar
 * tanto al finalizar la venta (descarga automática) como desde el historial (reimpresión); siempre
 * reproduce el mismo documento porque no depende de ningún estado guardado aparte. NO es un comprobante
 * electrónico válido ante SUNAT (ver RN-009/RF-081) — es un documento interno para el negocio y el cliente. */
export function generarComprobantePdf({ venta, productos, cliente, mediosPago, branding }: DatosComprobante): jsPDF {
  const doc = new jsPDF({ format: 'a5', unit: 'mm' })
  const anchoUtil = 148 - 20 // ancho de página a5 menos márgenes de 10mm a cada lado
  let y = 15

  const nombreProducto = (id: string) => {
    const producto = productos.find((p) => p.id === id)
    if (!producto) return id
    return producto.codigoInterno ? `${producto.codigoInterno} — ${producto.nombre}` : producto.nombre
  }

  const nombreMedioPago = (id: string) => mediosPago.find((m) => m.id === id)?.nombre ?? '—'

  if (branding?.logo) {
    const formato = /^data:image\/(\w+);/.exec(branding.logo)?.[1]?.toUpperCase() ?? 'PNG'
    try {
      doc.addImage(branding.logo, formato, 10, y, 20, 20)
    } catch {
      // Si el formato de imagen no es compatible con jsPDF (ej. SVG), se omite el logo sin interrumpir el comprobante.
    }
  }

  doc.setFontSize(13)
  doc.setFont('helvetica', 'bold')
  doc.text(branding?.razonSocial ?? 'ISARMIN ERP', 35, y + 5)
  doc.setFontSize(9)
  doc.setFont('helvetica', 'normal')
  if (branding?.ruc) doc.text(`RUC: ${branding.ruc}`, 35, y + 10)
  if (branding?.direccion) doc.text(branding.direccion, 35, y + 15)
  y += 25

  doc.setLineWidth(0.2)
  doc.line(10, y, 10 + anchoUtil, y)
  y += 6

  doc.setFontSize(11)
  doc.setFont('helvetica', 'bold')
  doc.text(venta.tipoComprobante.toUpperCase(), 10, y)
  doc.setFont('helvetica', 'normal')
  doc.setFontSize(9)
  doc.text(`N° de operación: ${venta.id.slice(0, 8).toUpperCase()}`, 10, (y += 6))
  doc.text(`Fecha: ${new Date(venta.fecha).toLocaleString('es-PE')}`, 10, (y += 5))
  doc.text(`Cliente: ${cliente?.nombreRazonSocial ?? 'Sin cliente registrado'}`, 10, (y += 5))
  if (cliente?.tipoDocumento === 'Ruc' && cliente.numeroDocumento) {
    doc.text(`RUC del cliente: ${cliente.numeroDocumento}`, 10, (y += 5))
  }
  y += 6

  doc.line(10, y, 10 + anchoUtil, y)
  y += 5
  doc.setFont('helvetica', 'bold')
  doc.text('Cant.', 10, y)
  doc.text('Descripción', 25, y)
  doc.text('P. Unit.', 105, y)
  doc.text('Subtotal', 125, y)
  doc.setFont('helvetica', 'normal')
  y += 4
  doc.line(10, y, 10 + anchoUtil, y)
  y += 5

  for (const detalle of venta.detalles) {
    const descripcion = nombreProducto(detalle.productoId)
    const lineasDescripcion = doc.splitTextToSize(descripcion, 78)
    doc.text(String(detalle.cantidad), 10, y)
    doc.text(lineasDescripcion, 25, y)
    doc.text(detalle.precioUnitario.toFixed(2), 105, y)
    doc.text((detalle.cantidad * detalle.precioUnitario).toFixed(2), 125, y)
    y += 4 * lineasDescripcion.length + 1
  }

  y += 3
  doc.line(10, y, 10 + anchoUtil, y)
  y += 6

  doc.setFont('helvetica', 'bold')
  doc.setFontSize(11)
  doc.text(`Total: S/ ${venta.total.toFixed(2)}`, 10 + anchoUtil, y, { align: 'right' })
  doc.setFont('helvetica', 'normal')
  doc.setFontSize(9)
  y += 7

  if (venta.pagos.length > 0) {
    doc.text('Pagos:', 10, y)
    y += 4
    for (const pago of venta.pagos) {
      doc.text(`${nombreMedioPago(pago.medioPagoId)}: S/ ${pago.monto.toFixed(2)}`, 12, y)
      y += 4
    }
  }

  if (venta.saldoPendiente) {
    doc.setTextColor(180, 60, 0)
    doc.text(`Saldo pendiente: S/ ${venta.saldoPendiente.toFixed(2)}`, 10, y)
    doc.setTextColor(0, 0, 0)
    y += 5
  }

  doc.setFontSize(7)
  doc.setTextColor(120, 120, 120)
  doc.text(
    'Documento interno de la empresa — no constituye un comprobante electrónico válido ante SUNAT.',
    10,
    200,
    { maxWidth: anchoUtil },
  )

  return doc
}
