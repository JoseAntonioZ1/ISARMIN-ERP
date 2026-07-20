using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Caja;
using ISARMIN.Application.Modulos.Caja.Commands.CerrarCaja;
using Moq;
using Xunit;
using CajaEntity = ISARMIN.Domain.Entities.Caja.Caja;

namespace ISARMIN.Application.Tests.Modulos.Caja;

public class CerrarCajaCommandHandlerTests
{
    private readonly Mock<ICajaRepository> _cajaRepository = new();
    private readonly Mock<IMovimientoCajaRepository> _movimientoCajaRepository = new();
    private readonly Mock<IFechaHoraProvider> _fechaHoraProvider = new();
    private readonly CerrarCajaCommandValidator _validator = new();

    public CerrarCajaCommandHandlerTests()
    {
        _fechaHoraProvider.Setup(f => f.UtcAhora).Returns(new DateTime(2026, 7, 20, 18, 0, 0, DateTimeKind.Utc));
    }

    private CerrarCajaCommandHandler CrearHandler() =>
        new(_cajaRepository.Object, _movimientoCajaRepository.Object, _fechaHoraProvider.Object, _validator);

    [Fact]
    public async Task ManejarAsync_CajaAbierta_CalculaMontoTeoricoYCierra()
    {
        var caja = new CajaEntity(100m, Guid.NewGuid(), DateTime.UtcNow);
        _cajaRepository.Setup(r => r.ObtenerAbiertaAsync(It.IsAny<CancellationToken>())).ReturnsAsync(caja);
        _movimientoCajaRepository
            .Setup(r => r.ObtenerTotalesAsync(caja.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((500m, 50m));

        var handler = CrearHandler();
        var resultado = await handler.ManejarAsync(new CerrarCajaCommand(540m));

        // Teórico = 100 (apertura) + 500 (ingresos) - 50 (egresos) = 550; diferencia = 540 - 550 = -10.
        Assert.Equal("Cerrada", resultado.Estado);
        Assert.Equal(550m, resultado.MontoTeoricoCierre);
        Assert.Equal(540m, resultado.MontoFisicoDeclarado);
        Assert.Equal(-10m, resultado.Diferencia);
        _cajaRepository.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ManejarAsync_SinCajaAbierta_LanzaExcepcion()
    {
        _cajaRepository.Setup(r => r.ObtenerAbiertaAsync(It.IsAny<CancellationToken>())).ReturnsAsync((CajaEntity?)null);

        var handler = CrearHandler();

        await Assert.ThrowsAsync<CajaNoAbiertaException>(() => handler.ManejarAsync(new CerrarCajaCommand(100m)));
    }
}
