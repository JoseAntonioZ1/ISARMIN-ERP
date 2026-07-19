using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Proveedores.Commands.CambiarEstadoProveedor;
using ISARMIN.Application.Modulos.Proveedores.Commands.EditarProveedor;
using ISARMIN.Application.Modulos.Proveedores.Commands.RegistrarProveedor;
using ISARMIN.Application.Modulos.Proveedores.DTOs;
using ISARMIN.Application.Modulos.Proveedores.Queries.BuscarProveedores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ISARMIN.API.Controllers;

/// <summary>UC-09 — Gestionar Proveedor.</summary>
[ApiController]
[Route("api/v1/proveedores")]
public class ProveedoresController : ControllerBase
{
    private readonly IQueryHandler<BuscarProveedoresQuery, ListadoPaginadoDto<ProveedorDto>> _buscarHandler;
    private readonly ICommandHandler<RegistrarProveedorCommand, ProveedorDto> _registrarHandler;
    private readonly ICommandHandler<EditarProveedorCommand, ProveedorDto> _editarHandler;
    private readonly ICommandHandler<CambiarEstadoProveedorCommand, Unit> _cambiarEstadoHandler;

    public ProveedoresController(
        IQueryHandler<BuscarProveedoresQuery, ListadoPaginadoDto<ProveedorDto>> buscarHandler,
        ICommandHandler<RegistrarProveedorCommand, ProveedorDto> registrarHandler,
        ICommandHandler<EditarProveedorCommand, ProveedorDto> editarHandler,
        ICommandHandler<CambiarEstadoProveedorCommand, Unit> cambiarEstadoHandler)
    {
        _buscarHandler = buscarHandler;
        _registrarHandler = registrarHandler;
        _editarHandler = editarHandler;
        _cambiarEstadoHandler = cambiarEstadoHandler;
    }

    [HttpGet]
    [Authorize(Policy = "Proveedores.Consultar")]
    public async Task<ActionResult<ListadoPaginadoDto<ProveedorDto>>> Buscar(
        [FromQuery] string? busqueda, [FromQuery] int pagina = 1, [FromQuery] int tamanoPagina = 20, CancellationToken cancellationToken = default)
    {
        var resultado = await _buscarHandler.ManejarAsync(new BuscarProveedoresQuery(busqueda, pagina, tamanoPagina), cancellationToken);
        return Ok(resultado);
    }

    [HttpPost]
    [Authorize(Policy = "Proveedores.Crear")]
    public async Task<ActionResult<ProveedorDto>> Registrar(RegistrarProveedorCommand comando, CancellationToken cancellationToken)
    {
        var proveedor = await _registrarHandler.ManejarAsync(comando, cancellationToken);
        return CreatedAtAction(nameof(Buscar), new { }, proveedor);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Proveedores.Editar")]
    public async Task<ActionResult<ProveedorDto>> Editar(Guid id, EditarProveedorRequest request, CancellationToken cancellationToken)
    {
        var comando = new EditarProveedorCommand(id, request.NombreRazonSocial, request.Documento, request.Telefono, request.Direccion);
        var proveedor = await _editarHandler.ManejarAsync(comando, cancellationToken);
        return Ok(proveedor);
    }

    /// <summary>RF-020/RN-023 — baja lógica, nunca eliminación física.</summary>
    [HttpPatch("{id:guid}/estado")]
    [Authorize(Policy = "Proveedores.Eliminar")]
    public async Task<IActionResult> CambiarEstado(Guid id, CambiarEstadoProveedorRequest request, CancellationToken cancellationToken)
    {
        await _cambiarEstadoHandler.ManejarAsync(new CambiarEstadoProveedorCommand(id, request.Activo), cancellationToken);
        return NoContent();
    }
}

public record EditarProveedorRequest(string NombreRazonSocial, string? Documento, string? Telefono, string? Direccion);

public record CambiarEstadoProveedorRequest(bool Activo);
