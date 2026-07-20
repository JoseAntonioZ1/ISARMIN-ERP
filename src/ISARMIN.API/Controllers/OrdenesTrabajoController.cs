using System.Security.Claims;
using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Taller.Commands.EntregarEquipo;
using ISARMIN.Application.Modulos.Taller.Commands.GenerarCotizacionReparacion;
using ISARMIN.Application.Modulos.Taller.Commands.RegistrarDecisionCliente;
using ISARMIN.Application.Modulos.Taller.Commands.RegistrarDiagnostico;
using ISARMIN.Application.Modulos.Taller.Commands.RegistrarGarantia;
using ISARMIN.Application.Modulos.Taller.Commands.RegistrarRecepcion;
using ISARMIN.Application.Modulos.Taller.Commands.RegistrarReparacion;
using ISARMIN.Application.Modulos.Taller.DTOs;
using ISARMIN.Application.Modulos.Taller.Queries.BuscarOrdenesTrabajo;
using ISARMIN.Application.Modulos.Taller.Queries.ObtenerOrdenTrabajo;
using ISARMIN.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ISARMIN.API.Controllers;

/// <summary>UC-22 a UC-28 — Taller (RF-051 a RF-063).</summary>
[ApiController]
[Route("api/v1/ordenes-trabajo")]
public class OrdenesTrabajoController : ControllerBase
{
    private readonly ICommandHandler<RegistrarRecepcionCommand, OrdenTrabajoDto> _registrarRecepcionHandler;
    private readonly ICommandHandler<RegistrarDiagnosticoCommand, OrdenTrabajoDto> _registrarDiagnosticoHandler;
    private readonly ICommandHandler<GenerarCotizacionReparacionCommand, OrdenTrabajoDto> _generarCotizacionHandler;
    private readonly ICommandHandler<RegistrarDecisionClienteCommand, OrdenTrabajoDto> _registrarDecisionHandler;
    private readonly ICommandHandler<RegistrarReparacionCommand, OrdenTrabajoDto> _registrarReparacionHandler;
    private readonly ICommandHandler<EntregarEquipoCommand, OrdenTrabajoDto> _entregarEquipoHandler;
    private readonly ICommandHandler<RegistrarGarantiaCommand, GarantiaDto> _registrarGarantiaHandler;
    private readonly IQueryHandler<BuscarOrdenesTrabajoQuery, ListadoPaginadoDto<OrdenTrabajoDto>> _buscarHandler;
    private readonly IQueryHandler<ObtenerOrdenTrabajoQuery, OrdenTrabajoDetalleDto> _obtenerHandler;

    public OrdenesTrabajoController(
        ICommandHandler<RegistrarRecepcionCommand, OrdenTrabajoDto> registrarRecepcionHandler,
        ICommandHandler<RegistrarDiagnosticoCommand, OrdenTrabajoDto> registrarDiagnosticoHandler,
        ICommandHandler<GenerarCotizacionReparacionCommand, OrdenTrabajoDto> generarCotizacionHandler,
        ICommandHandler<RegistrarDecisionClienteCommand, OrdenTrabajoDto> registrarDecisionHandler,
        ICommandHandler<RegistrarReparacionCommand, OrdenTrabajoDto> registrarReparacionHandler,
        ICommandHandler<EntregarEquipoCommand, OrdenTrabajoDto> entregarEquipoHandler,
        ICommandHandler<RegistrarGarantiaCommand, GarantiaDto> registrarGarantiaHandler,
        IQueryHandler<BuscarOrdenesTrabajoQuery, ListadoPaginadoDto<OrdenTrabajoDto>> buscarHandler,
        IQueryHandler<ObtenerOrdenTrabajoQuery, OrdenTrabajoDetalleDto> obtenerHandler)
    {
        _registrarRecepcionHandler = registrarRecepcionHandler;
        _registrarDiagnosticoHandler = registrarDiagnosticoHandler;
        _generarCotizacionHandler = generarCotizacionHandler;
        _registrarDecisionHandler = registrarDecisionHandler;
        _registrarReparacionHandler = registrarReparacionHandler;
        _entregarEquipoHandler = entregarEquipoHandler;
        _registrarGarantiaHandler = registrarGarantiaHandler;
        _buscarHandler = buscarHandler;
        _obtenerHandler = obtenerHandler;
    }

