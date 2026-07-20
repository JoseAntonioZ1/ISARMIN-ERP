using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Inventario;
using ISARMIN.Application.Modulos.Taller;
using ISARMIN.Application.Modulos.Taller.Commands.RegistrarReparacion;
using ISARMIN.Domain.Entities.Inventario;
using ISARMIN.Domain.Entities.Taller;
using ISARMIN.Domain.Enums;
using Moq;
using Xunit;

namespace ISARMIN.Application.Tests.Modulos.Taller;

public class RegistrarReparacionCommandHandlerTests
{
    private readonly Mock<IOrdenTrabajoRepository> _ordenTrabajoRepository = new();
    private readonly Mock<IProductoRepository> _productoRepository = new();
    private readonly Mock<IMovimientoInventarioRepository> _movimientoInventarioRepository = new();
    private readonly Mock<IFechaHoraProvider> _fechaHoraProvider = new();
    private readonly RegistrarReparacionCommandValidator _validator = new();

    public RegistrarReparacionCommandHandlerTests()
    {
        _fechaHoraProvider.Setup(f => f.UtcAhora).Returns(new DateTime(2026, 7, 20, 9, 0, 0, DateTimeKind.Utc));
    }

    private RegistrarReparacionCommandHandler CrearHandler() => new(
        _ordenTrabajoRepository.Object, _productoRepository.Object, _movimientoInventarioRepository.Object, _fechaHoraProvider.Object, _validator);

    private static OrdenTrabajo CrearOtAprobada()
    {
        var ot = new OrdenTrabajo(Guid.NewGuid(), "Taladro Bosch", "No enciende", Guid.NewGuid(), DateTime.UtcNow);
        ot.RegistrarDiagnostico("Motor quemado", Guid.NewGuid(), DateTime.UtcNow);
        ot.GenerarCotizacion(150m, DateTime.UtcNow);
        ot.RegistrarDecisionCliente(DecisionCliente.Aprobada, null, null);
        return ot;
    }

    [Fact]
    public async Task ManejarAsync_ConsumoValido_DescuentaStockYRegistraMovimiento()
    {
        var ot = CrearOtAprobada();
        var producto = new Producto("COD-001", "Carbón", Guid.NewGuid(), Guid.NewGuid(), 5m, 10m, 10m);

        _ordenTrabajoRepository.Setup(r => r.ObtenerPorIdAsync(ot.Id, It.IsAny<CancellationToken>())).ReturnsAsync(ot);
        _productoRepository.Setup(r => r.ObtenerPorIdAsync(producto.Id, It.IsAny<CancellationToken>())).ReturnsAsync(producto);

        var handler = CrearHandler();
        var comando = new RegistrarReparacionCommand(ot.Id, [new DetalleConsumoInput(producto.Id, 4m)], "Prueba OK", Guid.NewGuid());
        var resultado = await handler.ManejarAsync(comando);

        Assert.Equal("ListoParaEntrega", resultado.Estado);
        Assert.Equal(6m, producto.StockActual);
        _movimientoInventarioRepository.Verify(r => r.Agregar(It.IsAny<MovimientoInventario>()), Times.Once);
        _ordenTrabajoRepository.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ManejarAsync_StockInsuficiente_LanzaExcepcion()
    {
        var ot = CrearOtAprobada();
        var producto = new Producto("COD-001", "Carbón", Guid.NewGuid(), Guid.NewGuid(), 5m, 10m, 2m);

        _ordenTrabajoRepository.Setup(r => r.ObtenerPorIdAsync(ot.Id, It.IsAny<CancellationToken>())).ReturnsAsync(ot);
        _productoRepository.Setup(r => r.ObtenerPorIdAsync(producto.Id, It.IsAny<CancellationToken>())).ReturnsAsync(producto);

        var handler = CrearHandler();
        var comando = new RegistrarReparacionCommand(ot.Id, [new DetalleConsumoInput(producto.Id, 4m)], null, Guid.NewGuid());

        await Assert.ThrowsAsync<StockInsuficienteException>(() => handler.ManejarAsync(comando));
    }

    [Fact]
    public async Task ManejarAsync_OtNoAprobada_LanzaExcepcion()
    {
        var ot = new OrdenTrabajo(Guid.NewGuid(), "Taladro Bosch", "No enciende", Guid.NewGuid(), DateTime.UtcNow);
        _ordenTrabajoRepository.Setup(r => r.ObtenerPorIdAsync(ot.Id, It.IsAny<CancellationToken>())).ReturnsAsync(ot);

        var handler = CrearHandler();
        var comando = new RegistrarReparacionCommand(ot.Id, [], null, Guid.NewGuid());

        await Assert.ThrowsAsync<EstadoOrdenTrabajoInvalidoException>(() => handler.ManejarAsync(comando));
    }

    [Fact]
    public async Task ManejarAsync_OtInexistente_LanzaExcepcion()
    {
        _ordenTrabajoRepository.Setup(r => r.ObtenerPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((OrdenTrabajo?)null);

        var handler = CrearHandler();
        var comando = new RegistrarReparacionCommand(Guid.NewGuid(), [], null, Guid.NewGuid());

        await Assert.ThrowsAsync<OrdenTrabajoNoEncontradaException>(() => handler.ManejarAsync(comando));
    }
}
