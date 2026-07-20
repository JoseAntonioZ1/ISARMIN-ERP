using FluentValidation;

namespace ISARMIN.Application.Modulos.Taller.Commands.RegistrarReparacion;

public class RegistrarReparacionCommandValidator : AbstractValidator<RegistrarReparacionCommand>
{
    public RegistrarReparacionCommandValidator()
    {
        RuleFor(c => c.OrdenTrabajoId).NotEmpty();
        RuleFor(c => c.UsuarioId).NotEmpty();
        RuleFor(c => c.ResultadoPruebas).MaximumLength(1000);

        RuleForEach(c => c.Consumos).ChildRules(detalle =>
        {
            detalle.RuleFor(d => d.ProductoId).NotEmpty();
            detalle.RuleFor(d => d.Cantidad).GreaterThan(0);
        });
    }
}
