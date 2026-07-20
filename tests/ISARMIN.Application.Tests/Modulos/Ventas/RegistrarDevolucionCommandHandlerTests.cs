using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Inventario;
using ISARMIN.Application.Modulos.Ventas;
using ISARMIN.Application.Modulos.Ventas.Commands.RegistrarDevolucion;
using ISARMIN.Domain.Entities.Inventario;
using ISARMIN.Domain.Entities.Ventas;
using ISARMIN.Domain.Enums;
using Moq;
using Xunit;

namespace ISARMIN.Application.Tests.Modulos.Ventas;

public class RegistrarDevolucionCommandHandlerTests
{
    private readonly Mock<IVentaRepository> _ventaRepository = new();
    private readonly Mock<IProductoRepository> _productoRepository = new();
    private readonly Mock<IMovimientoInventarioRepository> _movimientoInventarioRepository = new();
    private readonly Mock<IFechaHoraProvider> _fechaHoraProvider = new();
    private readonly RegistrarDevolucionCommandValidator _validator = new();

    public RegistrarDevolucionCommandHandlerTests()
    {
        _fechaHoraProvider.Setup(f => f.UtcAhora).Returns(new DateTime(2026, 7, 20, 12, 0, 0, DateTimeKind.Utc));
    }

    private RegistrarDevolucionCommandHandler CrearHandler() => new(
        _ventaRepository.Object, _productoRepository.Object, _movimientoInventarioRepository.Object, _fechaHoraProvider.Object, _validator);

    private static Producto CrearProducto(decimal stock) =>
        new("COD-001", "Taladro", Guid.NewGuid(), Guid.NewGuid(), 100m, 150m, stock);

    [Fact]
    public async Task ManejarAsync_DatosValidos_RegistraDevolucionYRestauraStock()
    {
        var producto = CrearProducto(8m);
        var venta = new Venta(null, TipoComprobante.Ticket, Guid.NewGuid(), DateTime.UtcNow,
            [(producto.Id, 2m, 150m)], [(Guid.NewGuid(), 300m)], null);

        _ventaRepository.Setup(r => r.ObtenerPorIdAsync(venta.Id, It.IsAny<CancellationToken>())).ReturnsAsync(venta);
        _productoRepository.Setup(r => r.ObtenerPorIdAsync(producto.Id, It.IsAny<CancellationToken>())).ReturnsAsync(producto);

        var handler = CrearHandler();
        var comando = new RegistrarDevolucionCommand(venta.Id, [new DetalleDevolucionInput(producto.Id, 1m)], "Producto defectuoso", Guid.NewGuid());
        await handler.ManejarAsync(comando);

        Assert.Equal(9m, producto.StockActual);
        _movimientoInventarioRepository.Verify(r => r.Agregar(It.IsAny<MovimientoInventario>()), Times.Once);
        _ventaRepository.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ManejarAsync_ProductoNoVendido_LanzaExcepcion()
    {
        var productoVendido = CrearProducto(8m);
        var venta = new Venta(null, TipoComprobante.Ticket, Guid.NewGuid(), DateTime.UtcNow,
            [(productoVendido.Id, 2m, 150m)], [(Guid.NewGuid(), 300m)], null);

        _ventaRepository.Setup(r => r.ObtenerPorIdAsync(venta.Id, It.IsAny<CancellationToken>())).ReturnsAsync(venta);

        var handler = CrearHandler();
        var otroProductoId = Guid.NewGuid();
        var comando = new RegistrarDevolucionCommand(venta.Id, [new DetalleDevolucionInput(otroProductoId, 1m)], null, Guid.NewGuid());

        await Assert.ThrowsAsync<ProductoNoVendidoException>(() => handler.ManejarAsync(comando));
    }

    [Fact]
    public async Task ManejarAsync_VentaInexistente_LanzaExcepcion()
    {
        _ventaRepository.Setup(r => r.ObtenerPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Venta?)null);

        var handler = CrearHandler();
        var comando = new RegistrarDevolucionCommand(Guid.NewGuid(), [new DetalleDevolucionInput(Guid.NewGuid(), 1m)], null, Guid.NewGuid());

        await Assert.ThrowsAsync<VentaNoEncontradaException>(() => handler.ManejarAsync(comando));
    }
}
