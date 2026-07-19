using System.Security.Claims;
using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Inventario.Commands.AjustarInventario;
using ISARMIN.Application.Modulos.Inventario.Commands.CambiarEstadoProducto;
using ISARMIN.Application.Modulos.Inventario.Commands.EditarProducto;
using ISARMIN.Application.Modulos.Inventario.Commands.RegistrarProducto;
using ISARMIN.Application.Modulos.Inventario.DTOs;
using ISARMIN.Application.Modulos.Inventario.Queries.BuscarProductos;
using ISARMIN.Application.Modulos.Inventario.Queries.ConsultarKardex;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ISARMIN.API.Controllers;

/// <summary>UC-10 — Gestionar Producto (RF-023 a RF-026); UC-12 — Ajustar Inventario (RF-030).</summary>
[ApiController]
[Route("api/v1/productos")]
public class ProductosController : ControllerBase
{
    private readonly IQueryHandler<BuscarProductosQuery, ListadoPaginadoDto<ProductoDto>> _buscarHandler;
    private readonly ICommandHandler<RegistrarProductoCommand, ProductoDto> _registrarHandler;
    private readonly ICommandHandler<EditarProductoCommand, ProductoDto> _editarHandler;
    private readonly ICommandHandler<CambiarEstadoProductoCommand, Unit> _cambiarEstadoHandler;
    private readonly ICommandHandler<AjustarInventarioCommand, ProductoDto> _ajustarInventarioHandler;
    private readonly IQueryHandler<ConsultarKardexQuery, IReadOnlyCollection<MovimientoInventarioDto>> _consultarKardexHandler;

    public ProductosController(
        IQueryHandler<BuscarProductosQuery, ListadoPaginadoDto<ProductoDto>> buscarHandler,
        ICommandHandler<RegistrarProductoCommand, ProductoDto> registrarHandler,
        ICommandHandler<EditarProductoCommand, ProductoDto> editarHandler,
        ICommandHandler<CambiarEstadoProductoCommand, Unit> cambiarEstadoHandler,
        ICommandHandler<AjustarInventarioCommand, ProductoDto> ajustarInventarioHandler,
        IQueryHandler<ConsultarKardexQuery, IReadOnlyCollection<MovimientoInventarioDto>> consultarKardexHandler)
    {
        _buscarHandler = buscarHandler;
        _registrarHandler = registrarHandler;
        _editarHandler = editarHandler;
        _cambiarEstadoHandler = cambiarEstadoHandler;
        _ajustarInventarioHandler = ajustarInventarioHandler;
        _consultarKardexHandler = consultarKardexHandler;
    }

    /// <summary>RF-026 — consulta de stock disponible incluida en el listado (stock_actual).</summary>
    [HttpGet]
    [Authorize(Policy = "Inventario.Consultar")]
    public async Task<ActionResult<ListadoPaginadoDto<ProductoDto>>> Buscar(
        [FromQuery] string? busqueda, [FromQuery] Guid? categoria, [FromQuery] int pagina = 1, [FromQuery] int tamanoPagina = 20, CancellationToken cancellationToken = default)
    {
        var resultado = await _buscarHandler.ManejarAsync(new BuscarProductosQuery(busqueda, categoria, pagina, tamanoPagina), cancellationToken);
        return Ok(resultado);
    }

    [HttpPost]
    [Authorize(Policy = "Inventario.Crear")]
    public async Task<ActionResult<ProductoDto>> Registrar(RegistrarProductoRequest request, CancellationToken cancellationToken)
    {
        var comando = new RegistrarProductoCommand(
            request.CodigoInterno, request.Nombre, request.CategoriaId, request.UnidadMedidaId,
            request.CostoReferencia, request.PrecioVenta, request.StockInicial,
            request.Marca, request.CodigoBarras, request.StockMinimo);
        var producto = await _registrarHandler.ManejarAsync(comando, cancellationToken);
        return CreatedAtAction(nameof(Buscar), new { }, producto);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Inventario.Editar")]
    public async Task<ActionResult<ProductoDto>> Editar(Guid id, EditarProductoRequest request, CancellationToken cancellationToken)
    {
        var comando = new EditarProductoCommand(
            id, request.CodigoInterno, request.Nombre, request.CategoriaId, request.UnidadMedidaId,
            request.CostoReferencia, request.PrecioVenta, request.Marca, request.CodigoBarras, request.StockMinimo);
        var producto = await _editarHandler.ManejarAsync(comando, cancellationToken);
        return Ok(producto);
    }

    /// <summary>RN-023 — baja lógica, nunca eliminación física.</summary>
    [HttpPatch("{id:guid}/estado")]
    [Authorize(Policy = "Inventario.Eliminar")]
    public async Task<IActionResult> CambiarEstado(Guid id, CambiarEstadoProductoRequest request, CancellationToken cancellationToken)
    {
        await _cambiarEstadoHandler.ManejarAsync(new CambiarEstadoProductoCommand(id, request.Activo), cancellationToken);
        return NoContent();
    }

    /// <summary>RF-073 — Kardex del producto (historial de movimientos de inventario).</summary>
    [HttpGet("{id:guid}/movimientos")]
    [Authorize(Policy = "Inventario.Consultar")]
    public async Task<ActionResult<IReadOnlyCollection<MovimientoInventarioDto>>> ConsultarKardex(
        Guid id, [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, CancellationToken cancellationToken)
    {
        var movimientos = await _consultarKardexHandler.ManejarAsync(new ConsultarKardexQuery(id, desde, hasta), cancellationToken);
        return Ok(movimientos);
    }

    /// <summary>UC-12/RF-030 — ajuste manual de inventario, exclusivo del Administrador (RN-008).</summary>
    [HttpPost("{id:guid}/ajustes")]
    [Authorize(Policy = "Inventario.Ajustar")]
    public async Task<ActionResult<ProductoDto>> Ajustar(Guid id, AjustarInventarioRequest request, CancellationToken cancellationToken)
    {
        var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var comando = new AjustarInventarioCommand(id, request.CantidadAjuste, request.Motivo, usuarioId);
        var producto = await _ajustarInventarioHandler.ManejarAsync(comando, cancellationToken);
        return Ok(producto);
    }
}

public record RegistrarProductoRequest(
    string CodigoInterno,
    string Nombre,
    Guid CategoriaId,
    Guid UnidadMedidaId,
    decimal CostoReferencia,
    decimal PrecioVenta,
    decimal StockInicial,
    string? Marca,
    string? CodigoBarras,
    decimal? StockMinimo);

public record EditarProductoRequest(
    string CodigoInterno,
    string Nombre,
    Guid CategoriaId,
    Guid UnidadMedidaId,
    decimal CostoReferencia,
    decimal PrecioVenta,
    string? Marca,
    string? CodigoBarras,
    decimal? StockMinimo);

public record CambiarEstadoProductoRequest(bool Activo);

public record AjustarInventarioRequest(decimal CantidadAjuste, string Motivo);
