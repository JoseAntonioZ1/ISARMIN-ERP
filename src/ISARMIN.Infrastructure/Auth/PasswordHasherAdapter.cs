using ISARMIN.Application.Common;
using ISARMIN.Domain.Entities.Identidad;
using Microsoft.AspNetCore.Identity;

namespace ISARMIN.Infrastructure.Auth;

/// <summary>Adapta PasswordHasher&lt;Usuario&gt; (PBKDF2, Microsoft.Extensions.Identity.Core) al puerto IPasswordHasher de Application — RNF-011.</summary>
public class PasswordHasherAdapter : IPasswordHasher
{
    private readonly PasswordHasher<Usuario> _passwordHasher = new();

    public string HashearCredencial(string credencialPlana) =>
        _passwordHasher.HashPassword(null!, credencialPlana);

    public bool VerificarCredencial(string credencialHash, string credencialPlana)
    {
        var resultado = _passwordHasher.VerifyHashedPassword(null!, credencialHash, credencialPlana);
        return resultado != PasswordVerificationResult.Failed;
    }
}
