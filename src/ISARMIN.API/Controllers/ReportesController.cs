using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Reportes.DTOs;
using ISARMIN.Application.Modulos.Reportes.Queries.ReporteCaja;
using ISARMIN.Application.Modulos.Reportes.Queries.ReporteInventario;
using ISARMIN.Application.Modulos.Reportes.Queries.ReporteOrdenesTrabajo;
using ISARMIN.Application.Modulos.Reportes.Queries.ReporteServiciosCampo;
using ISARMIN.Application.Modulos.Reportes.Queries.ReporteVentas;
using ISARMIN.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ISARMIN.API.Controllers;

/// <summary>UC-35 — Reportes operativos (RF-072 a RF-076); solo lectura.</summary>
[ApiController]
[Route("api/v1/reportes")]
[Authorize(Policy = "Reportes.Consultar")]
public class ReportesController : ControllerBase
{
    private readonly IQueryHandler<ReporteVentasQuery, ReporteVentasDto> _reporteVentasHandler;
    private readonly IQueryHandler<ReporteInventarioQuery, ReporteInventarioDto> _reporteInventarioHandler;
    private readonly IQueryHandler<ReporteOrdenesTrabajoQuery, ReporteOrdenesTrabajoDto> _reporteOrdenesTrabajoHandler;
    private readonly IQueryHandler<ReporteServiciosCampoQuery, ReporteServiciosCampoDto> _reporteServiciosCampoHandler;
    private readonly IQueryHandler<ReporteCajaQuery, ReporteCajaDto> _reporteCajaHandler;

    public ReportesController(
        IQueryHandler<ReporteVentasQuery, ReporteVentasDto> reporteVentasHandler,
        IQueryHandler<ReporteInventarioQuery, ReporteInventarioDto> reporteInventarioHandler,
        IQueryHandler<ReporteOrdenesTrabajoQuery, ReporteOrdenesTrabajoDto> reporteOrdenesTrabajoHandler,
        IQueryHandler<ReporteServiciosCampoQuery, ReporteServiciosCampoDto> reporteServiciosCampoHandler,
        IQueryHandler<ReporteCajaQuery, ReporteCajaDto> reporteCajaHandler)
    {
        _reporteVentasHandler = reporteVentasHandler;
        _reporteInventarioHandler = reporteInventarioHandler;
        _reporteOrdenesTrabajoHandler = reporteOrdenesTrabajoHandler;
        _reporteServiciosCampoHandler = reporteServiciosCampoHandler;
        _reporteCajaHandler = reporteCajaHandler;
    }

    /// <summary>RF-072.</summary>
    [HttpGet("ventas")]
    public async Task<ActionResult<ReporteVentasDto>> ReporteVentas(
        [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, CancellationToken cancellationToken)
    {
        var reporte = await _reporteVentasHandler.ManejarAsync(new ReporteVentasQuery(ComoUtc(desde), ComoUtc(hasta)), cancellationToken);
        return Ok(reporte);
    }

    /// <summary>RF-073 — incluye el subconjunto de productos en quiebre de stock.</summary>
    [HttpGet("inventario")]
    public async Task<ActionResult<ReporteInventarioDto>> ReporteInventario(CancellationToken cancellationToken)
    {
        var reporte = await _reporteInventarioHandler.ManejarAsync(new ReporteInventarioQuery(), cancellationToken);
        return Ok(reporte);
    }

    /// <summary>RF-074 — filtra por estado y período; no filtra por técnico (OrdenTrabajo no tiene ese campo).</summary>
    [HttpGet("ordenes-trabajo")]
    public async Task<ActionResult<ReporteOrdenesTrabajoDto>> ReporteOrdenesTrabajo(
        [FromQuery] string? estado, [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, CancellationToken cancellationToken)
    {
        var estadoEnum = string.IsNullOrWhiteSpace(estado) ? (EstadoOrdenTrabajo?)null : Enum.Parse<EstadoOrdenTrabajo>(estado);
        var reporte = await _reporteOrdenesTrabajoHandler.ManejarAsync(
            new ReporteOrdenesTrabajoQuery(estadoEnum, ComoUtc(desde), ComoUtc(hasta)), cancellationToken);
        return Ok(reporte);
    }

    /// <summary>RF-075 — filtra por técnico asignado y período.</summary>
    [HttpGet("servicios-campo")]
    public async Task<ActionResult<ReporteServiciosCampoDto>> ReporteServiciosCampo(
        [FromQuery] Guid? tecnico, [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, CancellationToken cancellationToken)
    {
        var reporte = await _reporteServiciosCampoHandler.ManejarAsync(
            new ReporteServiciosCampoQuery(tecnico, ComoUtc(desde), ComoUtc(hasta)), cancellationToken);
        return Ok(reporte);
    }

    /// <summary>RF-076 — refleja únicamente los movimientos manuales de Caja.</summary>
    [HttpGet("caja")]
    public async Task<ActionResult<ReporteCajaDto>> ReporteCaja(
        [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, CancellationToken cancellationToken)
    {
        var reporte = await _reporteCajaHandler.ManejarAsync(new ReporteCajaQuery(ComoUtc(desde), ComoUtc(hasta)), cancellationToken);
        return Ok(reporte);
    }

    /// <summary>ASP.NET Core enlaza los parámetros de query DateTime con Kind=Unspecified; Npgsql
    /// exige Kind=Utc para columnas timestamptz. Sin esto, cualquier filtro desde/hasta lanza una
    /// excepción no controlada (bug real preexistente, ver también Kardex/Caja).</summary>
    private static DateTime? ComoUtc(DateTime? valor) => valor is { } v ? DateTime.SpecifyKind(v, DateTimeKind.Utc) : null;
}
