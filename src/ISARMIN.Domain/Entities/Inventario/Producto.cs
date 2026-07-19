using ISARMIN.Domain.Common;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Domain.Entities.Inventario;

/// <summary>UC-10 — Gestionar Producto (RF-023 a RF-025). El stock se inicializa aquí, pero su recálculo
/// posterior (Kardex, RF-027) y los ajustes manuales (RF-030, UC-12) pertenecen al módulo de Inventario.</summary>
public class Producto : Entity
{
    public string CodigoInterno { get; private set; } = null!;
    public string? CodigoBarras { get; private set; }
    public string Nombre { get; private set; } = null!;
    public Guid CategoriaId { get; private set; }
    public string? Marca { get; private set; }
    public Guid UnidadMedidaId { get; private set; }
    public decimal CostoReferencia { get; private set; }
    public decimal PrecioVenta { get; private set; }
    public decimal StockActual { get; private set; }
    public decimal? StockMinimo { get; private set; }
    public EstadoRegistro Estado { get; private set; }

    /// <summary>Margen calculado (precio de venta - costo de referencia), no persistido (RF-023).</summary>
    public decimal Margen => PrecioVenta - CostoReferencia;

    public bool EstaActivo => Estado == EstadoRegistro.Activo;

    private Producto() { }

    public Producto(
        string codigoInterno,
        string nombre,
        Guid categoriaId,
        Guid unidadMedidaId,
        decimal costoReferencia,
        decimal precioVenta,
        decimal stockInicial,
        string? marca = null,
        string? codigoBarras = null,
        decimal? stockMinimo = null)
    {
        ValidarCodigoInterno(codigoInterno);
        ValidarNombre(nombre);
        ValidarCostoYPrecio(costoReferencia, precioVenta);
        ValidarStock(stockInicial, stockMinimo);

        CodigoInterno = codigoInterno;
        Nombre = nombre;
        CategoriaId = categoriaId;
        UnidadMedidaId = unidadMedidaId;
        CostoReferencia = costoReferencia;
        PrecioVenta = precioVenta;
        StockActual = stockInicial;
        Marca = marca;
        CodigoBarras = codigoBarras;
        StockMinimo = stockMinimo;
        Estado = EstadoRegistro.Activo;
    }

    public void ActualizarDatos(
        string codigoInterno,
        string nombre,
        Guid categoriaId,
        Guid unidadMedidaId,
        decimal costoReferencia,
        decimal precioVenta,
        string? marca,
        string? codigoBarras,
        decimal? stockMinimo)
    {
        ValidarCodigoInterno(codigoInterno);
        ValidarNombre(nombre);
        ValidarCostoYPrecio(costoReferencia, precioVenta);
        ValidarStock(StockActual, stockMinimo);

        CodigoInterno = codigoInterno;
        Nombre = nombre;
        CategoriaId = categoriaId;
        UnidadMedidaId = unidadMedidaId;
        CostoReferencia = costoReferencia;
        PrecioVenta = precioVenta;
        Marca = marca;
        CodigoBarras = codigoBarras;
        StockMinimo = stockMinimo;
    }

    public void Activar() => Estado = EstadoRegistro.Activo;

    public void Desactivar() => Estado = EstadoRegistro.Inactivo;

    /// <summary>UC-12/RN-008 — único mecanismo autorizado para modificar el stock fuera del Kardex
    /// automático; exclusivo del Administrador, aplicado por <c>AjustarInventarioCommandHandler</c>.</summary>
    public void AjustarStock(decimal cantidadAjuste)
    {
        var nuevoStock = StockActual + cantidadAjuste;
        if (nuevoStock < 0)
        {
            throw new ArgumentException("El ajuste dejaría el stock en un valor negativo.", nameof(cantidadAjuste));
        }

        StockActual = nuevoStock;
    }

    private static void ValidarCodigoInterno(string codigoInterno)
    {
        if (string.IsNullOrWhiteSpace(codigoInterno))
        {
            throw new ArgumentException("El código interno del producto es obligatorio.", nameof(codigoInterno));
        }
    }

    private static void ValidarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre del producto es obligatorio.", nameof(nombre));
        }
    }

    private static void ValidarCostoYPrecio(decimal costoReferencia, decimal precioVenta)
    {
        if (costoReferencia < 0)
        {
            throw new ArgumentException("El costo de referencia no puede ser negativo.", nameof(costoReferencia));
        }

        if (precioVenta < 0)
        {
            throw new ArgumentException("El precio de venta no puede ser negativo.", nameof(precioVenta));
        }
    }

    private static void ValidarStock(decimal stock, decimal? stockMinimo)
    {
        if (stock < 0)
        {
            throw new ArgumentException("El stock no puede ser negativo.", nameof(stock));
        }

        if (stockMinimo is < 0)
        {
            throw new ArgumentException("El stock mínimo no puede ser negativo.", nameof(stockMinimo));
        }
    }
}
