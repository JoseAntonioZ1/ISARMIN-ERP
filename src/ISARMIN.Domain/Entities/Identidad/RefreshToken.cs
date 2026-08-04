using ISARMIN.Domain.Common;

namespace ISARMIN.Domain.Entities.Identidad;

/// <summary>
/// Permite renovar el JWT de acceso (corta duración) sin exigir un nuevo login.
/// Solo se persiste el hash del token; el valor crudo únicamente lo conoce el cliente.
/// </summary>
public class RefreshToken : Entity
{
    public Guid UsuarioId { get; private set; }
    public string TokenHash { get; private set; } = null!;
    public DateTime CreadoEnUtc { get; private set; }
    public DateTime ExpiraEnUtc { get; private set; }
    public DateTime? RevocadoEnUtc { get; private set; }

    private RefreshToken() { }

    public RefreshToken(Guid usuarioId, string tokenHash, DateTime creadoEnUtc, DateTime expiraEnUtc)
    {
        if (string.IsNullOrWhiteSpace(tokenHash))
        {
            throw new ArgumentException("El hash del refresh token es obligatorio.", nameof(tokenHash));
        }

        UsuarioId = usuarioId;
        TokenHash = tokenHash;
        CreadoEnUtc = creadoEnUtc;
        ExpiraEnUtc = expiraEnUtc;
    }

    public bool EsValido(DateTime ahora) => RevocadoEnUtc is null && ExpiraEnUtc > ahora;

    public void Revocar(DateTime ahora) => RevocadoEnUtc ??= ahora;
}
