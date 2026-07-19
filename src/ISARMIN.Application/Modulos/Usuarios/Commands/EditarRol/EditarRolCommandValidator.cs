using FluentValidation;

namespace ISARMIN.Application.Modulos.Usuarios.Commands.EditarRol;

public class EditarRolCommandValidator : AbstractValidator<EditarRolCommand>
{
    public EditarRolCommandValidator()
    {
        RuleFor(c => c.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(c => c.Descripcion).MaximumLength(500);
    }
}
