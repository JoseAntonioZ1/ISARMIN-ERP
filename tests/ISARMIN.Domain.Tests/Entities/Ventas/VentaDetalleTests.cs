using ISARMIN.Domain.Entities.Ventas;
using Xunit;

namespace ISARMIN.Domain.Tests.Entities.Ventas;

public class VentaDetalleTests
{
    [Fact]
    public void Constructor_CantidadCero_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() => new VentaDetalle(Guid.NewGuid(), Guid.NewGuid(), 0m, 10m));
    }

    [Fact]
    public void Constructor_PrecioUnitarioNegativo_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() => new VentaDetalle(Guid.NewGuid(), Guid.NewGuid(), 1m, -1m));
    }

    [Fact]
    public void Constructor_DatosValidos_CreaElDetalle()
    {
        var ventaId = Guid.NewGuid();
        var productoId = Guid.NewGuid();
        var detalle = new VentaDetalle(ventaId, productoId, 3m, 15.50m);

        Assert.Equal(ventaId, detalle.VentaId);
        Assert.Equal(productoId, detalle.ProductoId);
        Assert.Equal(3m, detalle.Cantidad);
        Assert.Equal(15.50m, detalle.PrecioUnitario);
    }
}