    private Guid UsuarioActualId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>RF-074 — listado con filtros; también satisface UC-28 (historial por cliente).</summary>
    [HttpGet]
    [Authorize(Policy = "Taller.Consultar")]
    public async Task<ActionResult<ListadoPaginadoDto<OrdenTrabajoDto>>> Buscar(
        [FromQuery] string? estado, [FromQuery] Guid? cliente, [FromQuery] int pagina = 1, [FromQuery] int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        var estadoEnum = string.IsNullOrWhiteSpace(estado) ? (EstadoOrdenTrabajo?)null : Enum.Parse<EstadoOrdenTrabajo>(estado);
        var resultado = await _buscarHandler.ManejarAsync(new BuscarOrdenesTrabajoQuery(estadoEnum, cliente, pagina, tamanoPagina), cancellationToken);
        return Ok(resultado);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Taller.Consultar")]
    public async Task<ActionResult<OrdenTrabajoDetalleDto>> Obtener(Guid id, CancellationToken cancellationToken)
    {
        var detalle = await _obtenerHandler.ManejarAsync(new ObtenerOrdenTrabajoQuery(id), cancellationToken);
        return Ok(detalle);
    }

    /// <summary>UC-22/RN-029 — la recepción no es exclusiva de un rol.</summary>
    [HttpPost]
    [Authorize(Policy = "Taller.Recepcionar")]
    public async Task<ActionResult<OrdenTrabajoDto>> RegistrarRecepcion(RegistrarRecepcionRequest request, CancellationToken cancellationToken)
    {
        var comando = new RegistrarRecepcionCommand(request.ClienteId, request.EquipoDescripcion, request.FallaReportada, UsuarioActualId);
        var ot = await _registrarRecepcionHandler.ManejarAsync(comando, cancellationToken);
        return CreatedAtAction(nameof(Obtener), new { id = ot.Id }, ot);
    }

    [HttpPost("{id:guid}/diagnostico")]
    [Authorize(Policy = "Taller.Diagnosticar")]
    public async Task<ActionResult<OrdenTrabajoDto>> RegistrarDiagnostico(Guid id, RegistrarDiagnosticoRequest request, CancellationToken cancellationToken)
    {
        var comando = new RegistrarDiagnosticoCommand(id, request.Descripcion, UsuarioActualId);
        var ot = await _registrarDiagnosticoHandler.ManejarAsync(comando, cancellationToken);
        return Ok(ot);
    }

    [HttpPost("{id:guid}/cotizacion")]
    [Authorize(Policy = "Taller.Cotizar")]
    public async Task<ActionResult<OrdenTrabajoDto>> GenerarCotizacion(Guid id, GenerarCotizacionRequest request, CancellationToken cancellationToken)
    {
        var comando = new GenerarCotizacionReparacionCommand(id, request.MontoEstimado);
        var ot = await _generarCotizacionHandler.ManejarAsync(comando, cancellationToken);
        return Ok(ot);
    }

    /// <summary>RN-016/RN-030 — aprobar o rechazar la cotización.</summary>
    [HttpPost("{id:guid}/decision")]
    [Authorize(Policy = "Taller.Cotizar")]
    public async Task<ActionResult<OrdenTrabajoDto>> RegistrarDecision(Guid id, RegistrarDecisionRequest request, CancellationToken cancellationToken)
    {
        var decision = Enum.Parse<DecisionCliente>(request.Decision);
        var comando = new RegistrarDecisionClienteCommand(id, decision, request.CobroDiagnosticoRechazo, request.EvidenciaAprobacion);
        var ot = await _registrarDecisionHandler.ManejarAsync(comando, cancellationToken);
        return Ok(ot);
    }

    /// <summary>RN-018 — descuenta los repuestos consumidos del inventario compartido.</summary>
    [HttpPost("{id:guid}/reparacion")]
    [Authorize(Policy = "Taller.Reparar")]
    public async Task<ActionResult<OrdenTrabajoDto>> RegistrarReparacion(Guid id, RegistrarReparacionRequest request, CancellationToken cancellationToken)
    {
        var comando = new RegistrarReparacionCommand(id, request.Consumos, request.ResultadoPruebas, UsuarioActualId);
        var ot = await _registrarReparacionHandler.ManejarAsync(comando, cancellationToken);
        return Ok(ot);
    }

    /// <summary>RN-001 — el pago no bloquea la entrega; RN-001/RN-031 — el saldo pendiente exige autorización.</summary>
    [HttpPost("{id:guid}/entrega")]
    [Authorize(Policy = "Taller.Entregar")]
    public async Task<ActionResult<OrdenTrabajoDto>> EntregarEquipo(Guid id, EntregarEquipoRequest request, CancellationToken cancellationToken)
    {
        var estadoPago = Enum.Parse<EstadoPagoOrdenTrabajo>(request.EstadoPago);
        var comando = new EntregarEquipoCommand(
            id, estadoPago, request.MontoPagado, request.SaldoPendiente, request.UsuarioAutorizoSaldoId, UsuarioActualId);
        var ot = await _entregarEquipoHandler.ManejarAsync(comando, cancellationToken);
        return Ok(ot);
    }

    /// <summary>UC-27/RN-017 — alcance V1 acotado a un período simple.</summary>
    [HttpPost("{id:guid}/garantia")]
    [Authorize(Policy = "Taller.Editar")]
    public async Task<ActionResult<GarantiaDto>> RegistrarGarantia(Guid id, RegistrarGarantiaRequest request, CancellationToken cancellationToken)
    {
        var comando = new RegistrarGarantiaCommand(id, request.FechaInicio, request.FechaFin);
        var garantia = await _registrarGarantiaHandler.ManejarAsync(comando, cancellationToken);
        return Ok(garantia);
    }
}

public record RegistrarRecepcionRequest(Guid ClienteId, string EquipoDescripcion, string FallaReportada);

public record RegistrarDiagnosticoRequest(string Descripcion);

public record GenerarCotizacionRequest(decimal MontoEstimado);

public record RegistrarDecisionRequest(string Decision, decimal? CobroDiagnosticoRechazo, string? EvidenciaAprobacion);

public record RegistrarReparacionRequest(IReadOnlyCollection<DetalleConsumoInput> Consumos, string? ResultadoPruebas);

public record EntregarEquipoRequest(
    string EstadoPago, decimal MontoPagado, decimal? SaldoPendiente, Guid? UsuarioAutorizoSaldoId);

public record RegistrarGarantiaRequest(DateOnly FechaInicio, DateOnly FechaFin);
