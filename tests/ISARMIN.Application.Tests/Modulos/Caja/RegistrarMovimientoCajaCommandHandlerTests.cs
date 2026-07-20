using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Caja;
using ISARMIN.Application.Modulos.Caja.Commands.RegistrarMovimientoCaja;
using ISARMIN.Domain.Entities.Caja;
using ISARMIN.Domain.Enums;
using Moq;
using Xunit;
using CajaEntity = ISARMIN.Domain.Entities.Caja.Caja;

namespace ISARMIN.Application.Tests.Modulos.Caja;

public class RegistrarMovimientoCajaCommandHandlerTests
{
    private readonly Mock<ICajaRepository> _cajaRepository = new();
    private readonly Mock<IMovimientoCajaRepository> _movimientoCajaRepository = new();
    private readonly Mock<IFechaHoraProvider> _fechaHoraProvider = new();
    private readonly RegistrarMovimientoCajaCommandValidator _validator = new();

    public RegistrarMovimientoCajaCommandHandlerTests()
    {
        _fechaHoraProvider.Setup(f => f.UtcAhora).Returns(new DateTime(2026, 7, 20, 10, 0, 0, DateTimeKind.Utc));
    }

    private RegistrarMovimientoCajaCommandHandler CrearHandler() =>
        new(_cajaRepository.Object, _movimientoCajaRepository.Object, _fechaHoraProvider.Object, _validator);

    [Fact]
    public async Task ManejarAsync_CajaAbierta_RegistraElMovimiento()
    {
        var caja = new CajaEntity(100m, Guid.NewGuid(), DateTime.UtcNow);
        _cajaRepository.Setup(r => r.ObtenerAbiertaAsync(It.IsAny<CancellationToken>())).ReturnsAsync(caja);

        var handler = CrearHandler();
        var resultado = await handler.ManejarAsync(
            new RegistrarMovimientoCajaCommand(ConceptoMovimientoCaja.GastoOperativo, 30m, "Pago de luz", Guid.NewGuid()));

        Assert.Equal("Egreso", resultado.Tipo);
        Assert.Equal(caja.Id, resultado.CajaId);
        _movimientoCajaRepository.Verify(r => r.Agregar(It.IsAny<MovimientoCaja>()), Times.Once);
        _movimientoCajaRepository.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ManejarAsync_SinCajaAbierta_LanzaExcepcion()
    {
        _cajaRepository.Setup(r => r.ObtenerAbiertaAsync(It.IsAny<CancellationToken>())).ReturnsAsync((CajaEntity?)null);

        var handler = CrearHandler();

        await Assert.ThrowsAsync<CajaNoAbiertaException>(() =>
            handler.ManejarAsync(new RegistrarMovimientoCajaCommand(ConceptoMovimientoCaja.GastoOperativo, 30m, null, Guid.NewGuid())));
    }
}
