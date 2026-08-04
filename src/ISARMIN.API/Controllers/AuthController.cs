using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Usuarios.Commands.CerrarSesion;
using ISARMIN.Application.Modulos.Usuarios.Commands.IniciarSesion;
using ISARMIN.Application.Modulos.Usuarios.Commands.RenovarSesion;
using ISARMIN.Application.Modulos.Usuarios.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ISARMIN.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
[EnableRateLimiting("Autenticacion")]
public class AuthController : ControllerBase
{
    private readonly ICommandHandler<IniciarSesionCommand, SesionDto> _iniciarSesionHandler;
    private readonly ICommandHandler<RenovarSesionCommand, SesionDto> _renovarSesionHandler;
    private readonly ICommandHandler<CerrarSesionCommand, Unit> _cerrarSesionHandler;

    public AuthController(
        ICommandHandler<IniciarSesionCommand, SesionDto> iniciarSesionHandler,
        ICommandHandler<RenovarSesionCommand, SesionDto> renovarSesionHandler,
        ICommandHandler<CerrarSesionCommand, Unit> cerrarSesionHandler)
    {
        _iniciarSesionHandler = iniciarSesionHandler;
        _renovarSesionHandler = renovarSesionHandler;
        _cerrarSesionHandler = cerrarSesionHandler;
    }

    /// <summary>
    /// UC-01 — Iniciar Sesión. Errores esperados (credenciales inválidas, cuenta bloqueada)
    /// se traducen a la respuesta HTTP correspondiente en GlobalExceptionHandler.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<SesionDto>> Login(IniciarSesionCommand comando, CancellationToken cancellationToken)
    {
        var sesion = await _iniciarSesionHandler.ManejarAsync(comando, cancellationToken);
        return Ok(sesion);
    }

    /// <summary>
    /// Renueva el JWT de acceso (60 min) usando el refresh token (7 días) entregado en el login,
    /// evitando que el usuario tenga que volver a loguearse cada hora durante su jornada.
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<SesionDto>> Refresh(RenovarSesionCommand comando, CancellationToken cancellationToken)
    {
        var sesion = await _renovarSesionHandler.ManejarAsync(comando, cancellationToken);
        return Ok(sesion);
    }

    /// <summary>
    /// UC-02 — Cerrar Sesión. El JWT es sin estado (sin blacklist/Redis en el stack); lo único
    /// que se puede invalidar del lado del servidor es el refresh token asociado.
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CerrarSesionCommand comando, CancellationToken cancellationToken)
    {
        await _cerrarSesionHandler.ManejarAsync(comando, cancellationToken);
        return NoContent();
    }
}
