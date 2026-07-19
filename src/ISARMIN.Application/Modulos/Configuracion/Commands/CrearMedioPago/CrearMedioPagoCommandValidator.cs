using FluentValidation;

namespace ISARMIN.Application.Modulos.Configuracion.Commands.CrearMedioPago;

public class CrearMedioPagoCommandValidator : AbstractValidator<CrearMedioPagoCommand>
{
    public CrearMedioPagoCommandValidator()
    {
        RuleFor(c => c.Nombre).NotEmpty().MaximumLength(50);
    }
}
