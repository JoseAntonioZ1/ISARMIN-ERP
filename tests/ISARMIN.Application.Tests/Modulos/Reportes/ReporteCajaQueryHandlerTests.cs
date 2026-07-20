using ISARMIN.Application.Modulos.Reportes;
using ISARMIN.Application.Modulos.Reportes.Queries.ReporteCaja;
using ISARMIN.Domain.Entities.Caja;
using ISARMIN.Domain.Enums;
using Moq;
using Xunit;
using CajaEntity = ISARMIN.Domain.Entities.Caja.Caja;

namespace ISARMIN.Application.Tests.Modulos.Reportes;

public class ReporteCajaQueryHandlerTests
{
    private readonly Mock<IReporteRepository> _reporteRepository = new();

    private ReporteCajaQueryHandler CrearHandler() => new(_reporteRepository.Object);

    [Fact]
    public async Task ManejarAsync_CalculaTotalesDeIngresosYEgresos()
    {
        var caja = new CajaEntity(200m, Guid.NewGuid(), DateTime.UtcNow);
        var ingreso = MovimientoCaja.Registrar(caja.Id, ConceptoMovimientoCaja.AporteCapital, 500m, null, Guid.NewGuid(), DateTime.UtcNow);
        var egreso = MovimientoCaja.Registrar(caja.Id, ConceptoMovimientoCaja.GastoOperativo, 120m, "Compra de insumos", Guid.NewGuid(), DateTime.UtcNow);

        _reporteRepository.Setup(r => r.ObtenerCajaAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(([caja], new[] { ingreso, egreso }));

        var handler = CrearHandler();
        var resultado = await handler.ManejarAsync(new ReporteCajaQuery(null, null));

        Assert.Equal(500m, resultado.TotalIngresos);
        Assert.Equal(120m, resultado.TotalEgresos);
        Assert.Equal(380m, resultado.SaldoNeto);
        Assert.Single(resultado.Cajas);
        Assert.Equal(2, resultado.Movimientos.Count);
    }
}
