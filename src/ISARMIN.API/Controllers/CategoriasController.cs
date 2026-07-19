using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Inventario.Commands.CrearCategoria;
using ISARMIN.Application.Modulos.Inventario.Commands.EditarCategoria;
using ISARMIN.Application.Modulos.Inventario.DTOs;
using ISARMIN.Application.Modulos.Inventario.Queries.ListarCategorias;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ISARMIN.API.Controllers;

/// <summary>CAT-002 — catálogo configurable de categorías de producto (RF-025, UC-37).</summary>
[ApiController]
[Route("api/v1/categorias")]
public class CategoriasController : ControllerBase
{
    private readonly IQueryHandler<ListarCategoriasQuery, IReadOnlyCollection<CategoriaDto>> _listarHandler;
    private readonly ICommandHandler<CrearCategoriaCommand, CategoriaDto> _crearHandler;
    private readonly ICommandHandler<EditarCategoriaCommand, CategoriaDto> _editarHandler;

    public CategoriasController(
        IQueryHandler<ListarCategoriasQuery, IReadOnlyCollection<CategoriaDto>> listarHandler,
        ICommandHandler<CrearCategoriaCommand, CategoriaDto> crearHandler,
        ICommandHandler<EditarCategoriaCommand, CategoriaDto> editarHandler)
    {
        _listarHandler = listarHandler;
        _crearHandler = crearHandler;
        _editarHandler = editarHandler;
    }

    [HttpGet]
    [Authorize(Policy = "Inventario.Consultar")]
    public async Task<ActionResult<IReadOnlyCollection<CategoriaDto>>> Listar(CancellationToken cancellationToken)
    {
        var categorias = await _listarHandler.ManejarAsync(new ListarCategoriasQuery(), cancellationToken);
        return Ok(categorias);
    }

    [HttpPost]
    [Authorize(Policy = "Configuracion.Editar")]
    public async Task<ActionResult<CategoriaDto>> Crear(CrearCategoriaCommand comando, CancellationToken cancellationToken)
    {
        var categoria = await _crearHandler.ManejarAsync(comando, cancellationToken);
        return CreatedAtAction(nameof(Listar), new { }, categoria);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Configuracion.Editar")]
    public async Task<ActionResult<CategoriaDto>> Editar(Guid id, EditarCategoriaRequest request, CancellationToken cancellationToken)
    {
        var categoria = await _editarHandler.ManejarAsync(
            new EditarCategoriaCommand(id, request.Nombre, request.CategoriaPadreId), cancellationToken);
        return Ok(categoria);
    }
}

public record EditarCategoriaRequest(string Nombre, Guid? CategoriaPadreId);
