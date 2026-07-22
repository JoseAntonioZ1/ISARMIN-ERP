import { zodResolver } from '@hookform/resolvers/zod'
import { useMutation } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import { useNavigate } from 'react-router'
import { z } from 'zod'
import { authApi } from '@/modules/usuarios/api/authApi'
import { ApiError } from '@/shared/api/httpClient'
import { useBranding } from '@/shared/hooks/useBranding'
import { useSessionStore } from '@/shared/hooks/useSessionStore'

const esquemaLogin = z.object({
  nombreUsuario: z.string().min(1, 'Ingresa tu usuario.'),
  credencial: z.string().min(1, 'Ingresa tu contraseña.'),
})

type FormularioLogin = z.infer<typeof esquemaLogin>

/**
 * Distingue el motivo real del fallo en vez de mostrar siempre "credenciales incorrectas":
 * una API caída o un error del servidor no es lo mismo que una contraseña equivocada,
 * y confundirlos hace perder tiempo a quien intenta diagnosticar el problema.
 */
function obtenerMensajeError(error: unknown): string | null {
  if (!error) return null

  if (error instanceof ApiError) {
    if (error.status === 423) return error.message
    if (error.status === 401) return 'Usuario o contraseña incorrectos.'
    return 'Ocurrió un error inesperado al iniciar sesión. Intenta nuevamente.'
  }

  return 'No se pudo conectar con el servidor. Verifica que la API esté disponible.'
}

export function LoginPage() {
  const navigate = useNavigate()
  const establecerSesion = useSessionStore((s) => s.establecerSesion)
  const { data: branding } = useBranding()

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FormularioLogin>({ resolver: zodResolver(esquemaLogin) })

  const mutacionLogin = useMutation({
    mutationFn: authApi.login,
    onSuccess: (sesion) => {
      establecerSesion({ ...sesion.usuario, permisos: sesion.permisos }, sesion.token)
      navigate('/', { replace: true })
    },
  })

  const onSubmit = (datos: FormularioLogin) => mutacionLogin.mutate(datos)

  const mensajeError = obtenerMensajeError(mutacionLogin.error)

  return (
    <main className="flex min-h-screen items-center justify-center bg-[var(--color-fondo)] dark:bg-slate-900">
      <form
        onSubmit={handleSubmit(onSubmit)}
        className="w-full max-w-sm rounded-lg border border-slate-200 bg-white p-8 shadow-sm dark:border-slate-700 dark:bg-slate-800"
      >
        <div className="mb-6 flex flex-col items-center gap-2">
          {branding?.logo && <img src={branding.logo} alt="Logo" className="h-16 w-16 object-contain" />}
          <h1 className="text-xl font-semibold text-slate-800 dark:text-slate-100">{branding?.razonSocial ?? 'ISARMIN ERP'}</h1>
        </div>

        <label className="mb-1 block text-sm font-medium text-[var(--color-apoyo)] dark:text-slate-300">Usuario</label>
        <input
          {...register('nombreUsuario')}
          type="text"
          autoComplete="username"
          className="mb-1 w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />
        {errors.nombreUsuario && <p className="mb-2 text-sm text-red-600">{errors.nombreUsuario.message}</p>}

        <label className="mb-1 mt-3 block text-sm font-medium text-[var(--color-apoyo)] dark:text-slate-300">Contraseña</label>
        <input
          {...register('credencial')}
          type="password"
          autoComplete="current-password"
          className="mb-1 w-full rounded border border-slate-300 px-3 py-2 dark:border-slate-600 dark:bg-slate-900 dark:text-slate-100"
        />
        {errors.credencial && <p className="mb-2 text-sm text-red-600">{errors.credencial.message}</p>}

        {mensajeError && <p className="mt-3 text-sm text-red-600">{mensajeError}</p>}

        <button
          type="submit"
          disabled={mutacionLogin.isPending}
          className="mt-5 w-full rounded bg-[var(--color-principal)] py-2 text-white hover:brightness-90 disabled:opacity-50 dark:bg-[var(--color-principal)] dark:hover:brightness-110"
        >
          {mutacionLogin.isPending ? 'Ingresando...' : 'Ingresar'}
        </button>
      </form>
    </main>
  )
}
