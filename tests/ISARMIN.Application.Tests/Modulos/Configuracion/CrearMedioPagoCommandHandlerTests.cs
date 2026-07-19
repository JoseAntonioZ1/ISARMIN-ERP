using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Configuracion;
using ISARMIN.Application.Modulos.Configuracion.Commands.CrearMedioPago;
using ISARMIN.Domain.Entities.Configuracion;
using Moq;
using Xunit;

namespace ISARMIN.Application.Tests.Modulos.Configuracion;

public class CrearMedioPagoCommandHandlerTests
{
    private readonly Mock<IMedioPagoRepository> _medioPagoRepository = new();
    private readonly CrearMedioPagoCommandValidator _validator = new();

    private CrearMedioPagoCommandHandler CrearHandler() => new(_medioPagoRepository.Object, _validator);

    [Fact]
    public async Task ManejarAsync_NombreDisponible_CreaElMedioPago()
    {
        _medioPagoRepository.Setup(r => r.ObtenerPorNombreAsync("Tarjeta", It.IsAny<CancellationToken>()))
            .ReturnsAsync((MedioPago?)null);

        var handler = CrearHandler();
        var resultado = await handler.ManejarAsync(new CrearMedioPagoCommand("Tarjeta"));

        Assert.Equal("Tarjeta", resultado.Nombre);
        Assert.True(resultado.Activo);
        _medioPagoRepository.Verify(r => r.Agregar(It.IsAny<MedioPago>()), Times.Once);
    }

    [Fact]
    public async Task ManejarAsync_NombreYaExiste_LanzaExcepcion()
    {
        _medioPagoRepository.Setup(r => r.ObtenerPorNombreAsync("Efectivo", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MedioPago("Efectivo"));

        var handler = CrearHandler();

        await Assert.ThrowsAsync<NombreMedioPagoDuplicadoException>(() =>
            handler.ManejarAsync(new CrearMedioPagoCommand("Efectivo")));
    }
}
