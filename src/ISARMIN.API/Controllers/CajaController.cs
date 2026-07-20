using System.Security.Claims;
using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Caja.Commands.AbrirCaja;
using ISARMIN.Application.Modulos.Caja.Commands.CerrarCaja;
using ISARMIN.Application.Modulos.Caja.Commands.RegistrarMovimientoCaja;
using ISARMIN.Application.Modulos.Caja.DTOs;
using ISARMIN.Application.Modulos.Caja.Queries.ListarMovimientosCaja;
using ISARMIN.Application.Modulos.Caja.Queries.ObtenerCajaActual;
using ISARMIN.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ISARMIN.API.Controllers;

/// <summary>UC-19 — Abrir/Cerrar Caja (RF-046 a RF-049); UC-20 — Registrar Movimiento de Caja (RF-047).</summary>
[ApiController]
[Route("api/v1/caja")]
public class CajaController : ControllerBase
{
    private readonly ICommandHandler<AbrirCajaCommand, CajaDto> _abrirHandler;
    private readonly ICommandHandler<CerrarCajaCommand, CajaDto> _cerrarHandler;
    private readonly ICommandHandler<RegistrarMovimientoCajaCommand, MovimientoCajaDto> _registrarMovimientoHandler;
    private readonly IQueryHandler<ObtenerCajaActualQuery, CajaDto?> _obtenerActualHandler;
    private readonly IQueryHandler<ListarMovimientosCajaQuery, IReadOnlyCollection<MovimientoCajaDto>> _listarMovimientosHandler;

    public CajaController(
        ICommandHandler<AbrirCajaCommand, CajaDto> abrirHandler,
        ICommandHandler<CerrarCajaCommand, CajaDto> cerrarHandler,
        ICommandHandler<RegistrarMovimientoCajaCommand, MovimientoCajaDto> registrarMovimientoHandler,
        IQueryHandler<ObtenerCajaActualQuery, CajaDto?> obtenerActualHandler,
        IQueryHandler<ListarMovimientosCajaQuery, IReadOnlyCollection<MovimientoCajaDto>> listarMovimientosHandler)
    {
        _abrirHandler = abrirHandler;
        _cerrarHandler = cerrarHandler;
        _registrarMovimientoHandler = registrarMovimientoHandler;
        _obtenerActualHandler = obtenerActualHandler;
        _listarMovimientosHandler = listarMovimientosHandler;
    }

    /// <summary>Caja abierta actualmente o, si no hay ninguna abierta, la más reciente (cerrada).</summary>
    [HttpGet]
    [Authorize(Policy = "Caja.Consultar")]
    public async Task<ActionResult<CajaDto?>> ObtenerActual(CancellationToken cancellationToken)
    {
        var caja = await _obtenerActualHandler.ManejarAsync(new ObtenerCajaActualQuery(), cancellationToken);
        return Ok(caja);
    }

    [HttpPost("apertura")]
    [Authorize(Policy = "Caja.Abrir")]
    public async Task<ActionResult<CajaDto>> AbrirCaja(AbrirCajaRequest request, CancellationToken cancellationToken)
    {
        var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var caja = await _abrirHandler.ManejarAsync(new AbrirCajaCommand(request.MontoApertura, usuarioId), cancellationToken);
        return CreatedAtAction(nameof(ObtenerActual), null, caja);
    }

    [HttpPost("cierre")]
    [Authorize(Policy = "Caja.Cerrar")]
    public async Task<ActionResult<CajaDto>> CerrarCaja(CerrarCajaRequest request, CancellationToken cancellationToken)
    {
        var caja = await _cerrarHandler.ManejarAsync(new CerrarCajaCommand(request.MontoFisicoDeclarado), cancellationToken);
        return Ok(caja);
    }

    [HttpGet("movimientos")]
    [Authorize(Policy = "Caja.Consultar")]
    public async Task<ActionResult<IReadOnlyCollection<MovimientoCajaDto>>> ListarMovimientos(
        [FromQuery] Guid? cajaId, [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, CancellationToken cancellationToken)
    {
        var movimientos = await _listarMovimientosHandler.ManejarAsync(new ListarMovimientosCajaQuery(cajaId, desde, hasta), cancellationToken);
        return Ok(movimientos);
    }

    /// <summary>RN-026 — egreso/ingreso manual sin origen transaccional (gasto operativo, retiro del
    /// propietario o aporte de capital).</summary>
    [HttpPost("movimientos")]
    [Authorize(Policy = "Caja.Registrar")]
    public async Task<ActionResult<MovimientoCajaDto>> RegistrarMovimiento(RegistrarMovimientoCajaRequest request, CancellationToken cancellationToken)
    {
        var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var concepto = Enum.Parse<ConceptoMovimientoCaja>(request.Concepto);
        var comando = new RegistrarMovimientoCajaCommand(concepto, request.Monto, request.Descripcion, usuarioId);
        var movimiento = await _registrarMovimientoHandler.ManejarAsync(comando, cancellationToken);
        return CreatedAtAction(nameof(ListarMovimientos), null, movimiento);
    }
}

public record AbrirCajaRequest(decimal MontoApertura);

public record CerrarCajaRequest(decimal MontoFisicoDeclarado);

public record RegistrarMovimientoCajaRequest(string Concepto, decimal Monto, string? Descripcion);
