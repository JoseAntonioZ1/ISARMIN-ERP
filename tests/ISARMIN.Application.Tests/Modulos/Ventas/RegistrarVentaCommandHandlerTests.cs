using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Clientes;
using ISARMIN.Application.Modulos.Configuracion;
using ISARMIN.Application.Modulos.Inventario;
using ISARMIN.Application.Modulos.Usuarios;
using ISARMIN.Application.Modulos.Ventas;
using ISARMIN.Application.Modulos.Ventas.Commands.RegistrarVenta;
using ISARMIN.Domain.Entities.Configuracion;
using ISARMIN.Domain.Entities.Identidad;
using ISARMIN.Domain.Entities.Inventario;
using ISARMIN.Domain.Entities.Terceros;
using ISARMIN.Domain.Entities.Ventas;
using ISARMIN.Domain.Enums;
using Moq;
using Xunit;

namespace ISARMIN.Application.Tests.Modulos.Ventas;

public class RegistrarVentaCommandHandlerTests
{
    private readonly Mock<IVentaRepository> _ventaRepository = new();
    private readonly Mock<IClienteRepository> _clienteRepository = new();
    private readonly Mock<IProductoRepository> _productoRepository = new();
    private readonly Mock<IMedioPagoRepository> _medioPagoRepository = new();
    private readonly Mock<IUsuarioRepository> _usuarioRepository = new();
    private readonly Mock<IMovimientoInventarioRepository> _movimientoInventarioRepository = new();
    private readonly Mock<IFechaHoraProvider> _fechaHoraProvider = new();
    private readonly RegistrarVentaCommandValidator _validator = new();

    public RegistrarVentaCommandHandlerTests()
    {
        _fechaHoraProvider.Setup(f => f.UtcAhora).Returns(new DateTime(2026, 7, 20, 10, 0, 0, DateTimeKind.Utc));
    }

    private RegistrarVentaCommandHandler CrearHandler() => new(
        _ventaRepository.Object, _clienteRepository.Object, _productoRepository.Object, _medioPagoRepository.Object,
        _usuarioRepository.Object, _movimientoInventarioRepository.Object, _fechaHoraProvider.Object, _validator);

    private static Producto CrearProducto(decimal stock) =>
        new("COD-001", "Taladro", Guid.NewGuid(), Guid.NewGuid(), 100m, 150m, stock);

