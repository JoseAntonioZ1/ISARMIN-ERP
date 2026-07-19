using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace ISARMIN.API.Authorization;

/// <summary>
/// Convierte cualquier `[Authorize(Policy = "Modulo.Accion")]` en un PermisoRequirement dinámico,
/// sin necesidad de registrar cada permiso lógico como política explícita (ADR-008).
/// </summary>
public class PermisoAuthorizationPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _proveedorPorDefecto;

    public PermisoAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _proveedorPorDefecto = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _proveedorPorDefecto.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _proveedorPorDefecto.GetFallbackPolicyAsync();

    public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var politicaExistente = await _proveedorPorDefecto.GetPolicyAsync(policyName);
        if (politicaExistente is not null)
        {
            return politicaExistente;
        }

        return new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new PermisoRequirement(policyName))
            .Build();
    }
}
