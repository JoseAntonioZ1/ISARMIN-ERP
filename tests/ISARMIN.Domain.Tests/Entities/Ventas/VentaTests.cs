using ISARMIN.Domain.Entities.Ventas;
using ISARMIN.Domain.Enums;
using Xunit;

namespace ISARMIN.Domain.Tests.Entities.Ventas;

public class VentaTests
{
    private static readonly DateTime Fecha = new(2026, 7, 20, 10, 0, 0, DateTimeKind.Utc);

    private static (Guid ProductoId, decimal Cantidad, decimal PrecioUnitario)[] DetalleValido() =>
        [(Guid.NewGuid(), 2m, 50m)];

    [Fact]
    public void Constructor_UsuarioVacio_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() =>
            new Venta(null, TipoComprobante.Ticket, Guid.Empty, Fecha, DetalleValido(), [], null));
    }

    [Fact]
    public void Constructor_SinDetalles_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() =>
            new Venta(null, TipoComprobante.Ticket, Guid.NewGuid(), Fecha, [], [], null));
    }

    [Fact]
    public void Constructor_PagoCompleto_QuedaPagadaSinSaldoPendiente()
    {
        var venta = new Venta(null, TipoComprobante.Ticket, Guid.NewGuid(), Fecha, DetalleValido(), [(Guid.NewGuid(), 100m)], null);

        Assert.Equal(100m, venta.Total);
        Assert.Equal(EstadoVenta.Pagada, venta.Estado);
        Assert.Null(venta.SaldoPendiente);
        Assert.Single(venta.Detalles);
        Assert.Single(venta.Pagos);
    }

    [Fact]
    public void Constructor_SinPagos_ConAutorizacion_QuedaRegistradaConSaldoPendienteTotal()
    {
        var autorizanteId = Guid.NewGuid();
        var venta = new Venta(null, TipoComprobante.Ticket, Guid.NewGuid(), Fecha, DetalleValido(), [], autorizanteId);

        Assert.Equal(EstadoVenta.Registrada, venta.Estado);
        Assert.Equal(100m, venta.SaldoPendiente);
        Assert.Equal(autorizanteId, venta.UsuarioAutorizoSaldoId);
    }

    [Fact]
    public void Constructor_SaldoPendienteSinAutorizacion_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() =>
            new Venta(null, TipoComprobante.Ticket, Guid.NewGuid(), Fecha, DetalleValido(), [(Guid.NewGuid(), 40m)], null));
    }

    [Fact]
    public void Constructor_MontoPagadoExcedeElTotal_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() =>
            new Venta(null, TipoComprobante.Ticket, Guid.NewGuid(), Fecha, DetalleValido(), [(Guid.NewGuid(), 500m)], null));
    }

    [Fact]
    public void Anular_VentaRegistrada_QuedaAnuladaConMotivo()
    {
        var venta = new Venta(null, TipoComprobante.Ticket, Guid.NewGuid(), Fecha, DetalleValido(), [(Guid.NewGuid(), 100m)], null);
        var usuarioAnuloId = Guid.NewGuid();

        venta.Anular("Cliente se arrepintió", usuarioAnuloId, Fecha.AddMinutes(5));

        Assert.Equal(EstadoVenta.Anulada, venta.Estado);
        Assert.Equal("Cliente se arrepintió", venta.MotivoAnulacion);
        Assert.Equal(usuarioAnuloId, venta.UsuarioAnuloId);
    }

    [Fact]
    public void Anular_SinMotivo_LanzaExcepcion()
    {
        var venta = new Venta(null, TipoComprobante.Ticket, Guid.NewGuid(), Fecha, DetalleValido(), [(Guid.NewGuid(), 100m)], null);

        Assert.Throws<ArgumentException>(() => venta.Anular("  ", Guid.NewGuid(), Fecha));
    }

    [Fact]
    public void Anular_VentaYaAnulada_LanzaExcepcion()
    {
        var venta = new Venta(null, TipoComprobante.Ticket, Guid.NewGuid(), Fecha, DetalleValido(), [(Guid.NewGuid(), 100m)], null);
        venta.Anular("Error de registro", Guid.NewGuid(), Fecha);

        Assert.Throws<InvalidOperationException>(() => venta.Anular("Otro motivo", Guid.NewGuid(), Fecha));
    }
}
