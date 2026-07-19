using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ISARMIN.Application.Common;
using ISARMIN.Domain.Entities.Identidad;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ISARMIN.Infrastructure.Auth;

public class GeneradorTokenJwt : IGeneradorTokenJwt
{
    public const string TipoClaimPermiso = "permiso";

    private readonly IConfiguration _configuration;

    public GeneradorTokenJwt(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public TokenGenerado Generar(Usuario usuario, IReadOnlyCollection<string> permisos)
    {
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var key = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("No se encontró la configuración 'Jwt:Key'.");
        var expiracionMinutos = int.TryParse(_configuration["Jwt:ExpiracionMinutos"], out var minutos) ? minutos : 60;

        var expiraEnUtc = DateTime.UtcNow.AddMinutes(expiracionMinutos);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nombre),
        };
        claims.AddRange(permisos.Select(permiso => new Claim(TipoClaimPermiso, permiso)));

        var credenciales = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiraEnUtc,
            signingCredentials: credenciales);

        var tokenSerializado = new JwtSecurityTokenHandler().WriteToken(token);

        return new TokenGenerado(tokenSerializado, expiraEnUtc);
    }
}
