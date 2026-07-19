using FluentValidation;

namespace ISARMIN.Application.Modulos.Compras.Commands.RegistrarCompra;

public class RegistrarCompraCommandValidator : AbstractValidator<RegistrarCompraCommand>
{
    public RegistrarCompraCommandValidator()
    {
        RuleFor(c => c.ProveedorId).NotEmpty();
        RuleFor(c => c.Fecha).NotEqual(default(DateOnly)).WithMessage("La fecha de la compra es obligatoria.");
        RuleFor(c => c.DocumentoCompraTipo).NotEmpty().MaximumLength(30);
        RuleFor(c => c.DocumentoCompraNumero).NotEmpty().MaximumLength(50);
        RuleFor(c => c.UsuarioId).NotEmpty();
        RuleFor(c => c.Detalles).NotEmpty().WithMessage("La compra debe tener al menos un producto.");

        RuleForEach(c => c.Detalles).ChildRules(detalle =>
        {
            detalle.RuleFor(d => d.ProductoId).NotEmpty();
            detalle.RuleFor(d => d.Cantidad).GreaterThan(0);
            detalle.RuleFor(d => d.CostoUnitario).GreaterThanOrEqualTo(0);
        });
    }
}
