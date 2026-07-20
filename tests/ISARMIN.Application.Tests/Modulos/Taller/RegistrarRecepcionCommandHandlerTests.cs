using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Clientes;
using ISARMIN.Application.Modulos.Taller;
using ISARMIN.Application.Modulos.Taller.Commands.RegistrarRecepcion;
using ISARMIN.Domain.Entities.Taller;
using ISARMIN.Domain.Entities.Terceros;
using Moq;
using Xunit;

namespace ISARMIN.Application.Tests.Modulos.Taller;

public class RegistrarRecepcionCommandHandlerTests
{
    private readonly Mock<IOrdenTrabajoRepository> _ordenTrabajoRepository = new();
    private readonly Mock<IClienteRepository> _clienteRepository = new();
    private readonly Mock<IFechaHoraProvider> _fechaHoraProvider = new();
    private readonly RegistrarRecepcionCommandValidator _validator = new();

    public RegistrarRecepcionCommandHandlerTests()
    {
        _fechaHoraProvider.Setup(f => f.UtcAhora).Returns(new DateTime(2026, 7, 20, 9, 0, 0, DateTimeKind.Utc));
    }

    private RegistrarRecepcionCommandHandler CrearHandler() =>
        new(_ordenTrabajoRepository.Object, _clienteRepository.Object, _fechaHoraProvider.Object, _validator);

    [Fact]
    public async Task ManejarAsync_ClienteExistente_RegistraLaOt()
    {
        var clienteId = Guid.NewGuid();
        _clienteRepository
            .Setup(r => r.ObtenerPorIdAsync(clienteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Cliente("Juan Pérez", "999888777", null, null, null));

        var handler = CrearHandler();
        var resultado = await handler.ManejarAsync(
            new RegistrarRecepcionCommand(clienteId, "Taladro Bosch", "No enciende", Guid.NewGuid()));

        Assert.Equal("Recibido", resultado.Estado);
        _ordenTrabajoRepository.Verify(r => r.Agregar(It.IsAny<OrdenTrabajo>()), Times.Once);
    }

    [Fact]
    public async Task ManejarAsync_ClienteInexistente_LanzaExcepcion()
    {
        _clienteRepository.Setup(r => r.ObtenerPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Cliente?)null);

        var handler = CrearHandler();

        await Assert.ThrowsAsync<ClienteNoEncontradoException>(() =>
            handler.ManejarAsync(new RegistrarRecepcionCommand(Guid.NewGuid(), "Taladro", "No enciende", Guid.NewGuid())));
    }
}
