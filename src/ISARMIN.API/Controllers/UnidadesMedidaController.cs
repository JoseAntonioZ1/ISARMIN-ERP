using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Inventario.Commands.CrearUnidadMedida;
using ISARMIN.Application.Modulos.Inventario.Commands.EditarUnidadMedida;
using ISARMIN.Application.Modulos.Inventario.DTOs;
using ISARMIN.Application.Modulos.Inventario.Queries.ListarUnidadesMedida;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ISARMIN.API.Controllers;

/// <summary>CAT-014 — catálogo configurable de unidades de medida (RN-040), resuelve BQ-008.</summary>
[ApiController]
[Route("api/v1/unidades-medida")]
public class UnidadesMedidaController : ControllerBase
{
    private readonly IQueryHandler<ListarUnidadesMedidaQuery, IReadOnlyCollection<UnidadMedidaDto>> _listarHandler;
    private readonly ICommandHandler<CrearUnidadMedidaCommand, UnidadMedidaDto> _crearHandler;
    private readonly ICommandHandler<EditarUnidadMedidaCommand, UnidadMedidaDto> _editarHandler;

    public UnidadesMedidaController(
        IQueryHandler<ListarUnidadesMedidaQuery, IReadOnlyCollection<UnidadMedidaDto>> listarHandler,
        ICommandHandler<CrearUnidadMedidaCommand, UnidadMedidaDto> crearHandler,
        ICommandHandler<EditarUnidadMedidaCommand, UnidadMedidaDto> editarHandler)
    {
        _listarHandler = listarHandler;
        _crearHandler = crearHandler;
        _editarHandler = editarHandler;
    }

    [HttpGet]
    [Authorize(Policy = "Inventario.Consultar")]
    public async Task<ActionResult<IReadOnlyCollection<UnidadMedidaDto>>> Listar(CancellationToken cancellationToken)
    {
        var unidadesMedida = await _listarHandler.ManejarAsync(new ListarUnidadesMedidaQuery(), cancellationToken);
        return Ok(unidadesMedida);
    }

    [HttpPost]
    [Authorize(Policy = "Configuracion.Editar")]
    public async Task<ActionResult<UnidadMedidaDto>> Crear(CrearUnidadMedidaCommand comando, CancellationToken cancellationToken)
    {
        var unidadMedida = await _crearHandler.ManejarAsync(comando, cancellationToken);
        return CreatedAtAction(nameof(Listar), new { }, unidadMedida);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Configuracion.Editar")]
    public async Task<ActionResult<UnidadMedidaDto>> Editar(Guid id, EditarUnidadMedidaRequest request, CancellationToken cancellationToken)
    {
        var unidadMedida = await _editarHandler.ManejarAsync(new EditarUnidadMedidaCommand(id, request.Nombre), cancellationToken);
        return Ok(unidadMedida);
    }
}

public record EditarUnidadMedidaRequest(string Nombre);
