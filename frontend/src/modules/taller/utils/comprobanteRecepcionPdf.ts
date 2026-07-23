import { jsPDF } from 'jspdf'
import type { Cliente } from '@/modules/clientes/api/clientesApi'
import type { OrdenTrabajo } from '@/modules/taller/api/ordenesTrabajoApi'
import type { Branding } from '@/shared/api/brandingApi'

interface DatosComprobanteRecepcion {
  ordenTrabajo: OrdenTrabajo
  cliente: Cliente | null
  branding: Branding | null
}

/** Comprobante de recepción de equipo (UC-22/RF-051 a RF-053) — nunca se había implementado (ni
 * backend ni frontend). Mismo patrón que el comprobante de venta (`ventas/utils/comprobantePdf.ts`):
 * PDF 100% cliente con jsPDF, regenerado desde los datos ya persistidos de la OT. */
export function generarComprobanteRecepcionPdf({ ordenTrabajo, cliente, branding }: DatosComprobanteRecepcion): jsPDF {
  const doc = new jsPDF({ format: 'a5', unit: 'mm' })
  const anchoUtil = 148 - 20
  let y = 15

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
  doc.text('COMPROBANTE DE RECEPCIÓN DE EQUIPO', 10, y)
  doc.setFont('helvetica', 'normal')
  doc.setFontSize(9)
  doc.text(`N° de OT: ${ordenTrabajo.id.slice(0, 8).toUpperCase()}`, 10, (y += 7))
  doc.text(`Fecha de recepción: ${new Date(ordenTrabajo.fechaRecepcion).toLocaleString('es-PE')}`, 10, (y += 5))
  doc.text(`Cliente: ${cliente?.nombreRazonSocial ?? '—'}`, 10, (y += 5))
  if (cliente?.telefono) doc.text(`Teléfono: ${cliente.telefono}`, 10, (y += 5))
  y += 6

  doc.line(10, y, 10 + anchoUtil, y)
  y += 6

  doc.setFont('helvetica', 'bold')
  doc.text('Equipo', 10, y)
  doc.setFont('helvetica', 'normal')
  const lineasEquipo = doc.splitTextToSize(ordenTrabajo.equipoDescripcion, anchoUtil)
  doc.text(lineasEquipo, 10, (y += 5))
  y += 4 * lineasEquipo.length + 3

  doc.setFont('helvetica', 'bold')
  doc.text('Falla reportada', 10, y)
  doc.setFont('helvetica', 'normal')
  const lineasFalla = doc.splitTextToSize(ordenTrabajo.fallaReportada, anchoUtil)
  doc.text(lineasFalla, 10, (y += 5))
  y += 4 * lineasFalla.length + 6

  doc.setFontSize(8)
  doc.setTextColor(120, 120, 120)
  doc.text('Conserve este comprobante para el retiro de su equipo.', 10, y, { maxWidth: anchoUtil })

  return doc
}