    [Fact]
    public async Task ManejarAsync_PagoCompleto_CreaLaVentaPagada()
    {
        var producto = CrearProducto(10m);
        var medioPago = new MedioPago("Efectivo");
        _productoRepository.Setup(r => r.ObtenerPorIdAsync(producto.Id, It.IsAny<CancellationToken>())).ReturnsAsync(producto);
        _medioPagoRepository.Setup(r => r.ObtenerPorIdAsync(medioPago.Id, It.IsAny<CancellationToken>())).ReturnsAsync(medioPago);

        var handler = CrearHandler();
        var comando = new RegistrarVentaCommand(
            null, TipoComprobante.Ticket,
            [new DetalleVentaInput(producto.Id, 2m, 150m)],
            [new PagoVentaInput(medioPago.Id, 300m)],
            null, Guid.NewGuid());

        var resultado = await handler.ManejarAsync(comando);

        Assert.Equal("Pagada", resultado.Estado);
        Assert.Equal(300m, resultado.Total);
        Assert.Equal(8m, producto.StockActual);
        _movimientoInventarioRepository.Verify(r => r.Agregar(It.IsAny<MovimientoInventario>()), Times.Once);
        _ventaRepository.Verify(r => r.Agregar(It.IsAny<Venta>()), Times.Once);
        _ventaRepository.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ManejarAsync_SinPagoConAutorizacion_CreaLaVentaRegistradaConSaldoPendiente()
    {
        var producto = CrearProducto(10m);
        var autorizanteId = Guid.NewGuid();
        _productoRepository.Setup(r => r.ObtenerPorIdAsync(producto.Id, It.IsAny<CancellationToken>())).ReturnsAsync(producto);
        _usuarioRepository.Setup(r => r.ObtenerPorIdAsync(autorizanteId, It.IsAny<CancellationToken>())).ReturnsAsync(
            new Usuario("Admin", "admin2", "hash", DateTime.UtcNow));

        var handler = CrearHandler();
        var comando = new RegistrarVentaCommand(
            null, TipoComprobante.NotaVenta,
            [new DetalleVentaInput(producto.Id, 1m, 150m)],
            [],
            autorizanteId, Guid.NewGuid());

        var resultado = await handler.ManejarAsync(comando);

        Assert.Equal("Registrada", resultado.Estado);
        Assert.Equal(150m, resultado.SaldoPendiente);
    }

    [Fact]
    public async Task ManejarAsync_StockInsuficiente_LanzaExcepcion()
    {
        var producto = CrearProducto(1m);
        _productoRepository.Setup(r => r.ObtenerPorIdAsync(producto.Id, It.IsAny<CancellationToken>())).ReturnsAsync(producto);

        var handler = CrearHandler();
        var comando = new RegistrarVentaCommand(
            null, TipoComprobante.Ticket,
            [new DetalleVentaInput(producto.Id, 5m, 150m)],
            [new PagoVentaInput(Guid.NewGuid(), 750m)],
            null, Guid.NewGuid());

        await Assert.ThrowsAsync<StockInsuficienteException>(() => handler.ManejarAsync(comando));
    }

    [Fact]
    public async Task ManejarAsync_ClienteInexistente_LanzaExcepcion()
    {
        _clienteRepository.Setup(r => r.ObtenerPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Cliente?)null);

        var handler = CrearHandler();
        var comando = new RegistrarVentaCommand(
            Guid.NewGuid(), TipoComprobante.Ticket,
            [new DetalleVentaInput(Guid.NewGuid(), 1m, 100m)],
            [new PagoVentaInput(Guid.NewGuid(), 100m)],
            null, Guid.NewGuid());

        await Assert.ThrowsAsync<ClienteNoEncontradoException>(() => handler.ManejarAsync(comando));
    }

    [Fact]
    public async Task ManejarAsync_MedioPagoInexistente_LanzaExcepcion()
    {
        var producto = CrearProducto(10m);
        _productoRepository.Setup(r => r.ObtenerPorIdAsync(producto.Id, It.IsAny<CancellationToken>())).ReturnsAsync(producto);
        _medioPagoRepository.Setup(r => r.ObtenerPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((MedioPago?)null);

        var handler = CrearHandler();
        var comando = new RegistrarVentaCommand(
            null, TipoComprobante.Ticket,
            [new DetalleVentaInput(producto.Id, 1m, 150m)],
            [new PagoVentaInput(Guid.NewGuid(), 150m)],
            null, Guid.NewGuid());

        await Assert.ThrowsAsync<MedioPagoNoEncontradoException>(() => handler.ManejarAsync(comando));
    }

    [Fact]
    public async Task ManejarAsync_SaldoPendienteSinUsuarioAutorizante_FallaValidacion()
    {
        var handler = CrearHandler();
        var comando = new RegistrarVentaCommand(
            null, TipoComprobante.Ticket,
            [new DetalleVentaInput(Guid.NewGuid(), 1m, 150m)],
            [new PagoVentaInput(Guid.NewGuid(), 50m)],
            null, Guid.NewGuid());

        await Assert.ThrowsAsync<ValidationException>(() => handler.ManejarAsync(comando));
    }

    [Fact]
    public async Task ManejarAsync_UsuarioAutorizanteInexistente_LanzaExcepcion()
    {
        var producto = CrearProducto(10m);
        _productoRepository.Setup(r => r.ObtenerPorIdAsync(producto.Id, It.IsAny<CancellationToken>())).ReturnsAsync(producto);
        _usuarioRepository.Setup(r => r.ObtenerPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Usuario?)null);

        var handler = CrearHandler();
        var comando = new RegistrarVentaCommand(
            null, TipoComprobante.Ticket,
            [new DetalleVentaInput(producto.Id, 1m, 150m)],
            [],
            Guid.NewGuid(), Guid.NewGuid());

        await Assert.ThrowsAsync<UsuarioNoEncontradoException>(() => handler.ManejarAsync(comando));
    }
}
