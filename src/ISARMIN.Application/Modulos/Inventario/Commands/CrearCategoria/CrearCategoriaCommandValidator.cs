using FluentValidation;

namespace ISARMIN.Application.Modulos.Inventario.Commands.CrearCategoria;

public class CrearCategoriaCommandValidator : AbstractValidator<CrearCategoriaCommand>
{
    public CrearCategoriaCommandValidator()
    {
        RuleFor(c => c.Nombre).NotEmpty().MaximumLength(100);
    }
}
