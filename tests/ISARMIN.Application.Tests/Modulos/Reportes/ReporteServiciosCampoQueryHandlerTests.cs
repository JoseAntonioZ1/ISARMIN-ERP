using ISARMIN.Application.Modulos.Reportes;
using ISARMIN.Application.Modulos.Reportes.Queries.ReporteServiciosCampo;
using ISARMIN.Domain.Entities.ServiciosCampo;
using Moq;
using Xunit;

namespace ISARMIN.Application.Tests.Modulos.Reportes;

public class ReporteServiciosCampoQueryHandlerTests
{
    private readonly Mock<IReporteRepository> _reporteRepository = new();

    private ReporteServiciosCampoQueryHandler CrearHandler() => new(_reporteRepository.Object);

    [Fact]
    public async Task ManejarAsync_DevuelveCantidadYListadoDeLosServiciosFiltrados()
    {
        var tecnicoId = Guid.NewGuid();
        var servicio = new ServicioCampo(Guid.NewGuid(), "Instalación de cámaras", tecnicoId, DateTime.UtcNow);

        _reporteRepository.Setup(r => r.ObtenerServiciosCampoAsync(
                tecnicoId, It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([servicio]);

        var handler = CrearHandler();
        var resultado = await handler.ManejarAsync(new ReporteServiciosCampoQuery(tecnicoId, null, null));

        Assert.Equal(1, resultado.CantidadTotal);
        Assert.Single(resultado.Servicios);
    }
}
