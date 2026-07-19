using FluentValidation;

namespace ISARMIN.Application.Modulos.Inventario.Commands.EditarCategoria;

public class EditarCategoriaCommandValidator : AbstractValidator<EditarCategoriaCommand>
{
    public EditarCategoriaCommandValidator()
    {
        RuleFor(c => c.Nombre).NotEmpty().MaximumLength(100);
    }
}
