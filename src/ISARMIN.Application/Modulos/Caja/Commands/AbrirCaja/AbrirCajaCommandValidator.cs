using FluentValidation;

namespace ISARMIN.Application.Modulos.Caja.Commands.AbrirCaja;

public class AbrirCajaCommandValidator : AbstractValidator<AbrirCajaCommand>
{
    public AbrirCajaCommandValidator()
    {
        RuleFor(c => c.MontoApertura).GreaterThanOrEqualTo(0);
        RuleFor(c => c.UsuarioId).NotEmpty();
    }
}
