using FluentValidation;

namespace ISARMIN.Application.Modulos.Taller.Commands.RegistrarGarantia;

public class RegistrarGarantiaCommandValidator : AbstractValidator<RegistrarGarantiaCommand>
{
    public RegistrarGarantiaCommandValidator()
    {
        RuleFor(c => c.OrdenTrabajoId).NotEmpty();
        RuleFor(c => c.FechaFin).GreaterThan(c => c.FechaInicio);
    }
}
