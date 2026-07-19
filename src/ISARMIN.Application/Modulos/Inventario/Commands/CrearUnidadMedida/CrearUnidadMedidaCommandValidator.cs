using FluentValidation;

namespace ISARMIN.Application.Modulos.Inventario.Commands.CrearUnidadMedida;

public class CrearUnidadMedidaCommandValidator : AbstractValidator<CrearUnidadMedidaCommand>
{
    public CrearUnidadMedidaCommandValidator()
    {
        RuleFor(c => c.Nombre).NotEmpty().MaximumLength(50);
    }
}
