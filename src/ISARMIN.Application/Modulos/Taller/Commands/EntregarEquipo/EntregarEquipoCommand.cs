using ISARMIN.Domain.Enums;

namespace ISARMIN.Application.Modulos.Taller.Commands.EntregarEquipo;

public record EntregarEquipoCommand(
    Guid OrdenTrabajoId,
    EstadoPagoOrdenTrabajo EstadoPago,
    decimal MontoPagado,
    decimal? SaldoPendiente,
    Guid? UsuarioAutorizoSaldoId,
    Guid UsuarioEntregaId);
