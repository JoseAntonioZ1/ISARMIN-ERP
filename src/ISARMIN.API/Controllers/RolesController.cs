using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Usuarios.DTOs;
using ISARMIN.Application.Modulos.Usuarios.Queries.ListarRoles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ISARMIN.API.Controllers;

/// <summary>
/// Solo lectura por ahora: el CRUD de roles/permisos (UC-04) es un módulo posterior.
/// Este endpoint existe para alimentar el selector de roles del formulario de Usuarios.
/// </summary>
[ApiController]
[Route("api/v1/roles")]
public class RolesController : ControllerBase
{
    private readonly IQueryHandler<ListarRolesQuery, IReadOnlyCollection<RolResumenDto>> _listarHandler;

    public RolesController(IQueryHandler<ListarRolesQuery, IReadOnlyCollection<RolResumenDto>> listarHandler)
    {
        _listarHandler = listarHandler;
    }

    [HttpGet]
    [Authorize(Policy = "Roles.Consultar")]
    public async Task<ActionResult<IReadOnlyCollection<RolResumenDto>>> Listar(CancellationToken cancellationToken)
    {
        var roles = await _listarHandler.ManejarAsync(new ListarRolesQuery(), cancellationToken);
        return Ok(roles);
    }
}
