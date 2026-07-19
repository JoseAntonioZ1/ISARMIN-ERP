using FluentValidation;
using ISARMIN.Application.Modulos.Proveedores;
using ISARMIN.Application.Modulos.Proveedores.Commands.RegistrarProveedor;
using ISARMIN.Domain.Entities.Terceros;
using Moq;
using Xunit;

namespace ISARMIN.Application.Tests.Modulos.Proveedores;

public class RegistrarProveedorCommandHandlerTests
{
    private readonly Mock<IProveedorRepository> _proveedorRepository = new();
    private readonly RegistrarProveedorCommandValidator _validator = new();

    private RegistrarProveedorCommandHandler CrearHandler() => new(_proveedorRepository.Object, _validator);

    [Fact]
    public async Task ManejarAsync_DatosValidos_RegistraElProveedor()
    {
        var handler = CrearHandler();
        var resultado = await handler.ManejarAsync(new RegistrarProveedorCommand("Distribuidora ACME", null, "999888777", null));

        Assert.Equal("Distribuidora ACME", resultado.NombreRazonSocial);
        _proveedorRepository.Verify(r => r.Agregar(It.IsAny<Proveedor>()), Times.Once);
        _proveedorRepository.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ManejarAsync_SinNombre_FallaValidacion()
    {
        var handler = CrearHandler();

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.ManejarAsync(new RegistrarProveedorCommand("", null, null, null)));
    }
}
