using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Usuarios.Commands.CambiarEstadoUsuario;
using ISARMIN.Application.Modulos.Usuarios.Commands.CrearUsuario;
using ISARMIN.Application.Modulos.Usuarios.Commands.EditarUsuario;
using ISARMIN.Application.Modulos.Usuarios.Commands.RestablecerCredencial;
using ISARMIN.Application.Modulos.Usuarios.DTOs;
using ISARMIN.Application.Modulos.Usuarios.Queries.ListarUsuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ISARMIN.API.Controllers;

[ApiController]
[Route("api/v1/usuarios")]
public class UsuariosController : ControllerBase
{
    private readonly ICommandHandler<CrearUsuarioCommand, UsuarioDto> _crearHandler;
    private readonly ICommandHandler<EditarUsuarioCommand, UsuarioDto> _editarHandler;
    private readonly ICommandHandler<CambiarEstadoUsuarioCommand, Unit> _cambiarEstadoHandler;
    private readonly ICommandHandler<RestablecerCredencialCommand, Unit> _restablecerCredencialHandler;
    private readonly IQueryHandler<ListarUsuariosQuery, ListadoPaginadoDto<UsuarioDto>> _listarHandler;

    public UsuariosController(
        ICommandHandler<CrearUsuarioCommand, UsuarioDto> crearHandler,
        ICommandHandler<EditarUsuarioCommand, UsuarioDto> editarHandler,
        ICommandHandler<CambiarEstadoUsuarioCommand, Unit> cambiarEstadoHandler,
        ICommandHandler<RestablecerCredencialCommand, Unit> restablecerCredencialHandler,
        IQueryHandler<ListarUsuariosQuery, ListadoPaginadoDto<UsuarioDto>> listarHandler)
    {
        _crearHandler = crearHandler;
        _editarHandler = editarHandler;
        _cambiarEstadoHandler = cambiarEstadoHandler;
        _restablecerCredencialHandler = restablecerCredencialHandler;
        _listarHandler = listarHandler;
    }

    /// <summary>UC-03 — listado de usuarios.</summary>
    [HttpGet]
    [Authorize(Policy = "Usuarios.Consultar")]
    public async Task<ActionResult<ListadoPaginadoDto<UsuarioDto>>> Listar(
        [FromQuery] int pagina = 1, [FromQuery] int tamanoPagina = 20, CancellationToken cancellationToken = default)
    {
        var resultado = await _listarHandler.ManejarAsync(new ListarUsuariosQuery(pagina, tamanoPagina), cancellationToken);
        return Ok(resultado);
    }

    /// <summary>UC-03 — registrar un nuevo usuario (RF-001).</summary>
    [HttpPost]
    [Authorize(Policy = "Usuarios.Crear")]
    public async Task<ActionResult<UsuarioDto>> Crear(CrearUsuarioCommand comando, CancellationToken cancellationToken)
    {
        var usuario = await _crearHandler.ManejarAsync(comando, cancellationToken);
        return CreatedAtAction(nameof(Listar), new { }, usuario);
    }

    /// <summary>UC-03 — editar nombre y roles de un usuario existente (RF-002).</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Usuarios.Editar")]
    public async Task<ActionResult<UsuarioDto>> Editar(Guid id, EditarUsuarioRequest request, CancellationToken cancellationToken)
    {
        var usuario = await _editarHandler.ManejarAsync(new EditarUsuarioCommand(id, request.Nombre, request.RolIds), cancellationToken);
        return Ok(usuario);
    }

    /// <summary>UC-03 — baja lógica / reactivación de un usuario (RF-003, RN-021).</summary>
    [HttpPatch("{id:guid}/estado")]
    [Authorize(Policy = "Usuarios.Eliminar")]
    public async Task<IActionResult> CambiarEstado(Guid id, CambiarEstadoUsuarioRequest request, CancellationToken cancellationToken)
    {
        await _cambiarEstadoHandler.ManejarAsync(new CambiarEstadoUsuarioCommand(id, request.Activo), cancellationToken);
        return NoContent();
    }

    /// <summary>RN-038 — restablecimiento de contraseña, exclusivo del Administrador.</summary>
    [HttpPatch("{id:guid}/restablecer-credencial")]
    [Authorize(Policy = "Usuarios.Editar")]
    public async Task<IActionResult> RestablecerCredencial(Guid id, RestablecerCredencialRequest request, CancellationToken cancellationToken)
    {
        await _restablecerCredencialHandler.ManejarAsync(new RestablecerCredencialCommand(id, request.NuevaCredencial), cancellationToken);
        return NoContent();
    }
}

public record EditarUsuarioRequest(string Nombre, IReadOnlyCollection<Guid> RolIds);

public record CambiarEstadoUsuarioRequest(bool Activo);

public record RestablecerCredencialRequest(string NuevaCredencial);
