using FluentValidation;

namespace ISARMIN.Application.Modulos.ServiciosCampo.Commands.CotizarServicioCampo;

public class CotizarServicioCampoCommandValidator : AbstractValidator<CotizarServicioCampoCommand>
{
    public CotizarServicioCampoCommandValidator()
    {
        RuleFor(c => c.ServicioCampoId).NotEmpty();
        RuleFor(c => c.MontoEstimado).GreaterThanOrEqualTo(0);
    }
}
