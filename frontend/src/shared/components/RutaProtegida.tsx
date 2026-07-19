import type { ReactNode } from 'react'
import { Navigate } from 'react-router'
import { useSessionStore } from '@/shared/hooks/useSessionStore'

export function RutaProtegida({ children }: { children: ReactNode }) {
  const token = useSessionStore((s) => s.token)

  if (!token) {
    return <Navigate to="/login" replace />
  }

  return children
}
