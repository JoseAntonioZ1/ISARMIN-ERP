using FluentValidation;

namespace ISARMIN.Application.Modulos.Ventas.Commands.AnularVenta;

public class AnularVentaCommandValidator : AbstractValidator<AnularVentaCommand>
{
    public AnularVentaCommandValidator()
    {
        RuleFor(c => c.VentaId).NotEmpty();
        RuleFor(c => c.Motivo).NotEmpty();
        RuleFor(c => c.UsuarioId).NotEmpty();
    }
}
