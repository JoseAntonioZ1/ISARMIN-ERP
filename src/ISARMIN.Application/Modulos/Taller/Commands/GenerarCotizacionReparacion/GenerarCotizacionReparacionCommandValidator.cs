using FluentValidation;

namespace ISARMIN.Application.Modulos.Taller.Commands.GenerarCotizacionReparacion;

public class GenerarCotizacionReparacionCommandValidator : AbstractValidator<GenerarCotizacionReparacionCommand>
{
    public GenerarCotizacionReparacionCommandValidator()
    {
        RuleFor(c => c.OrdenTrabajoId).NotEmpty();
        RuleFor(c => c.MontoEstimado).GreaterThanOrEqualTo(0);
    }
}
