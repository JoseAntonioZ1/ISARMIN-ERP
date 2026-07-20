using FluentValidation;

namespace ISARMIN.Application.Modulos.Taller.Commands.EntregarEquipo;

public class EntregarEquipoCommandValidator : AbstractValidator<EntregarEquipoCommand>
{
    public EntregarEquipoCommandValidator()
    {
        RuleFor(c => c.OrdenTrabajoId).NotEmpty();
        RuleFor(c => c.EstadoPago).IsInEnum();
        RuleFor(c => c.MontoPagado).GreaterThanOrEqualTo(0);
        RuleFor(c => c.UsuarioEntregaId).NotEmpty();
        RuleFor(c => c.SaldoPendiente).GreaterThan(0).When(c => c.SaldoPendiente.HasValue);
        RuleFor(c => c.UsuarioAutorizoSaldoId).NotEmpty()
            .When(c => c.EstadoPago == Domain.Enums.EstadoPagoOrdenTrabajo.SaldoPendiente)
            .WithMessage("Se requiere el usuario Administrador/Propietario que autorizó el saldo pendiente.");
    }
}
