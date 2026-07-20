using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Clientes;
using ISARMIN.Application.Modulos.ServiciosCampo;
using ISARMIN.Application.Modulos.ServiciosCampo.Commands.SolicitarServicioCampo;
using ISARMIN.Application.Modulos.Usuarios;
using ISARMIN.Domain.Entities.Identidad;
using ISARMIN.Domain.Entities.Terceros;
using Moq;
using Xunit;

namespace ISARMIN.Application.Tests.Modulos.ServiciosCampo;

public class SolicitarServicioCampoCommandHandlerTests
{
    private readonly Mock<IServicioCampoRepository> _servicioCampoRepository = new();
    private readonly Mock<IClienteRepository> _clienteRepository = new();
    private readonly Mock<IUsuarioRepository> _usuarioRepository = new();
    private readonly Mock<IFechaHoraProvider> _fechaHoraProvider = new();
    private readonly SolicitarServicioCampoCommandValidator _validator = new();

    public SolicitarServicioCampoCommandHandlerTests()
    {
        _fechaHoraProvider.Setup(f => f.UtcAhora).Returns(new DateTime(2026, 7, 19, 9, 0, 0, DateTimeKind.Utc));
    }

    private SolicitarServicioCampoCommandHandler CrearHandler() => new(
        _servicioCampoRepository.Object, _clienteRepository.Object, _usuarioRepository.Object, _fechaHoraProvider.Object, _validator);

    private static Cliente CrearCliente() => new("Juan Pérez", "987654321", null, null, null);

    [Fact]
    public async Task ManejarAsync_DatosValidosSinTecnico_CreaElServicio()
    {
        var cliente = CrearCliente();
        _clienteRepository.Setup(r => r.ObtenerPorIdAsync(cliente.Id, It.IsAny<CancellationToken>())).ReturnsAsync(cliente);

        var handler = CrearHandler();
        var comando = new SolicitarServicioCampoCommand(cliente.Id, "Instalación de cámaras", null);
        var resultado = await handler.ManejarAsync(comando);

        Assert.Equal("Solicitado", resultado.Estado);
        _servicioCampoRepository.Verify(r => r.Agregar(It.IsAny<Domain.Entities.ServiciosCampo.ServicioCampo>()), Times.Once);
        _servicioCampoRepository.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ManejarAsync_ConTecnicoExistente_AsignaTecnico()
    {
        var cliente = CrearCliente();
        var tecnico = new Usuario("Ana Torres", "atorres", "hash", DateTime.UtcNow);
        _clienteRepository.Setup(r => r.ObtenerPorIdAsync(cliente.Id, It.IsAny<CancellationToken>())).ReturnsAsync(cliente);
        _usuarioRepository.Setup(r => r.ObtenerPorIdAsync(tecnico.Id, It.IsAny<CancellationToken>())).ReturnsAsync(tecnico);

        var handler = CrearHandler();
        var comando = new SolicitarServicioCampoCommand(cliente.Id, "Instalación de cámaras", tecnico.Id);
        var resultado = await handler.ManejarAsync(comando);

        Assert.Equal(tecnico.Id, resultado.TecnicoAsignadoId);
    }

    [Fact]
    public async Task ManejarAsync_ClienteInexistente_LanzaExcepcion()
    {
        _clienteRepository.Setup(r => r.ObtenerPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Cliente?)null);

        var handler = CrearHandler();
        var comando = new SolicitarServicioCampoCommand(Guid.NewGuid(), "Instalación de cámaras", null);

        await Assert.ThrowsAsync<ClienteNoEncontradoException>(() => handler.ManejarAsync(comando));
    }

    [Fact]
    public async Task ManejarAsync_TecnicoInexistente_LanzaExcepcion()
    {
        var cliente = CrearCliente();
        _clienteRepository.Setup(r => r.ObtenerPorIdAsync(cliente.Id, It.IsAny<CancellationToken>())).ReturnsAsync(cliente);
        _usuarioRepository.Setup(r => r.ObtenerPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Usuario?)null);

        var handler = CrearHandler();
        var comando = new SolicitarServicioCampoCommand(cliente.Id, "Instalación de cámaras", Guid.NewGuid());

        await Assert.ThrowsAsync<UsuarioNoEncontradoException>(() => handler.ManejarAsync(comando));
    }
}
