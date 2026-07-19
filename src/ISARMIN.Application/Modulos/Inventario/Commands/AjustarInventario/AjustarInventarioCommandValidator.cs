using FluentValidation;

namespace ISARMIN.Application.Modulos.Inventario.Commands.AjustarInventario;

public class AjustarInventarioCommandValidator : AbstractValidator<AjustarInventarioCommand>
{
    public AjustarInventarioCommandValidator()
    {
        RuleFor(c => c.ProductoId).NotEmpty();
        RuleFor(c => c.CantidadAjuste).NotEqual(0).WithMessage("El ajuste debe ser distinto de cero.");
        RuleFor(c => c.Motivo).NotEmpty().WithMessage("El motivo del ajuste es obligatorio.").MaximumLength(500);
        RuleFor(c => c.UsuarioId).NotEmpty();
    }
}
