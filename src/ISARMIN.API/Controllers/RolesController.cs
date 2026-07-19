using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Usuarios.Commands.AsignarPermisos;
using ISARMIN.Application.Modulos.Usuarios.Commands.CrearRol;
using ISARMIN.Application.Modulos.Usuarios.Commands.EditarRol;
using ISARMIN.Application.Modulos.Usuarios.DTOs;
using ISARMIN.Application.Modulos.Usuarios.Queries.ListarRoles;
using ISARMIN.Application.Modulos.Usuarios.Queries.ListarRolesConPermisos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ISARMIN.API.Controllers;

/// <summary>UC-04 — Gestionar Roles y Permisos.</summary>
[ApiController]
[Route("api/v1/roles")]
public class RolesController : ControllerBase
{
    private readonly IQueryHandler<ListarRolesQuery, IReadOnlyCollection<RolResumenDto>> _listarHandler;
    private readonly IQueryHandler<ListarRolesConPermisosQuery, IReadOnlyCollection<RolDto>> _listarConPermisosHandler;
    private readonly ICommandHandler<CrearRolCommand, RolDto> _crearHandler;
    private readonly ICommandHandler<EditarRolCommand, RolDto> _editarHandler;
    private readonly ICommandHandler<AsignarPermisosCommand, RolDto> _asignarPermisosHandler;

    public RolesController(
        IQueryHandler<ListarRolesQuery, IReadOnlyCollection<RolResumenDto>> listarHandler,
        IQueryHandler<ListarRolesConPermisosQuery, IReadOnlyCollection<RolDto>> listarConPermisosHandler,
        ICommandHandler<CrearRolCommand, RolDto> crearHandler,
        ICommandHandler<EditarRolCommand, RolDto> editarHandler,
        ICommandHandler<AsignarPermisosCommand, RolDto> asignarPermisosHandler)
    {
        _listarHandler = listarHandler;
        _listarConPermisosHandler = listarConPermisosHandler;
        _crearHandler = crearHandler;
        _editarHandler = editarHandler;
        _asignarPermisosHandler = asignarPermisosHandler;
    }

    /// <summary>Listado liviano (id, nombre) — alimenta el selector de roles del formulario de Usuarios.</summary>
    [HttpGet]
    [Authorize(Policy = "Roles.Consultar")]
    public async Task<ActionResult<IReadOnlyCollection<RolResumenDto>>> Listar(CancellationToken cancellationToken)
    {
        var roles = await _listarHandler.ManejarAsync(new ListarRolesQuery(), cancellationToken);
        return Ok(roles);
    }

    /// <summary>Listado completo con permisos — alimenta la pantalla de administración de Roles.</summary>
    [HttpGet("detalle")]
    [Authorize(Policy = "Roles.Consultar")]
    public async Task<ActionResult<IReadOnlyCollection<RolDto>>> ListarConPermisos(CancellationToken cancellationToken)
    {
        var roles = await _listarConPermisosHandler.ManejarAsync(new ListarRolesConPermisosQuery(), cancellationToken);
        return Ok(roles);
    }

    /// <summary>UC-04, paso 1 — crear un rol (nombre, descripción).</summary>
    [HttpPost]
    [Authorize(Policy = "Roles.Crear")]
    public async Task<ActionResult<RolDto>> Crear(CrearRolCommand comando, CancellationToken cancellationToken)
    {
        var rol = await _crearHandler.ManejarAsync(comando, cancellationToken);
        return CreatedAtAction(nameof(ListarConPermisos), new { }, rol);
    }

    /// <summary>UC-04, paso 1 — editar nombre/descripción de un rol existente.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Roles.Editar")]
    public async Task<ActionResult<RolDto>> Editar(Guid id, EditarRolRequest request, CancellationToken cancellationToken)
    {
        var rol = await _editarHandler.ManejarAsync(new EditarRolCommand(id, request.Nombre, request.Descripcion), cancellationToken);
        return Ok(rol);
    }

    /// <summary>UC-04, paso 2 — asignar permisos por módulo y acción (RF-010, RN-028).</summary>
    [HttpPut("{id:guid}/permisos")]
    [Authorize(Policy = "Roles.Editar")]
    public async Task<ActionResult<RolDto>> AsignarPermisos(Guid id, AsignarPermisosRequest request, CancellationToken cancellationToken)
    {
        var rol = await _asignarPermisosHandler.ManejarAsync(new AsignarPermisosCommand(id, request.Permisos), cancellationToken);
        return Ok(rol);
    }
}

public record EditarRolRequest(string Nombre, string? Descripcion);

public record AsignarPermisosRequest(IReadOnlyCollection<PermisoAsignacion> Permisos);
