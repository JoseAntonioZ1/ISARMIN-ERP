using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Caja;
using ISARMIN.Application.Modulos.Caja.Commands.AbrirCaja;
using Moq;
using Xunit;
using CajaEntity = ISARMIN.Domain.Entities.Caja.Caja;

namespace ISARMIN.Application.Tests.Modulos.Caja;

public class AbrirCajaCommandHandlerTests
{
    private readonly Mock<ICajaRepository> _cajaRepository = new();
    private readonly Mock<IFechaHoraProvider> _fechaHoraProvider = new();
    private readonly AbrirCajaCommandValidator _validator = new();

    public AbrirCajaCommandHandlerTests()
    {
        _fechaHoraProvider.Setup(f => f.UtcAhora).Returns(new DateTime(2026, 7, 20, 8, 0, 0, DateTimeKind.Utc));
    }

    private AbrirCajaCommandHandler CrearHandler() => new(_cajaRepository.Object, _fechaHoraProvider.Object, _validator);

    [Fact]
    public async Task ManejarAsync_SinCajaAbierta_AbreLaCaja()
    {
        _cajaRepository.Setup(r => r.ObtenerAbiertaAsync(It.IsAny<CancellationToken>())).ReturnsAsync((CajaEntity?)null);

        var handler = CrearHandler();
        var resultado = await handler.ManejarAsync(new AbrirCajaCommand(100m, Guid.NewGuid()));

        Assert.Equal("Abierta", resultado.Estado);
        Assert.Equal(100m, resultado.MontoApertura);
        _cajaRepository.Verify(r => r.Agregar(It.IsAny<CajaEntity>()), Times.Once);
        _cajaRepository.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ManejarAsync_YaHayCajaAbierta_LanzaExcepcion()
    {
        _cajaRepository
            .Setup(r => r.ObtenerAbiertaAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CajaEntity(50m, Guid.NewGuid(), DateTime.UtcNow));

        var handler = CrearHandler();

        await Assert.ThrowsAsync<CajaYaAbiertaException>(() => handler.ManejarAsync(new AbrirCajaCommand(100m, Guid.NewGuid())));
    }

    [Fact]
    public async Task ManejarAsync_MontoNegativo_FallaValidacion()
    {
        var handler = CrearHandler();

        await Assert.ThrowsAsync<ValidationException>(() => handler.ManejarAsync(new AbrirCajaCommand(-1m, Guid.NewGuid())));
    }
}
