using ISARMIN.Domain.Entities.Ventas;
using Xunit;

namespace ISARMIN.Domain.Tests.Entities.Ventas;

public class PagoVentaTests
{
    [Fact]
    public void Constructor_MontoCero_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() => new PagoVenta(Guid.NewGuid(), Guid.NewGuid(), 0m));
    }

    [Fact]
    public void Constructor_MontoNegativo_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() => new PagoVenta(Guid.NewGuid(), Guid.NewGuid(), -5m));
    }

    [Fact]
    public void Constructor_DatosValidos_CreaElPago()
    {
        var ventaId = Guid.NewGuid();
        var medioPagoId = Guid.NewGuid();
        var pago = new PagoVenta(ventaId, medioPagoId, 100m);

        Assert.Equal(ventaId, pago.VentaId);
        Assert.Equal(medioPagoId, pago.MedioPagoId);
        Assert.Equal(100m, pago.Monto);
    }
}
