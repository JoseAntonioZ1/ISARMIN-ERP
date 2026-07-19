import { useQuery } from '@tanstack/react-query'
import type { Path, UseFormRegister } from 'react-hook-form'
import { rolesApi } from '@/modules/usuarios/api/usuariosApi'

interface CampoRoles {
  rolIds: string[]
}

export function SelectorRoles<T extends CampoRoles>({
  register,
  error,
}: {
  register: UseFormRegister<T>
  error?: string
}) {
  const { data: roles } = useQuery({ queryKey: ['roles'], queryFn: rolesApi.listar })

  return (
    <div>
      <label className="mb-1 mt-3 block text-sm font-medium text-slate-700 dark:text-slate-300">Roles</label>
      <div className="mb-1 flex flex-col gap-1">
        {roles?.map((rol) => (
          <label key={rol.id} className="flex items-center gap-2 text-sm text-slate-700 dark:text-slate-300">
            <input type="checkbox" value={rol.id} {...register('rolIds' as Path<T>)} />
            {rol.nombre}
          </label>
        ))}
      </div>
      {error && <p className="mb-2 text-sm text-red-600">{error}</p>}
    </div>
  )
}
