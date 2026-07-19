using FluentValidation;

namespace ISARMIN.Application.Modulos.Inventario.Commands.EditarUnidadMedida;

public class EditarUnidadMedidaCommandValidator : AbstractValidator<EditarUnidadMedidaCommand>
{
    public EditarUnidadMedidaCommandValidator()
    {
        RuleFor(c => c.Nombre).NotEmpty().MaximumLength(50);
    }
}
