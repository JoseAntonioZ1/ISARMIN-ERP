using FluentValidation;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Usuarios;
using ISARMIN.Application.Modulos.Usuarios.Commands.CrearRol;
using ISARMIN.Domain.Entities.Identidad;
using Moq;
using Xunit;

namespace ISARMIN.Application.Tests.Modulos.Usuarios;

public class CrearRolCommandHandlerTests
{
    private readonly Mock<IRolRepository> _rolRepository = new();
    private readonly IValidator<CrearRolCommand> _validator = new CrearRolCommandValidator();

    private CrearRolCommandHandler CrearHandler() => new(_rolRepository.Object, _validator);

    [Fact]
    public async Task ManejarAsync_NombreDisponible_CreaElRol()
    {
        _rolRepository.Setup(r => r.ObtenerPorNombreAsync("Caja", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Rol?)null);

        var handler = CrearHandler();
        var resultado = await handler.ManejarAsync(new CrearRolCommand("Caja", "Encargado de caja"));

        Assert.Equal("Caja", resultado.Nombre);
        Assert.Empty(resultado.Permisos);
        _rolRepository.Verify(r => r.Agregar(It.IsAny<Rol>()), Times.Once);
        _rolRepository.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ManejarAsync_NombreYaExiste_LanzaExcepcion()
    {
        _rolRepository.Setup(r => r.ObtenerPorNombreAsync("Ventas", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Rol("Ventas"));

        var handler = CrearHandler();

        await Assert.ThrowsAsync<NombreRolDuplicadoException>(() =>
            handler.ManejarAsync(new CrearRolCommand("Ventas", null)));
    }

    [Fact]
    public async Task ManejarAsync_NombreVacio_FallaValidacion()
    {
        var handler = CrearHandler();

        await Assert.ThrowsAsync<ValidationException>(() => handler.ManejarAsync(new CrearRolCommand("", null)));
    }
}
