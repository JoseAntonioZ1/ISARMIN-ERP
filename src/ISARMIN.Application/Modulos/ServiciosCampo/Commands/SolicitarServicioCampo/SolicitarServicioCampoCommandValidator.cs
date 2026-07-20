using FluentValidation;

namespace ISARMIN.Application.Modulos.ServiciosCampo.Commands.SolicitarServicioCampo;

public class SolicitarServicioCampoCommandValidator : AbstractValidator<SolicitarServicioCampoCommand>
{
    public SolicitarServicioCampoCommandValidator()
    {
        RuleFor(c => c.ClienteId).NotEmpty();
        RuleFor(c => c.DescripcionTrabajo).NotEmpty();
    }
}
