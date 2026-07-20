using FluentValidation;

namespace ISARMIN.Application.Modulos.ServiciosCampo.Commands.CobrarServicioCampo;

public class CobrarServicioCampoCommandValidator : AbstractValidator<CobrarServicioCampoCommand>
{
    public CobrarServicioCampoCommandValidator()
    {
        RuleFor(c => c.ServicioCampoId).NotEmpty();
        RuleFor(c => c.MedioPagoId).NotEmpty();
        RuleFor(c => c.MontoPagado).GreaterThanOrEqualTo(0);
        RuleFor(c => c.SaldoPendiente).GreaterThan(0).When(c => c.SaldoPendiente.HasValue);
        RuleFor(c => c.UsuarioAutorizoSaldoId).NotEmpty()
            .When(c => c.SaldoPendiente.HasValue)
            .WithMessage("Se requiere el usuario Administrador/Propietario que autorizó el saldo pendiente.");
    }
}
