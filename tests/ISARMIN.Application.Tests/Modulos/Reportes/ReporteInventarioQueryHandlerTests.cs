using ISARMIN.Application.Modulos.Reportes;
using ISARMIN.Application.Modulos.Reportes.Queries.ReporteInventario;
using ISARMIN.Domain.Entities.Inventario;
using Moq;
using Xunit;

namespace ISARMIN.Application.Tests.Modulos.Reportes;

public class ReporteInventarioQueryHandlerTests
{
    private readonly Mock<IReporteRepository> _reporteRepository = new();

    private ReporteInventarioQueryHandler CrearHandler() => new(_reporteRepository.Object);

    private static Producto CrearProducto(decimal stock, decimal? stockMinimo) =>
        new("COD-001", "Taladro", Guid.NewGuid(), Guid.NewGuid(), 100m, 150m, stock, stockMinimo: stockMinimo);

    [Fact]
    public async Task ManejarAsync_IdentificaProductosEnQuiebre()
    {
        var enQuiebre = CrearProducto(3m, 5m);
        var normal = CrearProducto(20m, 5m);
        var sinMinimo = CrearProducto(0m, null);

        _reporteRepository.Setup(r => r.ObtenerProductosAsync(It.IsAny<CancellationToken>())).ReturnsAsync([enQuiebre, normal, sinMinimo]);

        var handler = CrearHandler();
        var resultado = await handler.ManejarAsync(new ReporteInventarioQuery());

        Assert.Equal(3, resultado.Productos.Count);
        Assert.Single(resultado.ProductosEnQuiebre);
        Assert.Equal(enQuiebre.Id, resultado.ProductosEnQuiebre.Single().Id);
    }

    [Fact]
    public async Task ManejarAsync_StockIgualAlMinimo_CuentaComoQuiebre()
    {
        var producto = CrearProducto(5m, 5m);
        _reporteRepository.Setup(r => r.ObtenerProductosAsync(It.IsAny<CancellationToken>())).ReturnsAsync([producto]);

        var handler = CrearHandler();
        var resultado = await handler.ManejarAsync(new ReporteInventarioQuery());

        Assert.Single(resultado.ProductosEnQuiebre);
    }
}
