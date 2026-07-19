using FluentValidation;

namespace ISARMIN.Application.Modulos.Usuarios.Commands.IniciarSesion;

public class IniciarSesionCommandValidator : AbstractValidator<IniciarSesionCommand>
{
    public IniciarSesionCommandValidator()
    {
        RuleFor(c => c.NombreUsuario).NotEmpty();
        RuleFor(c => c.Credencial).NotEmpty();
    }
}
