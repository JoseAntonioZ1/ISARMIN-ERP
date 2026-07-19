using ISARMIN.Infrastructure.Auth;
using Microsoft.AspNetCore.Authorization;

namespace ISARMIN.API.Authorization;

public class PermisoAuthorizationHandler : AuthorizationHandler<PermisoRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermisoRequirement requirement)
    {
        var tienePermiso = context.User.Claims.Any(c =>
            c.Type == GeneradorTokenJwt.TipoClaimPermiso && c.Value == requirement.Permiso);

        if (tienePermiso)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
