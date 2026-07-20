using ISARMIN.Domain.Common;

namespace ISARMIN.Domain.Entities.Ventas;

/// <summary>Línea de detalle de una Venta (RF-038) — un producto vendido, su cantidad y precio unitario.</summary>
public class VentaDetalle : Entity
{
    public Guid VentaId { get; private set; }
    public Guid ProductoId { get; private set; }
    public decimal Cantidad { get; private set; }
    public decimal PrecioUnitario { get; private set; }

    private VentaDetalle() { }

    public VentaDetalle(Guid ventaId, Guid productoId, decimal cantidad, decimal precioUnitario)
    {
        if (cantidad <= 0)
        {
            throw new ArgumentException("La cantidad vendida debe ser mayor a cero.", nameof(cantidad));
        }

        if (precioUnitario < 0)
        {
            throw new ArgumentException("El precio unitario no puede ser negativo.", nameof(precioUnitario));
        }

        VentaId = ventaId;
        ProductoId = productoId;
        Cantidad = cantidad;
        PrecioUnitario = precioUnitario;
    }
}
