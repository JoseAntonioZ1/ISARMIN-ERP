interface ConfirmDialogProps {
  titulo: string
  mensaje: string
  confirmando?: boolean
  onConfirmar: () => void
  onCancelar: () => void
}

/** UX-Design.md §2.4 — acciones sensibles siempre piden confirmación explícita, nunca un solo clic. */
export function ConfirmDialog({ titulo, mensaje, confirmando, onConfirmar, onCancelar }: ConfirmDialogProps) {
  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
      <div className="w-full max-w-sm rounded-lg bg-white p-6 shadow-lg dark:bg-slate-800">
        <h2 className="mb-2 text-lg font-semibold text-slate-800 dark:text-slate-100">{titulo}</h2>
        <p className="mb-6 text-sm text-slate-600 dark:text-slate-300">{mensaje}</p>
        <div className="flex justify-end gap-3">
          <button
            type="button"
            onClick={onCancelar}
            className="rounded border border-slate-300 px-4 py-2 text-sm hover:bg-slate-100 dark:border-slate-600 dark:hover:bg-slate-700"
          >
            Cancelar
          </button>
          <button
            type="button"
            onClick={onConfirmar}
            disabled={confirmando}
            className="rounded bg-red-600 px-4 py-2 text-sm text-white hover:bg-red-700 disabled:opacity-50"
          >
            {confirmando ? 'Procesando...' : 'Confirmar'}
          </button>
        </div>
      </div>
    </div>
  )
}
