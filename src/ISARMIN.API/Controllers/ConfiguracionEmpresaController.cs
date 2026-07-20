using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Configuracion.Commands.ActualizarConfiguracionEmpresa;
using ISARMIN.Application.Modulos.Configuracion.DTOs;
using ISARMIN.Application.Modulos.Configuracion.Queries.ObtenerConfiguracionEmpresa;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ISARMIN.API.Controllers;

/// <summary>UC-37/RF-080 — datos generales de la empresa (fila única sembrada en la migración).</summary>
[ApiController]
[Route("api/v1/configuracion/empresa")]
public class ConfiguracionEmpresaController : ControllerBase
{
    private readonly IQueryHandler<ObtenerConfiguracionEmpresaQuery, ConfiguracionEmpresaDto> _obtenerHandler;
    private readonly ICommandHandler<ActualizarConfiguracionEmpresaCommand, ConfiguracionEmpresaDto> _actualizarHandler;

    public ConfiguracionEmpresaController(
        IQueryHandler<ObtenerConfiguracionEmpresaQuery, ConfiguracionEmpresaDto> obtenerHandler,
        ICommandHandler<ActualizarConfiguracionEmpresaCommand, ConfiguracionEmpresaDto> actualizarHandler)
    {
        _obtenerHandler = obtenerHandler;
        _actualizarHandler = actualizarHandler;
    }

    [HttpGet]
    [Authorize(Policy = "Configuracion.Consultar")]
    public async Task<ActionResult<ConfiguracionEmpresaDto>> Obtener(CancellationToken cancellationToken)
    {
        var configuracion = await _obtenerHandler.ManejarAsync(new ObtenerConfiguracionEmpresaQuery(), cancellationToken);
        return Ok(configuracion);
    }

    [HttpPut]
    [Authorize(Policy = "Configuracion.Editar")]
    public async Task<ActionResult<ConfiguracionEmpresaDto>> Actualizar(ActualizarConfiguracionEmpresaCommand comando, CancellationToken cancellationToken)
    {
        var configuracion = await _actualizarHandler.ManejarAsync(comando, cancellationToken);
        return Ok(configuracion);
    }
}
