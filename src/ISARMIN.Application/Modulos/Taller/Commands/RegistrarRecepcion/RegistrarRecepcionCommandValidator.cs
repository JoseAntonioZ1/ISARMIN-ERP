using FluentValidation;

namespace ISARMIN.Application.Modulos.Taller.Commands.RegistrarRecepcion;

public class RegistrarRecepcionCommandValidator : AbstractValidator<RegistrarRecepcionCommand>
{
    public RegistrarRecepcionCommandValidator()
    {
        RuleFor(c => c.ClienteId).NotEmpty();
        RuleFor(c => c.EquipoDescripcion).NotEmpty().MaximumLength(255);
        RuleFor(c => c.FallaReportada).NotEmpty();
        RuleFor(c => c.UsuarioId).NotEmpty();
    }
}
