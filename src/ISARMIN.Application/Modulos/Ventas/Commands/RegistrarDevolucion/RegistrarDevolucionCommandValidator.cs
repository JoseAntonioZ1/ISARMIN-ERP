using FluentValidation;

namespace ISARMIN.Application.Modulos.Ventas.Commands.RegistrarDevolucion;

public class RegistrarDevolucionCommandValidator : AbstractValidator<RegistrarDevolucionCommand>
{
    public RegistrarDevolucionCommandValidator()
    {
        RuleFor(c => c.VentaId).NotEmpty();
        RuleFor(c => c.UsuarioId).NotEmpty();
        RuleFor(c => c.Detalles).NotEmpty();

        RuleForEach(c => c.Detalles).ChildRules(detalle =>
        {
            detalle.RuleFor(d => d.ProductoId).NotEmpty();
            detalle.RuleFor(d => d.Cantidad).GreaterThan(0);
        });
    }
}
