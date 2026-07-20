using FluentValidation;

namespace ISARMIN.Application.Modulos.Taller.Commands.RegistrarDiagnostico;

public class RegistrarDiagnosticoCommandValidator : AbstractValidator<RegistrarDiagnosticoCommand>
{
    public RegistrarDiagnosticoCommandValidator()
    {
        RuleFor(c => c.OrdenTrabajoId).NotEmpty();
        RuleFor(c => c.Descripcion).NotEmpty();
        RuleFor(c => c.UsuarioId).NotEmpty();
    }
}
