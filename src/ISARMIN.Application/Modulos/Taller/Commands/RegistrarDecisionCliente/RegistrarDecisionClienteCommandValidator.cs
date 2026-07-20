using FluentValidation;

namespace ISARMIN.Application.Modulos.Taller.Commands.RegistrarDecisionCliente;

public class RegistrarDecisionClienteCommandValidator : AbstractValidator<RegistrarDecisionClienteCommand>
{
    public RegistrarDecisionClienteCommandValidator()
    {
        RuleFor(c => c.OrdenTrabajoId).NotEmpty();
        RuleFor(c => c.Decision).IsInEnum();
        RuleFor(c => c.CobroDiagnosticoRechazo).GreaterThanOrEqualTo(0).When(c => c.CobroDiagnosticoRechazo.HasValue);
        RuleFor(c => c.EvidenciaAprobacion).MaximumLength(255);
    }
}
