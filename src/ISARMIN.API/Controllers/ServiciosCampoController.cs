using System.Security.Claims;
using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.ServiciosCampo.Commands.CerrarServicioCampo;
using ISARMIN.Application.Modulos.ServiciosCampo.Commands.CobrarServicioCampo;
using ISARMIN.Application.Modulos.ServiciosCampo.Commands.CotizarServicioCampo;
using ISARMIN.Application.Modulos.ServiciosCampo.Commands.SolicitarServicioCampo;
using ISARMIN.Application.Modulos.ServiciosCampo.DTOs;
using ISARMIN.Application.Modulos.ServiciosCampo.Queries.BuscarServiciosCampo;
using ISARMIN.Application.Modulos.ServiciosCampo.Queries.ObtenerServicioCampo;
using ISARMIN.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ISARMIN.API.Controllers;

/// <summary>UC-30 a UC-33 — Servicios de Campo (RF-064 a RF-070).</summary>
[ApiController]
[Route("api/v1/servicios-campo")]
public class ServiciosCampoController : ControllerBase
{
    private readonly ICommandHandler<SolicitarServicioCampoCommand, ServicioCampoDto> _solicitarHandler;
    private readonly ICommandHandler<CotizarServicioCampoCommand, ServicioCampoDto> _cotizarHandler;
    private readonly ICommandHandler<CerrarServicioCampoCommand, ServicioCampoDto> _cerrarHandler;
    private readonly ICommandHandler<CobrarServicioCampoCommand, ServicioCampoDto> _cobrarHandler;
    private readonly IQueryHandler<BuscarServiciosCampoQuery, ListadoPaginadoDto<ServicioCampoDto>> _buscarHandler;
    private readonly IQueryHandler<ObtenerServicioCampoQuery, ServicioCampoDto> _obtenerHandler;

    public ServiciosCampoController(
        ICommandHandler<SolicitarServicioCampoCommand, ServicioCampoDto> solicitarHandler,
        ICommandHandler<CotizarServicioCampoCommand, ServicioCampoDto> cotizarHandler,
        ICommandHandler<CerrarServicioCampoCommand, ServicioCampoDto> cerrarHandler,
        ICommandHandler<CobrarServicioCampoCommand, ServicioCampoDto> cobrarHandler,
        IQueryHandler<BuscarServiciosCampoQuery, ListadoPaginadoDto<ServicioCampoDto>> buscarHandler,
        IQueryHandler<ObtenerServicioCampoQuery, ServicioCampoDto> obtenerHandler)
    {
        _solicitarHandler = solicitarHandler;
        _cotizarHandler = cotizarHandler;
        _cerrarHandler = cerrarHandler;
        _cobrarHandler = cobrarHandler;
        _buscarHandler = buscarHandler;
        _obtenerHandler = obtenerHandler;
    }

    private Guid UsuarioActualId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    [Authorize(Policy = "ServiciosCampo.Consultar")]
    public async Task<ActionResult<ListadoPaginadoDto<ServicioCampoDto>>> Buscar(
        [FromQuery] string? estado, [FromQuery] Guid? cliente, [FromQuery] int pagina = 1, [FromQuery] int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        var estadoEnum = string.IsNullOrWhiteSpace(estado) ? (EstadoServicioCampo?)null : Enum.Parse<EstadoServicioCampo>(estado);
        var resultado = await _buscarHandler.ManejarAsync(new BuscarServiciosCampoQuery(estadoEnum, cliente, pagina, tamanoPagina), cancellationToken);
        return Ok(resultado);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "ServiciosCampo.Consultar")]
    public async Task<ActionResult<ServicioCampoDto>> Obtener(Guid id, CancellationToken cancellationToken)
    {
        var servicio = await _obtenerHandler.ManejarAsync(new ObtenerServicioCampoQuery(id), cancellationToken);
        return Ok(servicio);
    }

    /// <summary>UC-30/RF-064, RF-065 — BQ-037: la asignación de técnico es opcional/informal.</summary>
    [HttpPost]
    [Authorize(Policy = "ServiciosCampo.Crear")]
    public async Task<ActionResult<ServicioCampoDto>> Solicitar(SolicitarServicioCampoRequest request, CancellationToken cancellationToken)
    {
        var comando = new SolicitarServicioCampoCommand(request.ClienteId, request.DescripcionTrabajo, request.TecnicoAsignadoId);
        var servicio = await _solicitarHandler.ManejarAsync(comando, cancellationToken);
        return CreatedAtAction(nameof(Obtener), new { id = servicio.Id }, servicio);
    }

    /// <summary>UC-31/RF-068 — captura de dato, sin transición de estado.</summary>
    [HttpPost("{id:guid}/cotizacion")]
    [Authorize(Policy = "ServiciosCampo.Cotizar")]
    public async Task<ActionResult<ServicioCampoDto>> Cotizar(Guid id, CotizarServicioCampoRequest request, CancellationToken cancellationToken)
    {
        var comando = new CotizarServicioCampoCommand(id, request.MontoEstimado);
        var servicio = await _cotizarHandler.ManejarAsync(comando, cancellationToken);
        return Ok(servicio);
    }

    /// <summary>UC-32/RF-067, RF-069 — RN-020: los materiales consumidos se descuentan del inventario compartido.</summary>
    [HttpPost("{id:guid}/cierre")]
    [Authorize(Policy = "ServiciosCampo.Cerrar")]
    public async Task<ActionResult<ServicioCampoDto>> Cerrar(Guid id, CerrarServicioCampoRequest request, CancellationToken cancellationToken)
    {
        var comando = new CerrarServicioCampoCommand(id, request.Consumos, request.EstadoFinal, request.Observaciones, UsuarioActualId);
        var servicio = await _cerrarHandler.ManejarAsync(comando, cancellationToken);
        return Ok(servicio);
    }

    /// <summary>UC-33/RF-070 — RN-031: el saldo pendiente exige un usuario Administrador/Propietario autorizante.</summary>
    [HttpPost("{id:guid}/cobro")]
    [Authorize(Policy = "ServiciosCampo.Cobrar")]
    public async Task<ActionResult<ServicioCampoDto>> Cobrar(Guid id, CobrarServicioCampoRequest request, CancellationToken cancellationToken)
    {
        var comando = new CobrarServicioCampoCommand(id, request.MedioPagoId, request.MontoPagado, request.SaldoPendiente, request.UsuarioAutorizoSaldoId);
        var servicio = await _cobrarHandler.ManejarAsync(comando, cancellationToken);
        return Ok(servicio);
    }
}

public record SolicitarServicioCampoRequest(Guid ClienteId, string DescripcionTrabajo, Guid? TecnicoAsignadoId);

public record CotizarServicioCampoRequest(decimal MontoEstimado);

public record CerrarServicioCampoRequest(IReadOnlyCollection<DetalleConsumoCampoInput> Consumos, string EstadoFinal, string? Observaciones);

public record CobrarServicioCampoRequest(Guid MedioPagoId, decimal MontoPagado, decimal? SaldoPendiente, Guid? UsuarioAutorizoSaldoId);
