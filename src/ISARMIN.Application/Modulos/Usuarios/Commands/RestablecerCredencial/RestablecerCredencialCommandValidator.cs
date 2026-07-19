using FluentValidation;

namespace ISARMIN.Application.Modulos.Usuarios.Commands.RestablecerCredencial;

public class RestablecerCredencialCommandValidator : AbstractValidator<RestablecerCredencialCommand>
{
    public RestablecerCredencialCommandValidator()
    {
        RuleFor(c => c.NuevaCredencial).NotEmpty().MinimumLength(8);
    }
}
