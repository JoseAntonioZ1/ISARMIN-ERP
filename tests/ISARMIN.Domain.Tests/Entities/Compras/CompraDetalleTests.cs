using ISARMIN.Domain.Entities.Compras;
using Xunit;

namespace ISARMIN.Domain.Tests.Entities.Compras;

public class CompraDetalleTests
{
    [Fact]
    public void Constructor_CantidadCero_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() => new CompraDetalle(Guid.NewGuid(), Guid.NewGuid(), 0m, 10m));
    }

    [Fact]
    public void Constructor_CostoNegativo_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() => new CompraDetalle(Guid.NewGuid(), Guid.NewGuid(), 5m, -1m));
    }

    [Fact]
    public void Constructor_DatosValidos_CreaElDetalle()
    {
        var compraId = Guid.NewGuid();
        var productoId = Guid.NewGuid();

        var detalle = new CompraDetalle(compraId, productoId, 10m, 25.50m);

        Assert.Equal(compraId, detalle.CompraId);
        Assert.Equal(productoId, detalle.ProductoId);
        Assert.Equal(10m, detalle.Cantidad);
        Assert.Equal(25.50m, detalle.CostoUnitario);
    }
}
