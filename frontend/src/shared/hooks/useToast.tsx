import { CircleCheck, CircleX } from 'lucide-react'
import { createContext, useCallback, useContext, useState } from 'react'
import type { ReactNode } from 'react'

interface ToastItem {
  id: number
  tipo: 'exito' | 'error'
  mensaje: string
}

interface ToastContextValue {
  mostrar: (tipo: ToastItem['tipo'], mensaje: string) => void
}

const ToastContext = createContext<ToastContextValue | null>(null)

const DURACION_MS = 4000

/** Reemplaza el patrón `useState<string|null>` + `<p roja>` reinventado en cada página — también
 * agrega confirmación de éxito, que antes no existía en ningún lado (el usuario solo inferia que
 * algo funcionó porque el diálogo se cerraba). Se monta una sola vez en App.tsx. */
export function ToastProvider({ children }: { children: ReactNode }) {
  const [toasts, setToasts] = useState<ToastItem[]>([])

  const mostrar = useCallback((tipo: ToastItem['tipo'], mensaje: string) => {
    const id = Date.now() + Math.random()
    setToasts((actual) => [...actual, { id, tipo, mensaje }])
    setTimeout(() => setToasts((actual) => actual.filter((t) => t.id !== id)), DURACION_MS)
  }, [])

  return (
    <ToastContext.Provider value={{ mostrar }}>
      {children}
      <div className="pointer-events-none fixed bottom-4 right-4 z-[100] flex flex-col gap-2">
        {toasts.map((toast) => {
          const Icono = toast.tipo === 'exito' ? CircleCheck : CircleX
          return (
            <div
              key={toast.id}
              className={`pointer-events-auto flex items-center gap-2 rounded-lg px-4 py-3 text-sm text-white shadow-lg ${
                toast.tipo === 'exito' ? 'bg-emerald-600' : 'bg-[var(--color-principal)]'
              }`}
            >
              <Icono className="h-4 w-4 flex-shrink-0" />
              {toast.mensaje}
            </div>
          )
        })}
      </div>
    </ToastContext.Provider>
  )
}

export function useToast() {
  const contexto = useContext(ToastContext)
  if (!contexto) {
    throw new Error('useToast debe usarse dentro de un ToastProvider')
  }
  return {
    mostrarExito: (mensaje: string) => contexto.mostrar('exito', mensaje),
    mostrarError: (mensaje: string) => contexto.mostrar('error', mensaje),
  }
}
