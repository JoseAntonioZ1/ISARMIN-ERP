using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Inventario;
using ISARMIN.Application.Modulos.Inventario.Commands.CrearCategoria;
using ISARMIN.Domain.Entities.Inventario;
using Moq;
using Xunit;

namespace ISARMIN.Application.Tests.Modulos.Inventario;

public class CrearCategoriaCommandHandlerTests
{
    private readonly Mock<ICategoriaRepository> _categoriaRepository = new();
    private readonly CrearCategoriaCommandValidator _validator = new();

    private CrearCategoriaCommandHandler CrearHandler() => new(_categoriaRepository.Object, _validator);

    [Fact]
    public async Task ManejarAsync_NombreDisponible_CreaLaCategoria()
    {
        _categoriaRepository.Setup(r => r.ObtenerPorNombreAsync("Repuestos", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Categoria?)null);

        var handler = CrearHandler();
        var resultado = await handler.ManejarAsync(new CrearCategoriaCommand("Repuestos", null));

        Assert.Equal("Repuestos", resultado.Nombre);
        _categoriaRepository.Verify(r => r.Agregar(It.IsAny<Categoria>()), Times.Once);
    }

    [Fact]
    public async Task ManejarAsync_NombreYaExiste_LanzaExcepcion()
    {
        _categoriaRepository.Setup(r => r.ObtenerPorNombreAsync("Repuestos", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Categoria("Repuestos"));

        var handler = CrearHandler();

        await Assert.ThrowsAsync<NombreCategoriaDuplicadoException>(() =>
            handler.ManejarAsync(new CrearCategoriaCommand("Repuestos", null)));
    }

    [Fact]
    public async Task ManejarAsync_CategoriaPadreInexistente_LanzaExcepcion()
    {
        _categoriaRepository.Setup(r => r.ObtenerPorNombreAsync("Repuestos", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Categoria?)null);
        _categoriaRepository.Setup(r => r.ExisteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = CrearHandler();

        await Assert.ThrowsAsync<CategoriaPadreInvalidaException>(() =>
            handler.ManejarAsync(new CrearCategoriaCommand("Repuestos", Guid.NewGuid())));
    }
}
