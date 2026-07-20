using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.ServiciosCampo;
using ISARMIN.Application.Modulos.ServiciosCampo.Commands.CotizarServicioCampo;
using ISARMIN.Domain.Entities.ServiciosCampo;
using Moq;
using Xunit;

namespace ISARMIN.Application.Tests.Modulos.ServiciosCampo;

public class CotizarServicioCampoCommandHandlerTests
{
    private readonly Mock<IServicioCampoRepository> _servicioCampoRepository = new();
    private readonly CotizarServicioCampoCommandValidator _validator = new();

    private CotizarServicioCampoCommandHandler CrearHandler() => new(_servicioCampoRepository.Object, _validator);

    private static ServicioCampo CrearServicioSolicitado() =>
        new(Guid.NewGuid(), "Instalación de cámaras", null, DateTime.UtcNow);

    [Fact]
    public async Task ManejarAsync_DatosValidos_AsignaMontoEstimado()
    {
        var servicio = CrearServicioSolicitado();
        _servicioCampoRepository.Setup(r => r.ObtenerPorIdAsync(servicio.Id, It.IsAny<CancellationToken>())).ReturnsAsync(servicio);

        var handler = CrearHandler();
        var comando = new CotizarServicioCampoCommand(servicio.Id, 250m);
        var resultado = await handler.ManejarAsync(comando);

        Assert.Equal(250m, resultado.MontoEstimado);
        _servicioCampoRepository.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ManejarAsync_ServicioYaCerrado_LanzaExcepcion()
    {
        var servicio = CrearServicioSolicitado();
        servicio.Cerrar([], "Completado", null, Guid.NewGuid(), DateTime.UtcNow);
        _servicioCampoRepository.Setup(r => r.ObtenerPorIdAsync(servicio.Id, It.IsAny<CancellationToken>())).ReturnsAsync(servicio);

        var handler = CrearHandler();
        var comando = new CotizarServicioCampoCommand(servicio.Id, 250m);

        await Assert.ThrowsAsync<EstadoServicioCampoInvalidoException>(() => handler.ManejarAsync(comando));
    }

    [Fact]
    public async Task ManejarAsync_ServicioInexistente_LanzaExcepcion()
    {
        _servicioCampoRepository.Setup(r => r.ObtenerPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((ServicioCampo?)null);

        var handler = CrearHandler();
        var comando = new CotizarServicioCampoCommand(Guid.NewGuid(), 250m);

        await Assert.ThrowsAsync<ServicioCampoNoEncontradoException>(() => handler.ManejarAsync(comando));
    }
}
