using FluentValidation;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Usuarios;
using ISARMIN.Application.Modulos.Usuarios.Commands.AsignarPermisos;
using ISARMIN.Domain.Entities.Identidad;
using ISARMIN.Domain.Enums;
using Moq;
using Xunit;

namespace ISARMIN.Application.Tests.Modulos.Usuarios;

public class AsignarPermisosCommandHandlerTests
{
    private readonly Mock<IRolRepository> _rolRepository = new();
    private readonly IValidator<AsignarPermisosCommand> _validator = new AsignarPermisosCommandValidator();

    private AsignarPermisosCommandHandler CrearHandler() => new(_rolRepository.Object, _validator);

    [Fact]
    public async Task ManejarAsync_RolExistente_ReemplazaLosPermisos()
    {
        var rol = new Rol("Ventas");
        rol.AsignarPermiso("Clientes", AccionPermiso.Crear);
        _rolRepository.Setup(r => r.ObtenerPorIdAsync(rol.Id, It.IsAny<CancellationToken>())).ReturnsAsync(rol);

        var handler = CrearHandler();
        var comando = new AsignarPermisosCommand(rol.Id, [new PermisoAsignacion("Ventas", "Crear"), new PermisoAsignacion("Ventas", "Consultar")]);

        var resultado = await handler.ManejarAsync(comando);

        Assert.Equal(2, resultado.Permisos.Count);
        Assert.DoesNotContain(resultado.Permisos, p => p.Modulo == "Clientes");
        _rolRepository.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ManejarAsync_RolNoExiste_LanzaExcepcion()
    {
        _rolRepository.Setup(r => r.ObtenerPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Rol?)null);

        var handler = CrearHandler();
        var comando = new AsignarPermisosCommand(Guid.NewGuid(), [new PermisoAsignacion("Ventas", "Crear")]);

        await Assert.ThrowsAsync<RolNoEncontradoException>(() => handler.ManejarAsync(comando));
    }

    [Fact]
    public async Task ManejarAsync_AccionInvalida_FallaValidacion()
    {
        var handler = CrearHandler();
        var comando = new AsignarPermisosCommand(Guid.NewGuid(), [new PermisoAsignacion("Ventas", "Volar")]);

        await Assert.ThrowsAsync<ValidationException>(() => handler.ManejarAsync(comando));
    }
}
