using FluentValidation;

namespace ISARMIN.Application.Modulos.ServiciosCampo.Commands.CerrarServicioCampo;

public class CerrarServicioCampoCommandValidator : AbstractValidator<CerrarServicioCampoCommand>
{
    public CerrarServicioCampoCommandValidator()
    {
        RuleFor(c => c.ServicioCampoId).NotEmpty();
        RuleFor(c => c.EstadoFinal).NotEmpty().MaximumLength(50);
        RuleFor(c => c.UsuarioId).NotEmpty();

        RuleForEach(c => c.Consumos).ChildRules(detalle =>
        {
            detalle.RuleFor(d => d.ProductoId).NotEmpty();
            detalle.RuleFor(d => d.Cantidad).GreaterThan(0);
        });
    }
}
