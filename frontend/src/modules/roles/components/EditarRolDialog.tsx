import { useState } from 'react'
import { ACCIONES, type Accion, type Permiso, type Rol } from '@/modules/roles/api/rolesApi'

interface EditarRolDialogProps {
  rol: Rol
  modulosConocidos: string[]
  onGuardarDatos: (datos: { nombre: string; descripcion?: string }) => void
  onGuardarPermisos: (permisos: Permiso[]) => void
  onCancelar: () => void
  guardandoDatos?: boolean
  guardandoPermisos?: boolean
}

function clavePermiso(modulo: string, accion: Accion) {
  return `${modulo}.${accion}`
}

export function EditarRolDialog({
  rol,
  modulosConocidos,
  onGuardarDatos,
  onGuardarPermisos,
  onCancelar,
  guardandoDatos,
  guardandoPermisos,
}: EditarRolDialogProps) {
  const [nombre, setNombre] = useState(rol.nombre)
  const [descripcion, setDescripcion] = useState(rol.descripcion ?? '')
  const [seleccionados, setSeleccionados] = useState(
    () => new Set(rol.permisos.map((p) => clavePermiso(p.modulo, p.accion))),
  )
  const [modulos, setModulos] = useState(() =>
    Array.from(new Set([...modulosConocidos, ...rol.permisos.map((p) => p.modulo)])).sort(),
  )
  const [moduloNuevo, setModuloNuevo] = useState('')

  const alternar = (modulo: string, accion: Accion) => {
    const clave = clavePermiso(modulo, accion)
    setSeleccionados((actual) => {
      const siguiente = new Set(actual)
      if (siguiente.has(clave)) {
        siguiente.delete(clave)
      } else {
        siguiente.add(clave)
      }
      return siguiente
    })
  }

  const agregarModulo = () => {
    const nombreModulo = moduloNuevo.trim()
    if (!nombreModulo || modulos.includes(nombreModulo)) return
    setModulos((actual) => [...actual, nombreModulo].sort())
    setModuloNuevo('')
  }

  const handleGuardarPermisos = () => {
    const permisos: Permiso[] = []
    for (const clave of seleccionados) {
      const separador = clave.lastIndexOf('.')
      permisos.push({ modulo: clave.slice(0, separador), accion: clave.slice(separador + 1) as Accion })
    }
    onGuardarPermisos(permisos)
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
      <div className="max-h-[90vh] w-full max-w-2xl overflow-y-auto rounded-lg bg-white p-6 shadow-lg dark:bg-slate-800">
        <h2 className="mb-4 text-lg font-semibold text-slate-800 dark:text-slate-100">Editar rol</h2>

        <label className="mb-1 block text-sm font-medium text-slate-700 dark:text-slate-300">Nombre</label>
        <input
          value={nombre}
          onChange={(e) => setNombre(e.target.value)}
          className="mb-1 w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />

        <label className="mb-1 mt-3 block text-sm font-medium text-slate-700 dark:text-slate-300">Descripción</label>
        <textarea
          value={descripcion}
          onChange={(e) => setDescripcion(e.target.value)}
          rows={2}
          className="mb-1 w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />

        <div className="mt-2 flex justify-end">
          <button
            type="button"
            onClick={() => onGuardarDatos({ nombre, descripcion: descripcion || undefined })}
            disabled={guardandoDatos}
            className="rounded border border-slate-300 px-3 py-1.5 text-sm hover:bg-slate-100 disabled:opacity-50 dark:border-slate-600 dark:hover:bg-slate-700"
          >
            {guardandoDatos ? 'Guardando...' : 'Guardar nombre/descripción'}
          </button>
        </div>

        <hr className="my-4 border-slate-200 dark:border-slate-700" />

        <h3 className="mb-2 text-sm font-semibold text-slate-800 dark:text-slate-100">Permisos por módulo</h3>

        <div className="mb-3 overflow-x-auto">
          <table className="w-full border-collapse text-left text-sm">
            <thead>
              <tr>
                <th className="px-2 py-1">Módulo</th>
                {ACCIONES.map((accion) => (
                  <th key={accion} className="px-2 py-1 text-center">
                    {accion}
                  </th>
                ))}
              </tr>
            </thead>
            <tbody>
              {modulos.map((modulo) => (
                <tr key={modulo} className="border-t border-slate-200 dark:border-slate-700">
                  <td className="px-2 py-1">{modulo}</td>
                  {ACCIONES.map((accion) => (
                    <td key={accion} className="px-2 py-1 text-center">
                      <input
                        type="checkbox"
                        checked={seleccionados.has(clavePermiso(modulo, accion))}
                        onChange={() => alternar(modulo, accion)}
                      />
                    </td>
                  ))}
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        <div className="mb-3 flex gap-2">
          <input
            value={moduloNuevo}
            onChange={(e) => setModuloNuevo(e.target.value)}
            placeholder="Nombre de un módulo nuevo (ej. Ventas)"
            className="flex-1 rounded border border-slate-300 px-3 py-1.5 text-sm dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
          />
          <button
            type="button"
            onClick={agregarModulo}
            className="rounded border border-slate-300 px-3 py-1.5 text-sm hover:bg-slate-100 dark:border-slate-600 dark:hover:bg-slate-700"
          >
            Agregar módulo
          </button>
        </div>

        <div className="flex justify-between">
          <button
            type="button"
            onClick={onCancelar}
            className="rounded border border-slate-300 px-4 py-2 text-sm hover:bg-slate-100 dark:border-slate-600 dark:hover:bg-slate-700"
          >
            Cerrar
          </button>
          <button
            type="button"
            onClick={handleGuardarPermisos}
            disabled={guardandoPermisos}
            className="rounded bg-[var(--color-acento)] px-4 py-2 text-sm text-white hover:brightness-90 disabled:opacity-50 dark:bg-[var(--color-acento)] dark:hover:brightness-110"
          >
            {guardandoPermisos ? 'Guardando...' : 'Guardar permisos'}
          </button>
        </div>
      </div>
    </div>
  )
}
