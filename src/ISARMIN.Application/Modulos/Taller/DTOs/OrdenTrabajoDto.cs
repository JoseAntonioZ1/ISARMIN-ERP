using ISARMIN.Domain.Entities.Taller;

namespace ISARMIN.Application.Modulos.Taller.DTOs;

public record DiagnosticoDto(Guid Id, string Descripcion, Guid UsuarioId, DateTime Fecha);

public record CotizacionReparacionDto(
    Guid Id,
    decimal MontoEstimado,
    DateTime Fecha,
    string? DecisionCliente,
    decimal? CobroDiagnosticoRechazo,
    string? EvidenciaAprobacion);

public record ConsumoRepuestoDto(Guid Id, Guid ProductoId, decimal Cantidad);

public record OrdenTrabajoDto(
    Guid Id,
    Guid ClienteId,
    string EquipoDescripcion,
    string FallaReportada,
    DateTime FechaRecepcion,
    Guid UsuarioRecepcionId,
    string Estado,
    DateTime? FechaEntrega,
    Guid? UsuarioEntregaId,
    string? EstadoPago,
    decimal? MontoPagado,
    decimal? SaldoPendiente,
    Guid? UsuarioAutorizoSaldoId,
    string? ResultadoPruebas,
    DiagnosticoDto? Diagnostico,
    CotizacionReparacionDto? CotizacionReparacion,
    IReadOnlyCollection<ConsumoRepuestoDto> ConsumosRepuesto);

public static class OrdenTrabajoMapper
{
    public static OrdenTrabajoDto ADto(this OrdenTrabajo ot) => new(
        ot.Id,
        ot.ClienteId,
        ot.EquipoDescripcion,
        ot.FallaReportada,
        ot.FechaRecepcion,
        ot.UsuarioRecepcionId,
        ot.Estado.ToString(),
        ot.FechaEntrega,
        ot.UsuarioEntregaId,
        ot.EstadoPago?.ToString(),
        ot.MontoPagado,
        ot.SaldoPendiente,
        ot.UsuarioAutorizoSaldoId,
        ot.ResultadoPruebas,
        ot.Diagnostico is null ? null : new DiagnosticoDto(ot.Diagnostico.Id, ot.Diagnostico.Descripcion, ot.Diagnostico.UsuarioId, ot.Diagnostico.Fecha),
        ot.CotizacionReparacion is null
            ? null
            : new CotizacionReparacionDto(
                ot.CotizacionReparacion.Id,
                ot.CotizacionReparacion.MontoEstimado,
                ot.CotizacionReparacion.Fecha,
                ot.CotizacionReparacion.DecisionCliente?.ToString(),
                ot.CotizacionReparacion.CobroDiagnosticoRechazo,
                ot.CotizacionReparacion.EvidenciaAprobacion),
        ot.ConsumosRepuesto.Select(c => new ConsumoRepuestoDto(c.Id, c.ProductoId, c.Cantidad)).ToList());
}
