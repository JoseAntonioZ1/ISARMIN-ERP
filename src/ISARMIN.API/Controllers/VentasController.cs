using System.Security.Claims;
using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Ventas.Commands.AnularVenta;
using ISARMIN.Application.Modulos.Ventas.Commands.RegistrarDevolucion;
using ISARMIN.Application.Modulos.Ventas.Commands.RegistrarVenta;
using ISARMIN.Application.Modulos.Ventas.DTOs;
using ISARMIN.Application.Modulos.Ventas.Queries.BuscarVentas;
using ISARMIN.Application.Modulos.Ventas.Queries.ObtenerVenta;
using ISARMIN.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ISARMIN.API.Controllers;

/// <summary>UC-14, UC-16, UC-17 — Ventas (RF-038 a RF-043, RF-089, RF-091).</summary>
[ApiController]
[Route("api/v1/ventas")]
public class VentasController : ControllerBase
{
    private readonly ICommandHandler<RegistrarVentaCommand, VentaDto> _registrarHandler;
    private readonly ICommandHandler<AnularVentaCommand, VentaDto> _anularHandler;
    private readonly ICommandHandler<RegistrarDevolucionCommand, VentaDto> _registrarDevolucionHandler;
    private readonly IQueryHandler<BuscarVentasQuery, ListadoPaginadoDto<VentaDto>> _buscarHandler;
    private readonly IQueryHandler<ObtenerVentaQuery, VentaDto> _obtenerHandler;

    public VentasController(
        ICommandHandler<RegistrarVentaCommand, VentaDto> registrarHandler,
        ICommandHandler<AnularVentaCommand, VentaDto> anularHandler,
        ICommandHandler<RegistrarDevolucionCommand, VentaDto> registrarDevolucionHandler,
        IQueryHandler<BuscarVentasQuery, ListadoPaginadoDto<VentaDto>> buscarHandler,
        IQueryHandler<ObtenerVentaQuery, VentaDto> obtenerHandler)
    {
        _registrarHandler = registrarHandler;
        _anularHandler = anularHandler;
        _registrarDevolucionHandler = registrarDevolucionHandler;
        _buscarHandler = buscarHandler;
        _obtenerHandler = obtenerHandler;
    }

    private Guid UsuarioActualId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    [Authorize(Policy = "Ventas.Consultar")]
    public async Task<ActionResult<ListadoPaginadoDto<VentaDto>>> Buscar(
        [FromQuery] string? estado, [FromQuery] Guid? cliente, [FromQuery] int pagina = 1, [FromQuery] int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        var estadoEnum = string.IsNullOrWhiteSpace(estado) ? (EstadoVenta?)null : Enum.Parse<EstadoVenta>(estado);
        var resultado = await _buscarHandler.ManejarAsync(new BuscarVentasQuery(estadoEnum, cliente, pagina, tamanoPagina), cancellationToken);
        return Ok(resultado);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Ventas.Consultar")]
    public async Task<ActionResult<VentaDto>> Obtener(Guid id, CancellationToken cancellationToken)
    {
        var venta = await _obtenerHandler.ManejarAsync(new ObtenerVentaQuery(id), cancellationToken);
        return Ok(venta);
    }

    /// <summary>UC-14/RN-003 — valida stock antes de confirmar; RN-031: saldo pendiente exige autorización.</summary>
    [HttpPost]
    [Authorize(Policy = "Ventas.Crear")]
    public async Task<ActionResult<VentaDto>> Registrar(RegistrarVentaRequest request, CancellationToken cancellationToken)
    {
        var tipoComprobante = Enum.Parse<TipoComprobante>(request.TipoComprobante);
        var comando = new RegistrarVentaCommand(
            request.ClienteId, tipoComprobante, request.Detalles, request.Pagos, request.UsuarioAutorizoSaldoId, UsuarioActualId);
        var venta = await _registrarHandler.ManejarAsync(comando, cancellationToken);
        return CreatedAtAction(nameof(Obtener), new { id = venta.Id }, venta);
    }

    /// <summary>UC-16/RN-010 — motivo obligatorio, reversión automática de inventario.</summary>
    [HttpPost("{id:guid}/anular")]
    [Authorize(Policy = "Ventas.Anular")]
    public async Task<ActionResult<VentaDto>> Anular(Guid id, AnularVentaRequest request, CancellationToken cancellationToken)
    {
        var comando = new AnularVentaCommand(id, request.Motivo, UsuarioActualId);
        var venta = await _anularHandler.ManejarAsync(comando, cancellationToken);
        return Ok(venta);
    }

    /// <summary>UC-17/RN-032 — trazable a la venta original.</summary>
    [HttpPost("{id:guid}/devoluciones")]
    [Authorize(Policy = "Ventas.Crear")]
    public async Task<ActionResult<VentaDto>> RegistrarDevolucion(Guid id, RegistrarDevolucionRequest request, CancellationToken cancellationToken)
    {
        var comando = new RegistrarDevolucionCommand(id, request.Detalles, request.Motivo, UsuarioActualId);
        var venta = await _registrarDevolucionHandler.ManejarAsync(comando, cancellationToken);
        return Ok(venta);
    }
}

public record RegistrarVentaRequest(
    Guid? ClienteId,
    string TipoComprobante,
    IReadOnlyCollection<DetalleVentaInput> Detalles,
    IReadOnlyCollection<PagoVentaInput> Pagos,
    Guid? UsuarioAutorizoSaldoId);

public record AnularVentaRequest(string Motivo);

public record RegistrarDevolucionRequest(IReadOnlyCollection<DetalleDevolucionInput> Detalles, string? Motivo);
