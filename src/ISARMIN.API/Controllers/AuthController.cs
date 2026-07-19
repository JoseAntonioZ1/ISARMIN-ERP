using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
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

    /// <summary>UC-01 — Iniciar Sesión.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<SesionDto>> Login(IniciarSesionCommand comando, CancellationToken cancellationToken)
    {
        try
        {
            var sesion = await _iniciarSesionHandler.ManejarAsync(comando, cancellationToken);
            return Ok(sesion);
        }
        catch (CredencialesInvalidasException)
        {
            return Unauthorized(new
            {
                error = new { codigo = "CREDENCIALES_INVALIDAS", mensaje = "El nombre de usuario o la credencial son incorrectos.", detalles = (object?)null }
            });
        }
        catch (CuentaBloqueadaException ex)
        {
            return StatusCode(StatusCodes.Status423Locked, new
            {
                error = new
                {
                    codigo = "CUENTA_BLOQUEADA_TEMPORALMENTE",
                    mensaje = $"Cuenta bloqueada por intentos fallidos. Intente nuevamente después de las {ex.BloqueadoHastaUtc:HH:mm}.",
                    detalles = (object?)null
                }
            });
        }
    }

    /// <summary>
    /// UC-02 — Cerrar Sesión. El JWT es sin estado (sin blacklist/Redis en el stack); no hay
    /// nada que invalidar del lado del servidor. El cliente descarta el token.
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout() => NoContent();
}
