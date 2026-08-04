import { render, screen } from '@testing-library/react'
import { describe, expect, it } from 'vitest'
import { EstadoBadge } from './EstadoBadge'

describe('EstadoBadge', () => {
  it('muestra "Activo" con estilo verde', () => {
    render(<EstadoBadge estado="Activo" />)

    const badge = screen.getByText('Activo')
    expect(badge).toBeInTheDocument()
    expect(badge.className).toContain('emerald')
  })

  it('muestra "Inactivo" con estilo gris', () => {
    render(<EstadoBadge estado="Inactivo" />)

    const badge = screen.getByText('Inactivo')
    expect(badge).toBeInTheDocument()
    expect(badge.className).not.toContain('emerald')
  })
})
