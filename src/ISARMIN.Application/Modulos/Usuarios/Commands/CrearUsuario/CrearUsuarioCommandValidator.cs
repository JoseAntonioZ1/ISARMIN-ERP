using FluentValidation;

namespace ISARMIN.Application.Modulos.Usuarios.Commands.CrearUsuario;

public class CrearUsuarioCommandValidator : AbstractValidator<CrearUsuarioCommand>
{
    public CrearUsuarioCommandValidator()
    {
        RuleFor(c => c.Nombre).NotEmpty().MaximumLength(150);
        RuleFor(c => c.NombreUsuario).NotEmpty().MaximumLength(50);
        RuleFor(c => c.CredencialInicial).NotEmpty().MinimumLength(8);
        RuleFor(c => c.RolIds).NotEmpty().WithMessage("Debe asignarse al menos un rol.");
    }
}
