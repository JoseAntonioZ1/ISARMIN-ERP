using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Taller;
using ISARMIN.Application.Modulos.Taller.Commands.EntregarEquipo;
using ISARMIN.Application.Modulos.Usuarios;
using ISARMIN.Domain.Entities.Identidad;
using ISARMIN.Domain.Entities.Taller;
using ISARMIN.Domain.Enums;
using Moq;
using Xunit;

namespace ISARMIN.Application.Tests.Modulos.Taller;

public class EntregarEquipoCommandHandlerTests
{
    private readonly Mock<IOrdenTrabajoRepository> _ordenTrabajoRepository = new();
    private readonly Mock<IUsuarioRepository> _usuarioRepository = new();
    private readonly Mock<IFechaHoraProvider> _fechaHoraProvider = new();
    private readonly EntregarEquipoCommandValidator _validator = new();

    public EntregarEquipoCommandHandlerTests()
    {
        _fechaHoraProvider.Setup(f => f.UtcAhora).Returns(new DateTime(2026, 7, 20, 15, 0, 0, DateTimeKind.Utc));
    }

    private EntregarEquipoCommandHandler CrearHandler() =>
        new(_ordenTrabajoRepository.Object, _usuarioRepository.Object, _fechaHoraProvider.Object, _validator);

    private static OrdenTrabajo CrearOtListaParaEntrega()
    {
        var ot = new OrdenTrabajo(Guid.NewGuid(), "Taladro Bosch", "No enciende", Guid.NewGuid(), DateTime.UtcNow);
        ot.RegistrarDiagnostico("Motor quemado", Guid.NewGuid(), DateTime.UtcNow);
        ot.GenerarCotizacion(150m, DateTime.UtcNow);
        ot.RegistrarDecisionCliente(DecisionCliente.Aprobada, null, null);
        ot.RegistrarReparacion([], "OK");
        return ot;
    }

    [Fact]
    public async Task ManejarAsync_PagoCompleto_EntregaLaOt()
    {
        var ot = CrearOtListaParaEntrega();
        _ordenTrabajoRepository.Setup(r => r.ObtenerPorIdAsync(ot.Id, It.IsAny<CancellationToken>())).ReturnsAsync(ot);

        var handler = CrearHandler();
        var comando = new EntregarEquipoCommand(ot.Id, EstadoPagoOrdenTrabajo.CompletoAlMomento, 150m, null, null, Guid.NewGuid());
        var resultado = await handler.ManejarAsync(comando);

        Assert.Equal("Entregado", resultado.Estado);
    }

    [Fact]
    public async Task ManejarAsync_SaldoPendienteSinUsuarioAutorizante_FallaValidacion()
    {
        var ot = CrearOtListaParaEntrega();
        _ordenTrabajoRepository.Setup(r => r.ObtenerPorIdAsync(ot.Id, It.IsAny<CancellationToken>())).ReturnsAsync(ot);

        var handler = CrearHandler();
        var comando = new EntregarEquipoCommand(ot.Id, EstadoPagoOrdenTrabajo.SaldoPendiente, 50m, 100m, null, Guid.NewGuid());

        await Assert.ThrowsAsync<ValidationException>(() => handler.ManejarAsync(comando));
    }

    [Fact]
    public async Task ManejarAsync_SaldoPendienteConUsuarioAutorizanteInexistente_LanzaExcepcion()
    {
        var ot = CrearOtListaParaEntrega();
        _ordenTrabajoRepository.Setup(r => r.ObtenerPorIdAsync(ot.Id, It.IsAny<CancellationToken>())).ReturnsAsync(ot);
        _usuarioRepository.Setup(r => r.ObtenerPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Usuario?)null);

        var handler = CrearHandler();
        var comando = new EntregarEquipoCommand(ot.Id, EstadoPagoOrdenTrabajo.SaldoPendiente, 50m, 100m, Guid.NewGuid(), Guid.NewGuid());

        await Assert.ThrowsAsync<UsuarioNoEncontradoException>(() => handler.ManejarAsync(comando));
    }

    [Fact]
    public async Task ManejarAsync_OtNoListaParaEntrega_LanzaExcepcion()
    {
        var ot = new OrdenTrabajo(Guid.NewGuid(), "Taladro Bosch", "No enciende", Guid.NewGuid(), DateTime.UtcNow);
        _ordenTrabajoRepository.Setup(r => r.ObtenerPorIdAsync(ot.Id, It.IsAny<CancellationToken>())).ReturnsAsync(ot);

        var handler = CrearHandler();
        var comando = new EntregarEquipoCommand(ot.Id, EstadoPagoOrdenTrabajo.CompletoAlMomento, 0m, null, null, Guid.NewGuid());

        await Assert.ThrowsAsync<EstadoOrdenTrabajoInvalidoException>(() => handler.ManejarAsync(comando));
    }
}
