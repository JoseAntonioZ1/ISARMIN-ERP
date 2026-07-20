using ISARMIN.Domain.Entities.ServiciosCampo;

namespace ISARMIN.Application.Modulos.ServiciosCampo.DTOs;

public record ServicioCampoDetalleDto(Guid Id, Guid ProductoId, decimal Cantidad);

public record ServicioCampoDto(
    Guid Id,
    Guid ClienteId,
    string DescripcionTrabajo,
    DateTime FechaSolicitud,
    Guid? TecnicoAsignadoId,
    string Estado,
    decimal? MontoEstimado,
    DateTime? FechaEjecucion,
    string? EstadoFinal,
    string? Observaciones,
    Guid? UsuarioCierreId,
    Guid? MedioPagoId,
    decimal? MontoPagado,
    decimal? SaldoPendiente,
    Guid? UsuarioAutorizoSaldoId,
    IReadOnlyCollection<ServicioCampoDetalleDto> Detalles);

public static class ServicioCampoMapper
{
    public static ServicioCampoDto ADto(this ServicioCampo servicio) => new(
        servicio.Id,
        servicio.ClienteId,
        servicio.DescripcionTrabajo,
        servicio.FechaSolicitud,
        servicio.TecnicoAsignadoId,
        servicio.Estado.ToString(),
        servicio.MontoEstimado,
        servicio.FechaEjecucion,
        servicio.EstadoFinal,
        servicio.Observaciones,
        servicio.UsuarioCierreId,
        servicio.MedioPagoId,
        servicio.MontoPagado,
        servicio.SaldoPendiente,
        servicio.UsuarioAutorizoSaldoId,
        servicio.Detalles.Select(d => new ServicioCampoDetalleDto(d.Id, d.ProductoId, d.Cantidad)).ToList());
}
