using ISARMIN.Domain.Common;

namespace ISARMIN.Domain.Entities.ServiciosCampo;

/// <summary>UC-32/RF-067 — material/repuesto consumido en un servicio de campo, descontado del
/// inventario compartido (RN-020).</summary>
public class ServicioCampoDetalle : Entity
{
    public Guid ServicioCampoId { get; private set; }
    public Guid ProductoId { get; private set; }
    public decimal Cantidad { get; private set; }

    private ServicioCampoDetalle() { }

    public ServicioCampoDetalle(Guid servicioCampoId, Guid productoId, decimal cantidad)
    {
        if (cantidad <= 0)
        {
            throw new ArgumentException("La cantidad consumida debe ser mayor a cero.", nameof(cantidad));
        }

        ServicioCampoId = servicioCampoId;
        ProductoId = productoId;
        Cantidad = cantidad;
    }
}
