using FluentValidation;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Usuarios;
using ISARMIN.Application.Modulos.Usuarios.Commands.EditarUsuario;
using ISARMIN.Domain.Entities.Identidad;
using Moq;
using Xunit;

namespace ISARMIN.Application.Tests.Modulos.Usuarios;

public class EditarUsuarioCommandHandlerTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepository = new();
    private readonly Mock<IRolRepository> _rolRepository = new();
    private readonly IValidator<EditarUsuarioCommand> _validator = new EditarUsuarioCommandValidator();

    private EditarUsuarioCommandHandler CrearHandler() =>
        new(_usuarioRepository.Object, _rolRepository.Object, _validator);

    [Fact]
    public async Task ManejarAsync_UsuarioExistente_ActualizaNombreYRoles()
    {
        var rolNuevo = Guid.NewGuid();
        var usuario = new Usuario("Ana Pérez", "aperez", "hash", DateTime.UtcNow);
        _usuarioRepository.Setup(r => r.ObtenerPorIdAsync(usuario.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);
        _rolRepository.Setup(r => r.ObtenerIdsExistentesAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { rolNuevo });
        _rolRepository.Setup(r => r.ListarTodosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { new Rol("Técnico") });

        var handler = CrearHandler();
        var resultado = await handler.ManejarAsync(new EditarUsuarioCommand(usuario.Id, "Ana Pérez Gómez", [rolNuevo]));

        Assert.Equal("Ana Pérez Gómez", resultado.Nombre);
        Assert.Contains(usuario.Roles, r => r.RolId == rolNuevo);
        _usuarioRepository.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ManejarAsync_UsuarioNoExiste_LanzaExcepcion()
    {
        _usuarioRepository.Setup(r => r.ObtenerPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario?)null);

        var handler = CrearHandler();

        await Assert.ThrowsAsync<UsuarioNoEncontradoException>(() =>
            handler.ManejarAsync(new EditarUsuarioCommand(Guid.NewGuid(), "Nombre", [Guid.NewGuid()])));
    }
}
