using FluentValidation;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Configuracion;
using ISARMIN.Application.Modulos.ServiciosCampo;
using ISARMIN.Application.Modulos.ServiciosCampo.Commands.CobrarServicioCampo;
using ISARMIN.Application.Modulos.Usuarios;
using ISARMIN.Domain.Entities.Configuracion;
using ISARMIN.Domain.Entities.Identidad;
using ISARMIN.Domain.Entities.ServiciosCampo;
using Moq;
using Xunit;

namespace ISARMIN.Application.Tests.Modulos.ServiciosCampo;

public class CobrarServicioCampoCommandHandlerTests
{
    private readonly Mock<IServicioCampoRepository> _servicioCampoRepository = new();
    private readonly Mock<IMedioPagoRepository> _medioPagoRepository = new();
    private readonly Mock<IUsuarioRepository> _usuarioRepository = new();
    private readonly CobrarServicioCampoCommandValidator _validator = new();

    private CobrarServicioCampoCommandHandler CrearHandler() => new(
        _servicioCampoRepository.Object, _medioPagoRepository.Object, _usuarioRepository.Object, _validator);

    private static ServicioCampo CrearServicioCerrado()
    {
        var servicio = new ServicioCampo(Guid.NewGuid(), "Instalación de cámaras", null, DateTime.UtcNow);
        servicio.Cerrar([], "Completado", null, Guid.NewGuid(), DateTime.UtcNow);
        return servicio;
    }

    [Fact]
    public async Task ManejarAsync_PagoCompleto_RegistraElCobro()
    {
        var servicio = CrearServicioCerrado();
        var medioPago = new MedioPago("Efectivo");
        _servicioCampoRepository.Setup(r => r.ObtenerPorIdAsync(servicio.Id, It.IsAny<CancellationToken>())).ReturnsAsync(servicio);
        _medioPagoRepository.Setup(r => r.ObtenerPorIdAsync(medioPago.Id, It.IsAny<CancellationToken>())).ReturnsAsync(medioPago);

        var handler = CrearHandler();
        var comando = new CobrarServicioCampoCommand(servicio.Id, medioPago.Id, 250m, null, null);
        var resultado = await handler.ManejarAsync(comando);

        Assert.Equal(250m, resultado.MontoPagado);
        Assert.Null(resultado.SaldoPendiente);
        _servicioCampoRepository.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ManejarAsync_SaldoPendienteSinUsuarioAutorizante_FallaValidacion()
    {
        var servicio = CrearServicioCerrado();
        _servicioCampoRepository.Setup(r => r.ObtenerPorIdAsync(servicio.Id, It.IsAny<CancellationToken>())).ReturnsAsync(servicio);

        var handler = CrearHandler();
        var comando = new CobrarServicioCampoCommand(servicio.Id, Guid.NewGuid(), 150m, 100m, null);

        await Assert.ThrowsAsync<ValidationException>(() => handler.ManejarAsync(comando));
    }

    [Fact]
    public async Task ManejarAsync_SaldoPendienteConUsuarioAutorizanteInexistente_LanzaExcepcion()
    {
        var servicio = CrearServicioCerrado();
        var medioPago = new MedioPago("Efectivo");
        _servicioCampoRepository.Setup(r => r.ObtenerPorIdAsync(servicio.Id, It.IsAny<CancellationToken>())).ReturnsAsync(servicio);
        _medioPagoRepository.Setup(r => r.ObtenerPorIdAsync(medioPago.Id, It.IsAny<CancellationToken>())).ReturnsAsync(medioPago);
        _usuarioRepository.Setup(r => r.ObtenerPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Usuario?)null);

        var handler = CrearHandler();
        var comando = new CobrarServicioCampoCommand(servicio.Id, medioPago.Id, 150m, 100m, Guid.NewGuid());

        await Assert.ThrowsAsync<UsuarioNoEncontradoException>(() => handler.ManejarAsync(comando));
    }

    [Fact]
    public async Task ManejarAsync_MedioPagoInexistente_LanzaExcepcion()
    {
        var servicio = CrearServicioCerrado();
        _servicioCampoRepository.Setup(r => r.ObtenerPorIdAsync(servicio.Id, It.IsAny<CancellationToken>())).ReturnsAsync(servicio);
        _medioPagoRepository.Setup(r => r.ObtenerPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((MedioPago?)null);

        var handler = CrearHandler();
        var comando = new CobrarServicioCampoCommand(servicio.Id, Guid.NewGuid(), 250m, null, null);

        await Assert.ThrowsAsync<MedioPagoNoEncontradoException>(() => handler.ManejarAsync(comando));
    }

    [Fact]
    public async Task ManejarAsync_ServicioNoCerrado_LanzaExcepcion()
    {
        var servicio = new ServicioCampo(Guid.NewGuid(), "Instalación de cámaras", null, DateTime.UtcNow);
        _servicioCampoRepository.Setup(r => r.ObtenerPorIdAsync(servicio.Id, It.IsAny<CancellationToken>())).ReturnsAsync(servicio);

        var handler = CrearHandler();
        var comando = new CobrarServicioCampoCommand(servicio.Id, Guid.NewGuid(), 250m, null, null);

        await Assert.ThrowsAsync<EstadoServicioCampoInvalidoException>(() => handler.ManejarAsync(comando));
    }

    [Fact]
    public async Task ManejarAsync_ServicioInexistente_LanzaExcepcion()
    {
        _servicioCampoRepository.Setup(r => r.ObtenerPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((ServicioCampo?)null);

        var handler = CrearHandler();
        var comando = new CobrarServicioCampoCommand(Guid.NewGuid(), Guid.NewGuid(), 250m, null, null);

        await Assert.ThrowsAsync<ServicioCampoNoEncontradoException>(() => handler.ManejarAsync(comando));
    }
}
