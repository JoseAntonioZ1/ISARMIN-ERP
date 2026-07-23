/** Exportación genérica a CSV — sin librería nueva, funciona en cualquier reporte tabular.
 * El BOM (﻿) al inicio hace que Excel detecte UTF-8 correctamente (tildes, ñ). */
export function exportarCsv(nombreArchivo: string, encabezados: string[], filas: (string | number)[][]) {
  const escapar = (valor: string | number) => {
    const texto = String(valor)
    return /[",\n]/.test(texto) ? `"${texto.replace(/"/g, '""')}"` : texto
  }

  const contenido = [encabezados, ...filas].map((fila) => fila.map(escapar).join(',')).join('\n')
  const blob = new Blob([`﻿${contenido}`], { type: 'text/csv;charset=utf-8;' })
  const url = URL.createObjectURL(blob)
  const enlace = document.createElement('a')
  enlace.href = url
  enlace.download = nombreArchivo
  enlace.click()
  URL.revokeObjectURL(url)
}
