using ISARMIN.Domain.Common;

namespace ISARMIN.Domain.Entities.Compras;

/// <summary>UC-13 — Registrar Compra (RF-033 a RF-036). Compra directa, ya realizada, sin flujo de
/// orden de compra ni aprobación previa (RN-024). Solo se registra; no existe edición ni anulación
/// en el alcance actual — corregirla requeriría revertir Kardex y costo promedio, fuera de RF-033 a RF-036.</summary>
public class Compra : Entity
{
    public Guid ProveedorId { get; private set; }
    public DateOnly Fecha { get; private set; }
    public string DocumentoCompraTipo { get; private set; } = null!;
    public string DocumentoCompraNumero { get; private set; } = null!;
    public Guid UsuarioId { get; private set; }
    public decimal Total { get; private set; }

    private readonly List<CompraDetalle> _detalles = [];
    public IReadOnlyCollection<CompraDetalle> Detalles => _detalles.AsReadOnly();

    private Compra() { }

    public Compra(
        Guid proveedorId,
        DateOnly fecha,
        string documentoCompraTipo,
        string documentoCompraNumero,
        Guid usuarioId,
        IEnumerable<(Guid ProductoId, decimal Cantidad, decimal CostoUnitario)> detalles)
    {
        if (proveedorId == Guid.Empty)
        {
            throw new ArgumentException("El proveedor es obligatorio.", nameof(proveedorId));
        }

        if (string.IsNullOrWhiteSpace(documentoCompraTipo))
        {
            throw new ArgumentException("El tipo de documento de compra es obligatorio.", nameof(documentoCompraTipo));
        }

        if (string.IsNullOrWhiteSpace(documentoCompraNumero))
        {
            throw new ArgumentException("El número de documento de compra es obligatorio.", nameof(documentoCompraNumero));
        }

        var lista = detalles.ToList();
        if (lista.Count == 0)
        {
            throw new ArgumentException("La compra debe tener al menos un producto.", nameof(detalles));
        }

        ProveedorId = proveedorId;
        Fecha = fecha;
        DocumentoCompraTipo = documentoCompraTipo;
        DocumentoCompraNumero = documentoCompraNumero;
        UsuarioId = usuarioId;

        foreach (var (productoId, cantidad, costoUnitario) in lista)
        {
            _detalles.Add(new CompraDetalle(Id, productoId, cantidad, costoUnitario));
        }

        Total = _detalles.Sum(d => d.Cantidad * d.CostoUnitario);
    }
}
