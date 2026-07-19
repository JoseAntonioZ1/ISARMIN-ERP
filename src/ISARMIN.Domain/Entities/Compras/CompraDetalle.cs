using ISARMIN.Domain.Common;

namespace ISARMIN.Domain.Entities.Compras;

/// <summary>Línea de detalle de una Compra (RF-033) — un producto adquirido, su cantidad y costo unitario.</summary>
public class CompraDetalle : Entity
{
    public Guid CompraId { get; private set; }
    public Guid ProductoId { get; private set; }
    public decimal Cantidad { get; private set; }
    public decimal CostoUnitario { get; private set; }

    private CompraDetalle() { }

    public CompraDetalle(Guid compraId, Guid productoId, decimal cantidad, decimal costoUnitario)
    {
        if (cantidad <= 0)
        {
            throw new ArgumentException("La cantidad comprada debe ser mayor a cero.", nameof(cantidad));
        }

        if (costoUnitario < 0)
        {
            throw new ArgumentException("El costo unitario no puede ser negativo.", nameof(costoUnitario));
        }

        CompraId = compraId;
        ProductoId = productoId;
        Cantidad = cantidad;
        CostoUnitario = costoUnitario;
    }
}
