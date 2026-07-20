using ISARMIN.Application.Modulos.Reportes;
using ISARMIN.Application.Modulos.Reportes.Queries.ReporteVentas;
using ISARMIN.Domain.Entities.Ventas;
using ISARMIN.Domain.Enums;
using Moq;
using Xunit;

namespace ISARMIN.Application.Tests.Modulos.Reportes;

public class ReporteVentasQueryHandlerTests
{
    private readonly Mock<IReporteRepository> _reporteRepository = new();

    private ReporteVentasQueryHandler CrearHandler() => new(_reporteRepository.Object);

    private static Venta CrearVenta(decimal total, bool anulada = false)
    {
        var venta = new Venta(null, TipoComprobante.Ticket, Guid.NewGuid(), DateTime.UtcNow,
            [(Guid.NewGuid(), 1m, total)], [(Guid.NewGuid(), total)], null);
        if (anulada)
        {
            venta.Anular("Motivo de prueba", Guid.NewGuid(), DateTime.UtcNow);
        }
        return venta;
    }

    [Fact]
    public async Task ManejarAsync_ExcluyeVentasAnuladasDelMontoTotal()
    {
        var ventas = new[] { CrearVenta(100m), CrearVenta(50m), CrearVenta(200m, anulada: true) };
        _reporteRepository.Setup(r => r.ObtenerVentasAsync(null, null, It.IsAny<CancellationToken>())).ReturnsAsync(ventas);

        var handler = CrearHandler();
        var resultado = await handler.ManejarAsync(new ReporteVentasQuery(null, null));

        Assert.Equal(3, resultado.CantidadVentas);
        Assert.Equal(150m, resultado.MontoTotal);
        Assert.Equal(3, resultado.Ventas.Count);
    }

    [Fact]
    public async Task ManejarAsync_SinVentas_DevuelveTotalesEnCero()
    {
        _reporteRepository.Setup(r => r.ObtenerVentasAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var handler = CrearHandler();
        var resultado = await handler.ManejarAsync(new ReporteVentasQuery(null, null));

        Assert.Equal(0, resultado.CantidadVentas);
        Assert.Equal(0m, resultado.MontoTotal);
    }
}
