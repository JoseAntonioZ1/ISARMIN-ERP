using FluentValidation;

namespace ISARMIN.Application.Modulos.Usuarios.Commands.CrearRol;

public class CrearRolCommandValidator : AbstractValidator<CrearRolCommand>
{
    public CrearRolCommandValidator()
    {
        RuleFor(c => c.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(c => c.Descripcion).MaximumLength(500);
    }
}
