using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Clientes.Commands.CambiarEstadoCliente;
using ISARMIN.Application.Modulos.Clientes.Commands.EditarCliente;
using ISARMIN.Application.Modulos.Clientes.Commands.RegistrarCliente;
using ISARMIN.Application.Modulos.Clientes.DTOs;
using ISARMIN.Application.Modulos.Clientes.Queries.BuscarClientes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ISARMIN.API.Controllers;

/// <summary>UC-05 — Gestionar Cliente.</summary>
[ApiController]
[Route("api/v1/clientes")]
public class ClientesController : ControllerBase
{
    private readonly IQueryHandler<BuscarClientesQuery, ListadoPaginadoDto<ClienteDto>> _buscarHandler;
    private readonly ICommandHandler<RegistrarClienteCommand, ClienteDto> _registrarHandler;
    private readonly ICommandHandler<EditarClienteCommand, ClienteDto> _editarHandler;
    private readonly ICommandHandler<CambiarEstadoClienteCommand, Unit> _cambiarEstadoHandler;

    public ClientesController(
        IQueryHandler<BuscarClientesQuery, ListadoPaginadoDto<ClienteDto>> buscarHandler,
        ICommandHandler<RegistrarClienteCommand, ClienteDto> registrarHandler,
        ICommandHandler<EditarClienteCommand, ClienteDto> editarHandler,
        ICommandHandler<CambiarEstadoClienteCommand, Unit> cambiarEstadoHandler)
    {
        _buscarHandler = buscarHandler;
        _registrarHandler = registrarHandler;
        _editarHandler = editarHandler;
        _cambiarEstadoHandler = cambiarEstadoHandler;
    }

    [HttpGet]
    [Authorize(Policy = "Clientes.Consultar")]
    public async Task<ActionResult<ListadoPaginadoDto<ClienteDto>>> Buscar(
        [FromQuery] string? busqueda, [FromQuery] int pagina = 1, [FromQuery] int tamanoPagina = 20, CancellationToken cancellationToken = default)
    {
        var resultado = await _buscarHandler.ManejarAsync(new BuscarClientesQuery(busqueda, pagina, tamanoPagina), cancellationToken);
        return Ok(resultado);
    }

    [HttpPost]
    [Authorize(Policy = "Clientes.Crear")]
    public async Task<ActionResult<ClienteDto>> Registrar(RegistrarClienteCommand comando, CancellationToken cancellationToken)
    {
        var cliente = await _registrarHandler.ManejarAsync(comando, cancellationToken);
        return CreatedAtAction(nameof(Buscar), new { }, cliente);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Clientes.Editar")]
    public async Task<ActionResult<ClienteDto>> Editar(Guid id, EditarClienteRequest request, CancellationToken cancellationToken)
    {
        var comando = new EditarClienteCommand(id, request.NombreRazonSocial, request.Telefono, request.Direccion, request.TipoDocumento, request.NumeroDocumento);
        var cliente = await _editarHandler.ManejarAsync(comando, cancellationToken);
        return Ok(cliente);
    }

    /// <summary>RF-014/RN-023 — baja lógica, nunca eliminación física.</summary>
    [HttpPatch("{id:guid}/estado")]
    [Authorize(Policy = "Clientes.Eliminar")]
    public async Task<IActionResult> CambiarEstado(Guid id, CambiarEstadoClienteRequest request, CancellationToken cancellationToken)
    {
        await _cambiarEstadoHandler.ManejarAsync(new CambiarEstadoClienteCommand(id, request.Activo), cancellationToken);
        return NoContent();
    }
}

public record EditarClienteRequest(
    string NombreRazonSocial,
    string Telefono,
    string? Direccion,
    string? TipoDocumento,
    string? NumeroDocumento);

public record CambiarEstadoClienteRequest(bool Activo);
