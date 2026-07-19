using FluentValidation;

namespace ISARMIN.Application.Modulos.Usuarios.Commands.EditarUsuario;

public class EditarUsuarioCommandValidator : AbstractValidator<EditarUsuarioCommand>
{
    public EditarUsuarioCommandValidator()
    {
        RuleFor(c => c.Nombre).NotEmpty().MaximumLength(150);
        RuleFor(c => c.RolIds).NotEmpty().WithMessage("Debe asignarse al menos un rol.");
    }
}
