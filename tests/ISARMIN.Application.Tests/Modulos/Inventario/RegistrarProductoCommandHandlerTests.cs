using FluentValidation;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Inventario;
using ISARMIN.Application.Modulos.Inventario.Commands.RegistrarProducto;
using ISARMIN.Domain.Entities.Inventario;
using Moq;
using Xunit;

namespace ISARMIN.Application.Tests.Modulos.Inventario;

public class RegistrarProductoCommandHandlerTests
{
    private readonly Mock<IProductoRepository> _productoRepository = new();
    private readonly Mock<ICategoriaRepository> _categoriaRepository = new();
    private readonly Mock<IUnidadMedidaRepository> _unidadMedidaRepository = new();
    private readonly RegistrarProductoCommandValidator _validator = new();

    public RegistrarProductoCommandHandlerTests()
    {
        _categoriaRepository.Setup(r => r.ExisteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _unidadMedidaRepository.Setup(r => r.ExisteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
    }

    private RegistrarProductoCommandHandler CrearHandler() =>
        new(_productoRepository.Object, _categoriaRepository.Object, _unidadMedidaRepository.Object, _validator);

    private static RegistrarProductoCommand ComandoValido() => new(
        "COD-001", "Taladro percutor", Guid.NewGuid(), Guid.NewGuid(), 100m, 150m, 10m, "Bosch", "7501234567890", 2m,
        "data:image/png;base64,abc123");

    [Fact]
    public async Task ManejarAsync_DatosValidos_RegistraElProducto()
    {
        var handler = CrearHandler();
        var resultado = await handler.ManejarAsync(ComandoValido());

        Assert.Equal("COD-001", resultado.CodigoInterno);
        Assert.Equal(50m, resultado.Margen);
        Assert.Equal("data:image/png;base64,abc123", resultado.Imagen);
        _productoRepository.Verify(r => r.Agregar(It.IsAny<Producto>()), Times.Once);
        _productoRepository.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ManejarAsync_SinCodigoInterno_FallaValidacion()
    {
        var handler = CrearHandler();
        var comando = ComandoValido() with { CodigoInterno = "" };

        await Assert.ThrowsAsync<ValidationException>(() => handler.ManejarAsync(comando));
    }

    [Fact]
    public async Task ManejarAsync_CodigoInternoDuplicado_LanzaExcepcion()
    {
        var comando = ComandoValido();
        _productoRepository
            .Setup(r => r.ObtenerPorCodigoInternoAsync(comando.CodigoInterno, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Producto(comando.CodigoInterno, "Existente", Guid.NewGuid(), Guid.NewGuid(), 1m, 2m, 1m));

        var handler = CrearHandler();

        await Assert.ThrowsAsync<CodigoInternoDuplicadoException>(() => handler.ManejarAsync(comando));
    }

    [Fact]
    public async Task ManejarAsync_CategoriaInexistente_LanzaExcepcion()
    {
        _categoriaRepository.Setup(r => r.ExisteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var handler = CrearHandler();

        await Assert.ThrowsAsync<CategoriaInvalidaException>(() => handler.ManejarAsync(ComandoValido()));
    }

    [Fact]
    public async Task ManejarAsync_UnidadMedidaInexistente_LanzaExcepcion()
    {
        _unidadMedidaRepository.Setup(r => r.ExisteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var handler = CrearHandler();

        await Assert.ThrowsAsync<UnidadMedidaInvalidaException>(() => handler.ManejarAsync(ComandoValido()));
    }
}
