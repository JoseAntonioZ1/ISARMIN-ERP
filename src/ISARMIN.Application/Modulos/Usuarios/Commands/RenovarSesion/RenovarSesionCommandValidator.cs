using FluentValidation;

namespace ISARMIN.Application.Modulos.Usuarios.Commands.RenovarSesion;

public class RenovarSesionCommandValidator : AbstractValidator<RenovarSesionCommand>
{
    public RenovarSesionCommandValidator()
    {
        RuleFor(c => c.RefreshToken).NotEmpty();
    }
}
