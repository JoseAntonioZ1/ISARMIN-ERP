using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Inventario;
using ISARMIN.Application.Modulos.Inventario.Commands.AjustarInventario;
using ISARMIN.Domain.Entities.Inventario;
using Moq;
using Xunit;

namespace ISARMIN.Application.Tests.Modulos.Inventario;

public class AjustarInventarioCommandHandlerTests
{
    private readonly Mock<IProductoRepository> _productoRepository = new();
    private readonly Mock<IMovimientoInventarioRepository> _movimientoInventarioRepository = new();
    private readonly Mock<IFechaHoraProvider> _fechaHoraProvider = new();
    private readonly AjustarInventarioCommandValidator _validator = new();

    private AjustarInventarioCommandHandler CrearHandler() =>
        new(_productoRepository.Object, _movimientoInventarioRepository.Object, _fechaHoraProvider.Object, _validator);

    private static Producto CrearProductoConStock(decimal stock) =>
        new("COD-001", "Taladro", Guid.NewGuid(), Guid.NewGuid(), 100m, 150m, stock);

    public AjustarInventarioCommandHandlerTests()
    {
        _fechaHoraProvider.Setup(f => f.UtcAhora).Returns(new DateTime(2026, 7, 19, 0, 0, 0, DateTimeKind.Utc));
    }

    [Fact]
    public async Task ManejarAsync_AjusteValido_ActualizaStockYRegistraMovimiento()
    {
        var producto = CrearProductoConStock(10m);
        _productoRepository.Setup(r => r.ObtenerPorIdAsync(producto.Id, It.IsAny<CancellationToken>())).ReturnsAsync(producto);

        var handler = CrearHandler();
        var resultado = await handler.ManejarAsync(new AjustarInventarioCommand(producto.Id, -3m, "Conteo físico", Guid.NewGuid()));

        Assert.Equal(7m, resultado.StockActual);
        _movimientoInventarioRepository.Verify(r => r.Agregar(It.IsAny<MovimientoInventario>()), Times.Once);
        _productoRepository.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ManejarAsync_SinMotivo_FallaValidacion()
    {
        var handler = CrearHandler();

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.ManejarAsync(new AjustarInventarioCommand(Guid.NewGuid(), -1m, "", Guid.NewGuid())));
    }

    [Fact]
    public async Task ManejarAsync_ProductoInexistente_LanzaExcepcion()
    {
        _productoRepository.Setup(r => r.ObtenerPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Producto?)null);

        var handler = CrearHandler();

        await Assert.ThrowsAsync<ProductoNoEncontradoException>(() =>
            handler.ManejarAsync(new AjustarInventarioCommand(Guid.NewGuid(), -1m, "Conteo", Guid.NewGuid())));
    }

    [Fact]
    public async Task ManejarAsync_ResultaEnStockNegativo_LanzaExcepcion()
    {
        var producto = CrearProductoConStock(2m);
        _productoRepository.Setup(r => r.ObtenerPorIdAsync(producto.Id, It.IsAny<CancellationToken>())).ReturnsAsync(producto);

        var handler = CrearHandler();

        await Assert.ThrowsAsync<AjusteInventarioInvalidoException>(() =>
            handler.ManejarAsync(new AjustarInventarioCommand(producto.Id, -5m, "Conteo físico", Guid.NewGuid())));
    }
}
