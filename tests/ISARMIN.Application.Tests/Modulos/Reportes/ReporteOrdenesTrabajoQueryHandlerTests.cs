using ISARMIN.Application.Modulos.Reportes;
using ISARMIN.Application.Modulos.Reportes.Queries.ReporteOrdenesTrabajo;
using ISARMIN.Domain.Entities.Taller;
using ISARMIN.Domain.Enums;
using Moq;
using Xunit;

namespace ISARMIN.Application.Tests.Modulos.Reportes;

public class ReporteOrdenesTrabajoQueryHandlerTests
{
    private readonly Mock<IReporteRepository> _reporteRepository = new();

    private ReporteOrdenesTrabajoQueryHandler CrearHandler() => new(_reporteRepository.Object);

    [Fact]
    public async Task ManejarAsync_DevuelveCantidadYListadoDeLasOrdenesFiltradas()
    {
        var ot1 = new OrdenTrabajo(Guid.NewGuid(), "Taladro Bosch", "No enciende", Guid.NewGuid(), DateTime.UtcNow);
        var ot2 = new OrdenTrabajo(Guid.NewGuid(), "Sierra", "Cable roto", Guid.NewGuid(), DateTime.UtcNow);

        _reporteRepository.Setup(r => r.ObtenerOrdenesTrabajoAsync(
                EstadoOrdenTrabajo.Recibido, It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([ot1, ot2]);

        var handler = CrearHandler();
        var resultado = await handler.ManejarAsync(new ReporteOrdenesTrabajoQuery(EstadoOrdenTrabajo.Recibido, null, null));

        Assert.Equal(2, resultado.CantidadTotal);
        Assert.Equal(2, resultado.Ordenes.Count);
    }
}
