using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Configuracion.Commands.CambiarEstadoMedioPago;
using ISARMIN.Application.Modulos.Configuracion.Commands.CrearMedioPago;
using ISARMIN.Application.Modulos.Configuracion.DTOs;
using ISARMIN.Application.Modulos.Configuracion.Queries.ListarMediosPago;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ISARMIN.API.Controllers;

/// <summary>CAT-008 — catálogo configurable de medios de pago (RF-042, UC-37).</summary>
[ApiController]
[Route("api/v1/configuracion/medios-pago")]
public class MediosPagoController : ControllerBase
{
    private readonly IQueryHandler<ListarMediosPagoQuery, IReadOnlyCollection<MedioPagoDto>> _listarHandler;
    private readonly ICommandHandler<CrearMedioPagoCommand, MedioPagoDto> _crearHandler;
    private readonly ICommandHandler<CambiarEstadoMedioPagoCommand, Unit> _cambiarEstadoHandler;

    public MediosPagoController(
        IQueryHandler<ListarMediosPagoQuery, IReadOnlyCollection<MedioPagoDto>> listarHandler,
        ICommandHandler<CrearMedioPagoCommand, MedioPagoDto> crearHandler,
        ICommandHandler<CambiarEstadoMedioPagoCommand, Unit> cambiarEstadoHandler)
    {
        _listarHandler = listarHandler;
        _crearHandler = crearHandler;
        _cambiarEstadoHandler = cambiarEstadoHandler;
    }

    [HttpGet]
    [Authorize(Policy = "Configuracion.Consultar")]
    public async Task<ActionResult<IReadOnlyCollection<MedioPagoDto>>> Listar(CancellationToken cancellationToken)
    {
        var mediosPago = await _listarHandler.ManejarAsync(new ListarMediosPagoQuery(), cancellationToken);
        return Ok(mediosPago);
    }

    [HttpPost]
    [Authorize(Policy = "Configuracion.Editar")]
    public async Task<ActionResult<MedioPagoDto>> Crear(CrearMedioPagoCommand comando, CancellationToken cancellationToken)
    {
        var medioPago = await _crearHandler.ManejarAsync(comando, cancellationToken);
        return CreatedAtAction(nameof(Listar), new { }, medioPago);
    }

    [HttpPatch("{id:guid}/estado")]
    [Authorize(Policy = "Configuracion.Editar")]
    public async Task<IActionResult> CambiarEstado(Guid id, CambiarEstadoMedioPagoRequest request, CancellationToken cancellationToken)
    {
        await _cambiarEstadoHandler.ManejarAsync(new CambiarEstadoMedioPagoCommand(id, request.Activo), cancellationToken);
        return NoContent();
    }
}

public record CambiarEstadoMedioPagoRequest(bool Activo);
