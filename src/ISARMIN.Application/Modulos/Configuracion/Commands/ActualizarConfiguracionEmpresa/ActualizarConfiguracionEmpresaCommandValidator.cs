using FluentValidation;

namespace ISARMIN.Application.Modulos.Configuracion.Commands.ActualizarConfiguracionEmpresa;

public class ActualizarConfiguracionEmpresaCommandValidator : AbstractValidator<ActualizarConfiguracionEmpresaCommand>
{
    public ActualizarConfiguracionEmpresaCommandValidator()
    {
        RuleFor(c => c.RazonSocial).NotEmpty().MaximumLength(200);
        RuleFor(c => c.Ruc).MaximumLength(11);
        RuleFor(c => c.MontoAperturaCajaPredeterminado).GreaterThanOrEqualTo(0).When(c => c.MontoAperturaCajaPredeterminado.HasValue);
        RuleFor(c => c.ColorAcento).Matches("^#[0-9A-Fa-f]{6}$").When(c => c.ColorAcento is not null)
            .WithMessage("El color de acento debe tener el formato hexadecimal '#RRGGBB'.");
        RuleFor(c => c.MensajeBienvenida).MaximumLength(500);
    }
}
