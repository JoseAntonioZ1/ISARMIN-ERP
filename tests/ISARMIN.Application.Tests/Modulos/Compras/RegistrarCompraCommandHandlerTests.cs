using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Compras;
using ISARMIN.Application.Modulos.Compras.Commands.RegistrarCompra;
using ISARMIN.Application.Modulos.Inventario;
using ISARMIN.Application.Modulos.Proveedores;
using ISARMIN.Domain.Entities.Compras;
using ISARMIN.Domain.Entities.Inventario;
using ISARMIN.Domain.Entities.Terceros;
using Moq;
using Xunit;

namespace ISARMIN.Application.Tests.Modulos.Compras;

public class RegistrarCompraCommandHandlerTests
{
    private readonly Mock<ICompraRepository> _compraRepository = new();
    private readonly Mock<IProveedorRepository> _proveedorRepository = new();
    private readonly Mock<IProductoRepository> _productoRepository = new();
    private readonly Mock<IMovimientoInventarioRepository> _movimientoInventarioRepository = new();
    private readonly Mock<IFechaHoraProvider> _fechaHoraProvider = new();
    private readonly RegistrarCompraCommandValidator _validator = new();

    public RegistrarCompraCommandHandlerTests()
    {
        _fechaHoraProvider.Setup(f => f.UtcAhora).Returns(new DateTime(2026, 7, 19, 0, 0, 0, DateTimeKind.Utc));
        _proveedorRepository
            .Setup(r => r.ObtenerPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Proveedor("Distribuidora ACME", null, null, null));
    }

    private RegistrarCompraCommandHandler CrearHandler() => new(
        _compraRepository.Object,
        _proveedorRepository.Object,
        _productoRepository.Object,
        _movimientoInventarioRepository.Object,
        _fechaHoraProvider.Object,
        _validator);

    private static RegistrarCompraCommand ComandoValido(Guid productoId) => new(
        Guid.NewGuid(),
        new DateOnly(2026, 7, 19),
        "Factura",
        "F001-123",
        Guid.NewGuid(),
        [new DetalleCompraInput(productoId, 10m, 200m)]);

    [Fact]
    public async Task ManejarAsync_DatosValidos_RegistraLaCompraYActualizaElProducto()
    {
        var producto = new Producto("COD-001", "Taladro", Guid.NewGuid(), Guid.NewGuid(), 100m, 150m, 10m);
        _productoRepository.Setup(r => r.ObtenerPorIdAsync(producto.Id, It.IsAny<CancellationToken>())).ReturnsAsync(producto);

        var handler = CrearHandler();
        var resultado = await handler.ManejarAsync(ComandoValido(producto.Id));

        Assert.Equal(2000m, resultado.Total);
        Assert.Equal(20m, producto.StockActual);
        Assert.Equal(150m, producto.CostoReferencia);
        _compraRepository.Verify(r => r.Agregar(It.IsAny<Compra>()), Times.Once);
        _movimientoInventarioRepository.Verify(r => r.Agregar(It.IsAny<MovimientoInventario>()), Times.Once);
        _compraRepository.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ManejarAsync_SinDetalles_FallaValidacion()
    {
        var handler = CrearHandler();
        var comando = ComandoValido(Guid.NewGuid()) with { Detalles = [] };

        await Assert.ThrowsAsync<ValidationException>(() => handler.ManejarAsync(comando));
    }

    [Fact]
    public async Task ManejarAsync_ProveedorInexistente_LanzaExcepcion()
    {
        _proveedorRepository.Setup(r => r.ObtenerPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Proveedor?)null);

        var handler = CrearHandler();

        await Assert.ThrowsAsync<ProveedorNoEncontradoException>(() => handler.ManejarAsync(ComandoValido(Guid.NewGuid())));
    }

    [Fact]
    public async Task ManejarAsync_ProductoInexistente_LanzaExcepcion()
    {
        _productoRepository.Setup(r => r.ObtenerPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Producto?)null);

        var handler = CrearHandler();

        await Assert.ThrowsAsync<ProductoNoEncontradoException>(() => handler.ManejarAsync(ComandoValido(Guid.NewGuid())));
    }
}
