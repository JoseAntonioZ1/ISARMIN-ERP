using ISARMIN.Domain.Common;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Domain.Entities.Identidad;

public class Usuario : Entity
{
    /// <summary>RN-037 — intentos fallidos consecutivos antes del bloqueo temporal.</summary>
    public const int MaxIntentosFallidos = 5;

    /// <summary>RN-037 — duración del bloqueo temporal tras superar MaxIntentosFallidos.</summary>
    public static readonly TimeSpan DuracionBloqueo = TimeSpan.FromMinutes(15);

    public string Nombre { get; private set; } = null!;
    public string NombreUsuario { get; private set; } = null!;
    public string CredencialHash { get; private set; } = null!;
    public int IntentosFallidos { get; private set; }
    public DateTime? BloqueadoHasta { get; private set; }
    public EstadoRegistro Estado { get; private set; }
    public DateTime FechaCreacion { get; private set; }

    private readonly List<UsuarioRol> _roles = [];
    public IReadOnlyCollection<UsuarioRol> Roles => _roles.AsReadOnly();

    private Usuario() { }

    public Usuario(string nombre, string nombreUsuario, string credencialHash, DateTime ahora)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre del usuario es obligatorio.", nameof(nombre));
        }

        if (string.IsNullOrWhiteSpace(nombreUsuario))
        {
            throw new ArgumentException("El nombre de usuario (login) es obligatorio.", nameof(nombreUsuario));
        }

        if (string.IsNullOrWhiteSpace(credencialHash))
        {
            throw new ArgumentException("La credencial del usuario es obligatoria.", nameof(credencialHash));
        }

        Nombre = nombre;
        NombreUsuario = nombreUsuario;
        CredencialHash = credencialHash;
        Estado = EstadoRegistro.Activo;
        FechaCreacion = ahora;
    }

    public bool EstaActivo => Estado == EstadoRegistro.Activo;

    /// <summary>RN-021/RN-023 — baja lógica, nunca eliminación física.</summary>
    public void Desactivar() => Estado = EstadoRegistro.Inactivo;

    public void Activar() => Estado = EstadoRegistro.Activo;

    /// <summary>RN-021 — un usuario desactivado no puede autenticarse.</summary>
    public bool PuedeAutenticarse(DateTime ahora) => EstaActivo && !EstaBloqueado(ahora);

    public bool EstaBloqueado(DateTime ahora) => BloqueadoHasta is not null && BloqueadoHasta.Value > ahora;

    /// <summary>RN-037 — registra un intento fallido y bloquea temporalmente al superar el máximo.</summary>
    public void RegistrarIntentoFallido(DateTime ahora)
    {
        IntentosFallidos++;

        if (IntentosFallidos >= MaxIntentosFallidos)
        {
            BloqueadoHasta = ahora.Add(DuracionBloqueo);
        }
    }

    public void RegistrarInicioSesionExitoso()
    {
        IntentosFallidos = 0;
        BloqueadoHasta = null;
    }

    /// <summary>RN-038 — restablecimiento de credencial, exclusivo del Administrador.</summary>
    public void RestablecerCredencial(string nuevaCredencialHash)
    {
        if (string.IsNullOrWhiteSpace(nuevaCredencialHash))
        {
            throw new ArgumentException("La nueva credencial es obligatoria.", nameof(nuevaCredencialHash));
        }

        CredencialHash = nuevaCredencialHash;
        IntentosFallidos = 0;
        BloqueadoHasta = null;
    }
}
