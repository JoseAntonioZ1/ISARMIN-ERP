using FluentValidation;

namespace ISARMIN.Application.Modulos.Inventario.Commands.RegistrarProducto;

public class RegistrarProductoCommandValidator : AbstractValidator<RegistrarProductoCommand>
{
    public RegistrarProductoCommandValidator()
    {
        RuleFor(c => c.CodigoInterno).NotEmpty().MaximumLength(50);
        RuleFor(c => c.Nombre).NotEmpty().MaximumLength(200);
        RuleFor(c => c.CategoriaId).NotEmpty();
        RuleFor(c => c.UnidadMedidaId).NotEmpty();
        RuleFor(c => c.CostoReferencia).GreaterThanOrEqualTo(0);
        RuleFor(c => c.PrecioVenta).GreaterThanOrEqualTo(0);
        RuleFor(c => c.StockInicial).GreaterThanOrEqualTo(0);
        RuleFor(c => c.StockMinimo).GreaterThanOrEqualTo(0).When(c => c.StockMinimo.HasValue);
        RuleFor(c => c.Marca).MaximumLength(100);
        RuleFor(c => c.CodigoBarras).MaximumLength(50);
    }
}
