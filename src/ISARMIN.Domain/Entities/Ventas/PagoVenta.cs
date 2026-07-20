using ISARMIN.Domain.Common;

namespace ISARMIN.Domain.Entities.Ventas;

/// <summary>Pago registrado sobre una Venta (RF-042) — permite combinar varios medios de pago en una
/// misma venta.</summary>
public class PagoVenta : Entity
{
    public Guid VentaId { get; private set; }
    public Guid MedioPagoId { get; private set; }
    public decimal Monto { get; private set; }

    private PagoVenta() { }

    public PagoVenta(Guid ventaId, Guid medioPagoId, decimal monto)
    {
        if (monto <= 0)
        {
            throw new ArgumentException("El monto del pago debe ser mayor a cero.", nameof(monto));
        }

        VentaId = ventaId;
        MedioPagoId = medioPagoId;
        Monto = monto;
    }
}
