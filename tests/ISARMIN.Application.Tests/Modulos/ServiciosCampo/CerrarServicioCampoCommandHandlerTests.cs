using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Inventario;
using ISARMIN.Application.Modulos.ServiciosCampo;
using ISARMIN.Application.Modulos.ServiciosCampo.Commands.CerrarServicioCampo;
using ISARMIN.Domain.Entities.Inventario;
using ISARMIN.Domain.Entities.ServiciosCampo;
using Moq;
using Xunit;

namespace ISARMIN.Application.Tests.Modulos.ServiciosCampo;

public class CerrarServicioCampoCommandHandlerTests
{
    private readonly Mock<IServicioCampoRepository> _servicioCampoRepository = new();
    private readonly Mock<IProductoRepository> _productoRepository = new();
    private readonly Mock<IMovimientoInventarioRepository> _movimientoInventarioRepository = new();
    private readonly Mock<IFechaHoraProvider> _fechaHoraProvider = new();
    private readonly CerrarServicioCampoCommandValidator _validator = new();

    public CerrarServicioCampoCommandHandlerTests()
    {
        _fechaHoraProvider.Setup(f => f.UtcAhora).Returns(new DateTime(2026, 7, 19, 12, 0, 0, DateTimeKind.Utc));
    }

    private CerrarServicioCampoCommandHandler CrearHandler() => new(
        _servicioCampoRepository.Object, _productoRepository.Object, _movimientoInventarioRepository.Object, _fechaHoraProvider.Object, _validator);

    private static ServicioCampo CrearServicioSolicitado() =>
        new(Guid.NewGuid(), "Instalación de cámaras", null, DateTime.UtcNow);

    [Fact]
    public async Task ManejarAsync_ConsumoValido_DescuentaStockYCierraElServicio()
    {
        var servicio = CrearServicioSolicitado();
        var producto = new Producto("COD-001", "Cable UTP", Guid.NewGuid(), Guid.NewGuid(), 5m, 10m, 10m);

        _servicioCampoRepository.Setup(r => r.ObtenerPorIdAsync(servicio.Id, It.IsAny<CancellationToken>())).ReturnsAsync(servicio);
        _productoRepository.Setup(r => r.ObtenerPorIdAsync(producto.Id, It.IsAny<CancellationToken>())).ReturnsAsync(producto);

        var handler = CrearHandler();
        var comando = new CerrarServicioCampoCommand(servicio.Id, [new DetalleConsumoCampoInput(producto.Id, 4m)], "Completado", "Sin novedad", Guid.NewGuid());
        var resultado = await handler.ManejarAsync(comando);

        Assert.Equal("Cerrado", resultado.Estado);
        Assert.Equal(6m, producto.StockActual);
        Assert.Single(resultado.Detalles);
        _movimientoInventarioRepository.Verify(r => r.Agregar(It.IsAny<MovimientoInventario>()), Times.Once);
        _servicioCampoRepository.Verify(r => r.AgregarDetalles(It.Is<IEnumerable<ServicioCampoDetalle>>(d => d.Count() == 1)), Times.Once);
        _servicioCampoRepository.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ManejarAsync_StockInsuficiente_LanzaExcepcion()
    {
        var servicio = CrearServicioSolicitado();
        var producto = new Producto("COD-001", "Cable UTP", Guid.NewGuid(), Guid.NewGuid(), 5m, 10m, 2m);

        _servicioCampoRepository.Setup(r => r.ObtenerPorIdAsync(servicio.Id, It.IsAny<CancellationToken>())).ReturnsAsync(servicio);
        _productoRepository.Setup(r => r.ObtenerPorIdAsync(producto.Id, It.IsAny<CancellationToken>())).ReturnsAsync(producto);

        var handler = CrearHandler();
        var comando = new CerrarServicioCampoCommand(servicio.Id, [new DetalleConsumoCampoInput(producto.Id, 4m)], "Completado", null, Guid.NewGuid());

        await Assert.ThrowsAsync<StockInsuficienteException>(() => handler.ManejarAsync(comando));
    }

    [Fact]
    public async Task ManejarAsync_ServicioYaCerrado_LanzaExcepcion()
    {
        var servicio = CrearServicioSolicitado();
        servicio.Cerrar([], "Completado", null, Guid.NewGuid(), DateTime.UtcNow);
        _servicioCampoRepository.Setup(r => r.ObtenerPorIdAsync(servicio.Id, It.IsAny<CancellationToken>())).ReturnsAsync(servicio);

        var handler = CrearHandler();
        var comando = new CerrarServicioCampoCommand(servicio.Id, [], "Completado", null, Guid.NewGuid());

        await Assert.ThrowsAsync<EstadoServicioCampoInvalidoException>(() => handler.ManejarAsync(comando));
    }

    [Fact]
    public async Task ManejarAsync_ServicioInexistente_LanzaExcepcion()
    {
        _servicioCampoRepository.Setup(r => r.ObtenerPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((ServicioCampo?)null);

        var handler = CrearHandler();
        var comando = new CerrarServicioCampoCommand(Guid.NewGuid(), [], "Completado", null, Guid.NewGuid());

        await Assert.ThrowsAsync<ServicioCampoNoEncontradoException>(() => handler.ManejarAsync(comando));
    }
}
