using FluentValidation;
using ISARMIN.Application.Modulos.Clientes;
using ISARMIN.Application.Modulos.Clientes.Commands.RegistrarCliente;
using ISARMIN.Domain.Entities.Terceros;
using Moq;
using Xunit;

namespace ISARMIN.Application.Tests.Modulos.Clientes;

public class RegistrarClienteCommandHandlerTests
{
    private readonly Mock<IClienteRepository> _clienteRepository = new();
    private readonly RegistrarClienteCommandValidator _validator = new();

    private RegistrarClienteCommandHandler CrearHandler() => new(_clienteRepository.Object, _validator);

    [Fact]
    public async Task ManejarAsync_DatosValidosSinDocumento_RegistraElCliente()
    {
        var handler = CrearHandler();
        var resultado = await handler.ManejarAsync(new RegistrarClienteCommand("Juan Pérez", "999888777", null, null, null));

        Assert.Equal("Juan Pérez", resultado.NombreRazonSocial);
        Assert.Null(resultado.TipoDocumento);
        _clienteRepository.Verify(r => r.Agregar(It.IsAny<Cliente>()), Times.Once);
    }

    [Fact]
    public async Task ManejarAsync_ConRucValido_RegistraComoJuridica()
    {
        var handler = CrearHandler();
        var resultado = await handler.ManejarAsync(
            new RegistrarClienteCommand("ISARMIN SAC", "999888777", null, "Ruc", "20100070970"));

        Assert.Equal("Juridica", resultado.TipoCliente);
    }

    [Fact]
    public async Task ManejarAsync_SinTelefono_FallaValidacion()
    {
        var handler = CrearHandler();

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.ManejarAsync(new RegistrarClienteCommand("Juan Pérez", "", null, null, null)));
    }

    [Fact]
    public async Task ManejarAsync_RucConDigitoVerificadorInvalido_FallaValidacion()
    {
        var handler = CrearHandler();

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.ManejarAsync(new RegistrarClienteCommand("ISARMIN SAC", "999888777", null, "Ruc", "20100070971")));
    }

    [Fact]
    public async Task ManejarAsync_TipoDocumentoSinNumero_FallaValidacion()
    {
        var handler = CrearHandler();

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.ManejarAsync(new RegistrarClienteCommand("Juan Pérez", "999888777", null, "Dni", null)));
    }
}
