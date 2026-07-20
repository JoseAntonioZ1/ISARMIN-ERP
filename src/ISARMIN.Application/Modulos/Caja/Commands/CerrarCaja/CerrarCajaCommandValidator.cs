using FluentValidation;

namespace ISARMIN.Application.Modulos.Caja.Commands.CerrarCaja;

public class CerrarCajaCommandValidator : AbstractValidator<CerrarCajaCommand>
{
    public CerrarCajaCommandValidator()
    {
        RuleFor(c => c.MontoFisicoDeclarado).GreaterThanOrEqualTo(0);
    }
}
