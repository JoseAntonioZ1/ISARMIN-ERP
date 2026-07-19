using System.Security.Claims;
using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Compras.Commands.RegistrarCompra;
using ISARMIN.Application.Modulos.Compras.DTOs;
using ISARMIN.Application.Modulos.Compras.Queries.BuscarCompras;
using ISARMIN.Application.Modulos.Compras.Queries.ObtenerCompra;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ISARMIN.API.Controllers;

/// <summary>UC-13 — Registrar Compra (RF-033 a RF-036).</summary>
[ApiController]
[Route("api/v1/compras")]
public class ComprasController : ControllerBase
{
    private readonly IQueryHandler<BuscarComprasQuery, ListadoPaginadoDto<CompraDto>> _buscarHandler;
    private readonly IQueryHandler<ObtenerCompraQuery, CompraDto> _obtenerHandler;
    private readonly ICommandHandler<RegistrarCompraCommand, CompraDto> _registrarHandler;

    public ComprasController(
        IQueryHandler<BuscarComprasQuery, ListadoPaginadoDto<CompraDto>> buscarHandler,
        IQueryHandler<ObtenerCompraQuery, CompraDto> obtenerHandler,
        ICommandHandler<RegistrarCompraCommand, CompraDto> registrarHandler)
    {
        _buscarHandler = buscarHandler;
        _obtenerHandler = obtenerHandler;
        _registrarHandler = registrarHandler;
    }

    /// <summary>RF-036 — historial de compras por proveedor o por producto.</summary>
    [HttpGet]
    [Authorize(Policy = "Compras.Consultar")]
    public async Task<ActionResult<ListadoPaginadoDto<CompraDto>>> Buscar(
        [FromQuery] Guid? proveedor, [FromQuery] Guid? producto, [FromQuery] int pagina = 1, [FromQuery] int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        var resultado = await _buscarHandler.ManejarAsync(new BuscarComprasQuery(proveedor, producto, pagina, tamanoPagina), cancellationToken);
        return Ok(resultado);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Compras.Consultar")]
    public async Task<ActionResult<CompraDto>> Obtener(Guid id, CancellationToken cancellationToken)
    {
        var compra = await _obtenerHandler.ManejarAsync(new ObtenerCompraQuery(id), cancellationToken);
        return Ok(compra);
    }

    /// <summary>UC-13/RN-024 — compra directa ya realizada, sin orden de compra ni aprobación previa.</summary>
    [HttpPost]
    [Authorize(Policy = "Compras.Crear")]
    public async Task<ActionResult<CompraDto>> Registrar(RegistrarCompraRequest request, CancellationToken cancellationToken)
    {
        var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var comando = new RegistrarCompraCommand(
            request.ProveedorId, request.Fecha, request.DocumentoCompraTipo, request.DocumentoCompraNumero, usuarioId, request.Detalles);
        var compra = await _registrarHandler.ManejarAsync(comando, cancellationToken);
        return CreatedAtAction(nameof(Obtener), new { id = compra.Id }, compra);
    }
}

public record RegistrarCompraRequest(
    Guid ProveedorId,
    DateOnly Fecha,
    string DocumentoCompraTipo,
    string DocumentoCompraNumero,
    IReadOnlyCollection<DetalleCompraInput> Detalles);
