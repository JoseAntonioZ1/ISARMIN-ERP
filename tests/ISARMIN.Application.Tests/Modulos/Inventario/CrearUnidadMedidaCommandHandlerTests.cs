using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Inventario;
using ISARMIN.Application.Modulos.Inventario.Commands.CrearUnidadMedida;
using ISARMIN.Domain.Entities.Inventario;
using Moq;
using Xunit;

namespace ISARMIN.Application.Tests.Modulos.Inventario;

public class CrearUnidadMedidaCommandHandlerTests
{
    private readonly Mock<IUnidadMedidaRepository> _unidadMedidaRepository = new();
    private readonly CrearUnidadMedidaCommandValidator _validator = new();

    private CrearUnidadMedidaCommandHandler CrearHandler() => new(_unidadMedidaRepository.Object, _validator);

    [Fact]
    public async Task ManejarAsync_NombreDisponible_CreaLaUnidadMedida()
    {
        _unidadMedidaRepository.Setup(r => r.ObtenerPorNombreAsync("Rollo", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UnidadMedida?)null);

        var handler = CrearHandler();
        var resultado = await handler.ManejarAsync(new CrearUnidadMedidaCommand("Rollo"));

        Assert.Equal("Rollo", resultado.Nombre);
        _unidadMedidaRepository.Verify(r => r.Agregar(It.IsAny<UnidadMedida>()), Times.Once);
    }

    [Fact]
    public async Task ManejarAsync_NombreYaExiste_LanzaExcepcion()
    {
        _unidadMedidaRepository.Setup(r => r.ObtenerPorNombreAsync("Rollo", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UnidadMedida("Rollo"));

        var handler = CrearHandler();

        await Assert.ThrowsAsync<NombreUnidadMedidaDuplicadoException>(() =>
            handler.ManejarAsync(new CrearUnidadMedidaCommand("Rollo")));
    }
}
