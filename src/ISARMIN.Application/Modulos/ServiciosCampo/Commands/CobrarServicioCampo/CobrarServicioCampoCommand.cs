namespace ISARMIN.Application.Modulos.ServiciosCampo.Commands.CobrarServicioCampo;

public record CobrarServicioCampoCommand(
    Guid ServicioCampoId, Guid MedioPagoId, decimal MontoPagado, decimal? SaldoPendiente, Guid? UsuarioAutorizoSaldoId);
