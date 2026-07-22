using ISARMIN.Domain.Entities.Inventario;
using Xunit;

namespace ISARMIN.Domain.Tests.Entities.Inventario;

public class ProductoTests
{
    private static Producto CrearProductoValido() =>
        new("COD-001", "Taladro percutor", Guid.NewGuid(), Guid.NewGuid(), 100m, 150m, 10m);

    [Fact]
    public void Constructor_CodigoInternoVacio_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() =>
            new Producto("", "Taladro", Guid.NewGuid(), Guid.NewGuid(), 100m, 150m, 10m));
    }

    [Fact]
    public void Constructor_NombreVacio_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() =>
            new Producto("COD-001", "", Guid.NewGuid(), Guid.NewGuid(), 100m, 150m, 10m));
    }

    [Fact]
    public void Constructor_CostoNegativo_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() =>
            new Producto("COD-001", "Taladro", Guid.NewGuid(), Guid.NewGuid(), -1m, 150m, 10m));
    }

    [Fact]
    public void Constructor_PrecioNegativo_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() =>
            new Producto("COD-001", "Taladro", Guid.NewGuid(), Guid.NewGuid(), 100m, -1m, 10m));
    }

    [Fact]
    public void Constructor_StockInicialNegativo_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() =>
            new Producto("COD-001", "Taladro", Guid.NewGuid(), Guid.NewGuid(), 100m, 150m, -1m));
    }

    [Fact]
    public void Constructor_StockMinimoNegativo_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() =>
            new Producto("COD-001", "Taladro", Guid.NewGuid(), Guid.NewGuid(), 100m, 150m, 10m, stockMinimo: -1m));
    }

    [Fact]
    public void Constructor_DatosValidos_CreaProductoActivoConMargenCalculado()
    {
        var producto = CrearProductoValido();

        Assert.True(producto.EstaActivo);
        Assert.Equal(50m, producto.Margen);
        Assert.Equal(10m, producto.StockActual);
        Assert.Null(producto.StockMinimo);
        Assert.Null(producto.CodigoBarras);
        Assert.Null(producto.Imagen);
    }

    [Fact]
    public void Constructor_ConImagen_AsignaLaImagen()
    {
        var producto = new Producto(
            "COD-001", "Taladro", Guid.NewGuid(), Guid.NewGuid(), 100m, 150m, 10m,
            imagen: "data:image/png;base64,abc123");

        Assert.Equal("data:image/png;base64,abc123", producto.Imagen);
    }

    [Fact]
    public void ActualizarDatos_CambiaLosCamposSinAfectarElStock()
    {
        var producto = CrearProductoValido();
        var nuevaCategoria = Guid.NewGuid();
        var nuevaUnidad = Guid.NewGuid();

        producto.ActualizarDatos(
            "COD-002", "Taladro percutor 1/2", nuevaCategoria, nuevaUnidad, 120m, 180m,
            "Bosch", "7501234567890", 5m, "data:image/png;base64,def456");

        Assert.Equal("COD-002", producto.CodigoInterno);
        Assert.Equal("Taladro percutor 1/2", producto.Nombre);
        Assert.Equal(nuevaCategoria, producto.CategoriaId);
        Assert.Equal(nuevaUnidad, producto.UnidadMedidaId);
        Assert.Equal(120m, producto.CostoReferencia);
        Assert.Equal(180m, producto.PrecioVenta);
        Assert.Equal("Bosch", producto.Marca);
        Assert.Equal("7501234567890", producto.CodigoBarras);
        Assert.Equal(5m, producto.StockMinimo);
        Assert.Equal(10m, producto.StockActual);
        Assert.Equal("data:image/png;base64,def456", producto.Imagen);
    }

    [Fact]
    public void Desactivar_LuegoActivar_RestauraElEstado()
    {
        var producto = CrearProductoValido();

        producto.Desactivar();
        Assert.False(producto.EstaActivo);

        producto.Activar();
        Assert.True(producto.EstaActivo);
    }

    [Fact]
    public void AjustarStock_ResultadoPositivo_ActualizaElStock()
    {
        var producto = CrearProductoValido();

        producto.AjustarStock(-4m);

        Assert.Equal(6m, producto.StockActual);
    }

    [Fact]
    public void AjustarStock_ResultadoNegativo_LanzaExcepcion()
    {
        var producto = CrearProductoValido();

        Assert.Throws<ArgumentException>(() => producto.AjustarStock(-11m));
    }

    [Fact]
    public void RegistrarCompra_CantidadCero_LanzaExcepcion()
    {
        var producto = CrearProductoValido();

        Assert.Throws<ArgumentException>(() => producto.RegistrarCompra(0m, 100m));
    }

    [Fact]
    public void RegistrarCompra_CostoNegativo_LanzaExcepcion()
    {
        var producto = CrearProductoValido();

        Assert.Throws<ArgumentException>(() => producto.RegistrarCompra(5m, -1m));
    }

    [Fact]
    public void RegistrarCompra_ActualizaStockYCostoPromedioPonderado()
    {
        // Producto con stock 10 a costo 100 (RN-013): comprar 10 más a costo 200
        // debe dejar costo promedio = (10*100 + 10*200) / 20 = 150.
        var producto = new Producto("COD-001", "Taladro", Guid.NewGuid(), Guid.NewGuid(), 100m, 150m, 10m);

        producto.RegistrarCompra(10m, 200m);

        Assert.Equal(20m, producto.StockActual);
        Assert.Equal(150m, producto.CostoReferencia);
    }

    [Fact]
    public void RegistrarCompra_ConStockInicialCero_TomaElCostoDeCompra()
    {
        var producto = new Producto("COD-001", "Taladro", Guid.NewGuid(), Guid.NewGuid(), 0m, 150m, 0m);

        producto.RegistrarCompra(5m, 80m);

        Assert.Equal(5m, producto.StockActual);
        Assert.Equal(80m, producto.CostoReferencia);
    }
}
