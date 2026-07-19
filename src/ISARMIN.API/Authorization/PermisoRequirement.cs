using Microsoft.AspNetCore.Authorization;

namespace ISARMIN.API.Authorization;

/// <summary>ADR-008 — un permiso lógico (ej. "Inventario.Ajustar"), nunca un rol fijo.</summary>
public class PermisoRequirement : IAuthorizationRequirement
{
    public string Permiso { get; }

    public PermisoRequirement(string permiso)
    {
        Permiso = permiso;
    }
}
