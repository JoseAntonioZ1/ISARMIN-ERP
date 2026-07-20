using FluentValidation;

namespace ISARMIN.Application.Modulos.Caja.Commands.RegistrarMovimientoCaja;

public class RegistrarMovimientoCajaCommandValidator : AbstractValidator<RegistrarMovimientoCajaCommand>
{
    public RegistrarMovimientoCajaCommandValidator()
    {
        RuleFor(c => c.Concepto).IsInEnum();
        RuleFor(c => c.Monto).GreaterThan(0);
        RuleFor(c => c.Descripcion).MaximumLength(255);
        RuleFor(c => c.UsuarioId).NotEmpty();
    }
}
