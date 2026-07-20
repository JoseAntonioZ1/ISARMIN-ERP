using FluentValidation;

namespace ISARMIN.Application.Modulos.Ventas.Commands.RegistrarVenta;

public class RegistrarVentaCommandValidator : AbstractValidator<RegistrarVentaCommand>
{
    public RegistrarVentaCommandValidator()
    {
        RuleFor(c => c.TipoComprobante).IsInEnum();
        RuleFor(c => c.UsuarioId).NotEmpty();
        RuleFor(c => c.Detalles).NotEmpty();

        RuleForEach(c => c.Detalles).ChildRules(detalle =>
        {
            detalle.RuleFor(d => d.ProductoId).NotEmpty();
            detalle.RuleFor(d => d.Cantidad).GreaterThan(0);
            detalle.RuleFor(d => d.PrecioUnitario).GreaterThanOrEqualTo(0);
        });

        RuleForEach(c => c.Pagos).ChildRules(pago =>
        {
            pago.RuleFor(p => p.MedioPagoId).NotEmpty();
            pago.RuleFor(p => p.Monto).GreaterThan(0);
        });

        RuleFor(c => c).Custom((comando, contexto) =>
        {
            var total = comando.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario);
            var montoPagado = comando.Pagos.Sum(p => p.Monto);
            var saldoPendiente = total - montoPagado;

            if (saldoPendiente < 0)
            {
                contexto.AddFailure(nameof(comando.Pagos), "El monto pagado no puede exceder el total de la venta.");
            }
            else if (saldoPendiente > 0 && comando.UsuarioAutorizoSaldoId is null)
            {
                contexto.AddFailure(
                    nameof(comando.UsuarioAutorizoSaldoId),
                    "Se requiere el usuario Administrador/Propietario que autorizó el saldo pendiente.");
            }
        });
    }
}
