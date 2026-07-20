using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Inventario;
using ISARMIN.Application.Modulos.Ventas;
using ISARMIN.Application.Modulos.Ventas.Commands.AnularVenta;
using ISARMIN.Domain.Entities.Inventario;
using ISARMIN.Domain.Entities.Ventas;
using ISARMIN.Domain.Enums;
using Moq;
using Xunit;

namespace ISARMIN.Application.Tests.Modulos.Ventas;

public class AnularVentaCommandHandlerTests
{
    private readonly Mock<IVentaRepository> _ventaRepository = new();
    private readonly Mock<IProductoRepository> _productoRepository = new();
    private readonly Mock<IMovimientoInventarioRepository> _movimientoInventarioRepository = new();
    private readonly Mock<IFechaHoraProvider> _fechaHoraProvider = new();
    private readonly AnularVentaCommandValidator _validator = new();

    public AnularVentaCommandHandlerTests()
    {
        _fechaHoraProvider.Setup(f => f.UtcAhora).Returns(new DateTime(2026, 7, 20, 11, 0, 0, DateTimeKind.Utc));
    }

    private AnularVentaCommandHandler CrearHandler() => new(
        _ventaRepository.Object, _productoRepository.Object, _movimientoInventarioRepository.Object, _fechaHoraProvider.Object, _validator);

    private static Producto CrearProducto(decimal stock) =>
        new("COD-001", "Taladro", Guid.NewGuid(), Guid.NewGuid(), 100m, 150m, stock);

    [Fact]
    public async Task ManejarAsync_VentaRegistrada_SeAnulaYRevierteInventario()
    {
        var producto = CrearProducto(8m);
        var venta = new Venta(null, TipoComprobante.Ticket, Guid.NewGuid(), DateTime.UtcNow,
            [(producto.Id, 2m, 150m)], [(Guid.NewGuid(), 300m)], null);

        _ventaRepository.Setup(r => r.ObtenerPorIdAsync(venta.Id, It.IsAny<CancellationToken>())).ReturnsAsync(venta);
        _productoRepository.Setup(r => r.ObtenerPorIdAsync(producto.Id, It.IsAny<CancellationToken>())).ReturnsAsync(producto);

        var handler = CrearHandler();
        var comando = new AnularVentaCommand(venta.Id, "Cliente se arrepintió", Guid.NewGuid());
        var resultado = await handler.ManejarAsync(comando);

        Assert.Equal("Anulada", resultado.Estado);
        Assert.Equal(10m, producto.StockActual);
        _movimientoInventarioRepository.Verify(r => r.Agregar(It.IsAny<MovimientoInventario>()), Times.Once);
        _ventaRepository.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ManejarAsync_VentaYaAnulada_LanzaExcepcion()
    {
        var producto = CrearProducto(10m);
        var venta = new Venta(null, TipoComprobante.Ticket, Guid.NewGuid(), DateTime.UtcNow,
            [(producto.Id, 1m, 150m)], [(Guid.NewGuid(), 150m)], null);
        venta.Anular("Motivo original", Guid.NewGuid(), DateTime.UtcNow);

        _ventaRepository.Setup(r => r.ObtenerPorIdAsync(venta.Id, It.IsAny<CancellationToken>())).ReturnsAsync(venta);

        var handler = CrearHandler();
        var comando = new AnularVentaCommand(venta.Id, "Otro motivo", Guid.NewGuid());

        await Assert.ThrowsAsync<EstadoVentaInvalidoException>(() => handler.ManejarAsync(comando));
    }

    [Fact]
    public async Task ManejarAsync_VentaInexistente_LanzaExcepcion()
    {
        _ventaRepository.Setup(r => r.ObtenerPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Venta?)null);

        var handler = CrearHandler();
        var comando = new AnularVentaCommand(Guid.NewGuid(), "Motivo", Guid.NewGuid());

        await Assert.ThrowsAsync<VentaNoEncontradaException>(() => handler.ManejarAsync(comando));
    }
}
