import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useEffect, useState } from 'react'
import { type Categoria, categoriasApi } from '@/modules/catalogos/api/catalogosApi'
import { CategoriaDialog } from '@/modules/catalogos/components/CategoriaDialog'
import { ApiError } from '@/shared/api/httpClient'
import { EstadoCarga } from '@/shared/components/EstadoCarga'
import { EstadoVacio } from '@/shared/components/EstadoVacio'
import { useToast } from '@/shared/hooks/useToast'

export function CategoriasPage() {
  const queryClient = useQueryClient()
  const { mostrarExito, mostrarError } = useToast()
  const [creando, setCreando] = useState(false)
  const [editando, setEditando] = useState<Categoria | null>(null)

  const {
    data: categorias,
    isLoading,
    error: errorListado,
  } = useQuery({
    queryKey: ['categorias'],
    queryFn: categoriasApi.listar,
  })

  useEffect(() => {
    if (errorListado) mostrarError('No se pudo cargar la lista de categorías.')
  }, [errorListado, mostrarError])

  const invalidar = () => queryClient.invalidateQueries({ queryKey: ['categorias'] })

  const mutacionCrear = useMutation({
    mutationFn: categoriasApi.crear,
    onSuccess: () => {
      invalidar()
      setCreando(false)
      mostrarExito('Categoría creada correctamente.')
    },
    onError: (e) => mostrarError(e instanceof ApiError ? e.message : 'No se pudo crear la categoría.'),
  })

  const mutacionEditar = useMutation({
    mutationFn: ({ id, datos }: { id: string; datos: { nombre: string; categoriaPadreId: string | null } }) =>
      categoriasApi.editar(id, datos),
    onSuccess: () => {
      invalidar()
      setEditando(null)
      mostrarExito('Categoría actualizada correctamente.')
    },
    onError: (e) => mostrarError(e instanceof ApiError ? e.message : 'No se pudo editar la categoría.'),
  })

  const nombreCategoria = (id: string | null) => categorias?.find((c) => c.id === id)?.nombre ?? '—'

  return (
    <div>
      <div className="mb-4 flex items-center justify-between">
        <h1 className="text-xl font-semibold text-slate-800 dark:text-slate-100">Categorías de Producto</h1>
        <button
          type="button"
          onClick={() => setCreando(true)}
          className="rounded bg-[var(--color-principal)] px-4 py-2 text-sm text-white hover:brightness-90 dark:bg-[var(--color-principal)] dark:hover:brightness-110"
        >
          Nueva categoría
        </button>
      </div>

      {isLoading ? (
        <EstadoCarga />
      ) : categorias?.length === 0 ? (
        <EstadoVacio mensaje="No hay categorías registradas." />
      ) : (
        <table className="w-full border-collapse overflow-hidden rounded-lg bg-white text-left text-sm shadow-sm dark:bg-slate-800">
          <thead className="bg-slate-100 dark:bg-slate-700">
            <tr>
              <th className="px-4 py-2">Nombre</th>
              <th className="px-4 py-2">Categoría padre</th>
              <th className="px-4 py-2">Acciones</th>
            </tr>
          </thead>
          <tbody>
            {categorias?.map((categoria) => (
              <tr key={categoria.id} className="border-t border-slate-200 dark:border-slate-700">
                <td className="px-4 py-2">{categoria.nombre}</td>
                <td className="px-4 py-2">{nombreCategoria(categoria.categoriaPadreId)}</td>
                <td className="px-4 py-2">
                  <button
                    type="button"
                    onClick={() => setEditando(categoria)}
                    className="text-[var(--color-apoyo)] underline hover:text-slate-900 dark:text-slate-300"
                  >
                    Editar
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {creando && (
        <CategoriaDialog
          categorias={categorias ?? []}
          onGuardar={(datos) => mutacionCrear.mutate(datos)}
          onCancelar={() => setCreando(false)}
          guardando={mutacionCrear.isPending}
        />
      )}

      {editando && (
        <CategoriaDialog
          categoria={editando}
          categorias={categorias ?? []}
          onGuardar={(datos) => mutacionEditar.mutate({ id: editando.id, datos })}
          onCancelar={() => setEditando(null)}
          guardando={mutacionEditar.isPending}
        />
      )}
    </div>
  )
}
