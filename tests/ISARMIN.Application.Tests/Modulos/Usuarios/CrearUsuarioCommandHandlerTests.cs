using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Usuarios;
using ISARMIN.Application.Modulos.Usuarios.Commands.CrearUsuario;
using ISARMIN.Domain.Entities.Identidad;
using Moq;
using Xunit;

namespace ISARMIN.Application.Tests.Modulos.Usuarios;

public class CrearUsuarioCommandHandlerTests
{
    private static readonly DateTime Ahora = new(2026, 7, 19, 10, 0, 0, DateTimeKind.Utc);
    private readonly Guid _rolVentasId = Guid.NewGuid();

    private readonly Mock<IUsuarioRepository> _usuarioRepository = new();
    private readonly Mock<IRolRepository> _rolRepository = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<IFechaHoraProvider> _fechaHoraProvider = new();
    private readonly IValidator<CrearUsuarioCommand> _validator = new CrearUsuarioCommandValidator();

    private CrearUsuarioCommandHandler CrearHandler()
    {
        _fechaHoraProvider.Setup(f => f.UtcAhora).Returns(Ahora);
        return new CrearUsuarioCommandHandler(
            _usuarioRepository.Object, _rolRepository.Object, _passwordHasher.Object, _fechaHoraProvider.Object, _validator);
    }

    [Fact]
    public async Task ManejarAsync_DatosValidos_CreaUsuarioConRolesAsignados()
    {
        _usuarioRepository.Setup(r => r.ObtenerPorNombreUsuarioAsync("jperez", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario?)null);
        _rolRepository.Setup(r => r.ObtenerIdsExistentesAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { _rolVentasId });
        _rolRepository.Setup(r => r.ListarTodosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { new Rol("Ventas") });
        _passwordHasher.Setup(h => h.HashearCredencial("clave1234")).Returns("hash-generado");

        var handler = CrearHandler();
        var comando = new CrearUsuarioCommand("Juan Pérez", "jperez", "clave1234", [_rolVentasId]);

        var resultado = await handler.ManejarAsync(comando);

        Assert.Equal("jperez", resultado.NombreUsuario);
        _usuarioRepository.Verify(r => r.Agregar(It.IsAny<Usuario>()), Times.Once);
        _usuarioRepository.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ManejarAsync_NombreUsuarioYaExiste_LanzaExcepcion()
    {
        _usuarioRepository.Setup(r => r.ObtenerPorNombreUsuarioAsync("jperez", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Usuario("Otro", "jperez", "hash", Ahora));

        var handler = CrearHandler();
        var comando = new CrearUsuarioCommand("Juan Pérez", "jperez", "clave1234", [_rolVentasId]);

        await Assert.ThrowsAsync<NombreUsuarioDuplicadoException>(() => handler.ManejarAsync(comando));
    }

    [Fact]
    public async Task ManejarAsync_RolInexistente_LanzaExcepcion()
    {
        _usuarioRepository.Setup(r => r.ObtenerPorNombreUsuarioAsync("jperez", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario?)null);
        _rolRepository.Setup(r => r.ObtenerIdsExistentesAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Guid>());

        var handler = CrearHandler();
        var comando = new CrearUsuarioCommand("Juan Pérez", "jperez", "clave1234", [Guid.NewGuid()]);

        await Assert.ThrowsAsync<RolInvalidoException>(() => handler.ManejarAsync(comando));
    }

    [Fact]
    public async Task ManejarAsync_SinRoles_FallaValidacion()
    {
        var handler = CrearHandler();
        var comando = new CrearUsuarioCommand("Juan Pérez", "jperez", "clave1234", []);

        await Assert.ThrowsAsync<ValidationException>(() => handler.ManejarAsync(comando));
    }
}
