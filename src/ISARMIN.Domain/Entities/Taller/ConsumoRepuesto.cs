using ISARMIN.Domain.Common;

namespace ISARMIN.Domain.Entities.Taller;

/// <summary>UC-25/RF-057 — repuesto consumido durante una reparación, descontado del inventario compartido (RN-018).</summary>
public class ConsumoRepuesto : Entity
{
    public Guid OrdenTrabajoId { get; private set; }
    public Guid ProductoId { get; private set; }
    public decimal Cantidad { get; private set; }

    private ConsumoRepuesto() { }

    public ConsumoRepuesto(Guid ordenTrabajoId, Guid productoId, decimal cantidad)
    {
        if (cantidad <= 0)
        {
            throw new ArgumentException("La cantidad consumida debe ser mayor a cero.", nameof(cantidad));
        }

        OrdenTrabajoId = ordenTrabajoId;
        ProductoId = productoId;
        Cantidad = cantidad;
    }
}
