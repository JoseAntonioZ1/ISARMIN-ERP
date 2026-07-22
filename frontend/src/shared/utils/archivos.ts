export const TAMANO_MAXIMO_IMAGEN_BYTES = 1.5 * 1024 * 1024 // 1.5 MB — la imagen queda embebida (Base64) en la base de datos, no como archivo aparte

export function archivoABase64(archivo: File): Promise<string> {
  return new Promise((resolve, reject) => {
    const lector = new FileReader()
    lector.onload = () => resolve(lector.result as string)
    lector.onerror = () => reject(lector.error)
    lector.readAsDataURL(archivo)
  })
}
