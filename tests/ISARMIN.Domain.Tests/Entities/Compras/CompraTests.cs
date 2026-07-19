using ISARMIN.Domain.Entities.Compras;
using Xunit;

namespace ISARMIN.Domain.Tests.Entities.Compras;

public class CompraTests
{
    private static readonly DateOnly Fecha = new(2026, 7, 19);

    private static (Guid ProductoId, decimal Cantidad, decimal CostoUnitario)[] DetalleValido() =>
        [(Guid.NewGuid(), 10m, 25.50m)];

    [Fact]
    public void Constructor_ProveedorVacio_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() =>
            new Compra(Guid.Empty, Fecha, "Factura", "F001-123", Guid.NewGuid(), DetalleValido()));
    }

    [Fact]
    public void Constructor_SinDocumentoCompraTipo_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() =>
            new Compra(Guid.NewGuid(), Fecha, "", "F001-123", Guid.NewGuid(), DetalleValido()));
    }

    [Fact]
    public void Constructor_SinDocumentoCompraNumero_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() =>
            new Compra(Guid.NewGuid(), Fecha, "Factura", "", Guid.NewGuid(), DetalleValido()));
    }

    [Fact]
    public void Constructor_SinDetalles_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() =>
            new Compra(Guid.NewGuid(), Fecha, "Factura", "F001-123", Guid.NewGuid(), []));
    }

    [Fact]
    public void Constructor_DatosValidos_CreaLaCompraConTotalCalculado()
    {
        var productoId = Guid.NewGuid();
        var compra = new Compra(Guid.NewGuid(), Fecha, "Factura", "F001-123", Guid.NewGuid(),
            [(productoId, 10m, 25.50m), (Guid.NewGuid(), 2m, 100m)]);

        Assert.Equal(2, compra.Detalles.Count);
        Assert.Equal(455m, compra.Total);
        Assert.All(compra.Detalles, d => Assert.Equal(compra.Id, d.CompraId));
    }
}
