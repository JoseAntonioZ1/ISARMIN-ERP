using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Usuarios.Commands.IniciarSesion;
using ISARMIN.Application.Modulos.Usuarios.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ISARMIN.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly ICommandHandler<IniciarSesionCommand, SesionDto> _iniciarSesionHandler;

    public AuthController(ICommandHandler<IniciarSesionCommand, SesionDto> iniciarSesionHandler)
    {
        _iniciarSesionHandler = iniciarSesionHandler;
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
    /// UC-02 — Cerrar Sesión. El JWT es sin estado (sin blacklist/Redis en el stack); no hay
    /// nada que invalidar del lado del servidor. El cliente descarta el token.
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout() => NoContent();
}
