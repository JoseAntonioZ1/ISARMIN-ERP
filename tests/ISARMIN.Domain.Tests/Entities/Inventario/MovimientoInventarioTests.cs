using ISARMIN.Domain.Entities.Inventario;
using ISARMIN.Domain.Enums;
using Xunit;

namespace ISARMIN.Domain.Tests.Entities.Inventario;

public class MovimientoInventarioTests
{
    [Fact]
    public void CrearAjuste_CantidadCero_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() =>
            MovimientoInventario.CrearAjuste(Guid.NewGuid(), 0m, "Conteo físico", Guid.NewGuid(), DateTime.UtcNow));
    }

    [Fact]
    public void CrearAjuste_SinMotivo_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() =>
            MovimientoInventario.CrearAjuste(Guid.NewGuid(), 5m, "", Guid.NewGuid(), DateTime.UtcNow));
    }

    [Fact]
    public void CrearAjuste_DatosValidos_CreaMovimientoDeTipoAjuste()
    {
        var productoId = Guid.NewGuid();
        var usuarioId = Guid.NewGuid();
        var fecha = DateTime.UtcNow;

        var movimiento = MovimientoInventario.CrearAjuste(productoId, -3m, "Conteo físico", usuarioId, fecha);

        Assert.Equal(productoId, movimiento.ProductoId);
        Assert.Equal(TipoMovimientoInventario.Ajuste, movimiento.TipoMovimiento);
        Assert.Equal(-3m, movimiento.Cantidad);
        Assert.Equal("Conteo físico", movimiento.MotivoAjuste);
        Assert.Equal(usuarioId, movimiento.UsuarioId);
        Assert.Equal(fecha, movimiento.Fecha);
        Assert.Null(movimiento.OrigenTipo);
        Assert.Null(movimiento.OrigenId);
    }

    [Fact]
    public void CrearCompra_CantidadCero_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() =>
            MovimientoInventario.CrearCompra(Guid.NewGuid(), 0m, Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow));
    }

    [Fact]
    public void CrearCompra_DatosValidos_CreaMovimientoDeTipoCompraConOrigen()
    {
        var productoId = Guid.NewGuid();
        var compraId = Guid.NewGuid();
        var usuarioId = Guid.NewGuid();
        var fecha = DateTime.UtcNow;

        var movimiento = MovimientoInventario.CrearCompra(productoId, 10m, compraId, usuarioId, fecha);

        Assert.Equal(productoId, movimiento.ProductoId);
        Assert.Equal(TipoMovimientoInventario.Compra, movimiento.TipoMovimiento);
        Assert.Equal(10m, movimiento.Cantidad);
        Assert.Equal("Compra", movimiento.OrigenTipo);
        Assert.Equal(compraId, movimiento.OrigenId);
        Assert.Null(movimiento.MotivoAjuste);
    }

    [Fact]
    public void CrearConsumoTaller_CantidadCero_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() =>
            MovimientoInventario.CrearConsumoTaller(Guid.NewGuid(), 0m, Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow));
    }

    [Fact]
    public void CrearConsumoTaller_DatosValidos_CreaMovimientoDeSalidaConOrigenOrdenTrabajo()
    {
        var productoId = Guid.NewGuid();
        var ordenTrabajoId = Guid.NewGuid();
        var usuarioId = Guid.NewGuid();
        var fecha = DateTime.UtcNow;

        var movimiento = MovimientoInventario.CrearConsumoTaller(productoId, 3m, ordenTrabajoId, usuarioId, fecha);

        Assert.Equal(productoId, movimiento.ProductoId);
        Assert.Equal(TipoMovimientoInventario.ConsumoTaller, movimiento.TipoMovimiento);
        Assert.Equal(-3m, movimiento.Cantidad);
        Assert.Equal("OrdenTrabajo", movimiento.OrigenTipo);
        Assert.Equal(ordenTrabajoId, movimiento.OrigenId);
    }

    [Fact]
    public void CrearConsumoCampo_CantidadCero_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() =>
            MovimientoInventario.CrearConsumoCampo(Guid.NewGuid(), 0m, Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow));
    }

    [Fact]
    public void CrearConsumoCampo_DatosValidos_CreaMovimientoDeSalidaConOrigenServicioCampo()
    {
        var productoId = Guid.NewGuid();
        var servicioCampoId = Guid.NewGuid();
        var usuarioId = Guid.NewGuid();
        var fecha = DateTime.UtcNow;

        var movimiento = MovimientoInventario.CrearConsumoCampo(productoId, 3m, servicioCampoId, usuarioId, fecha);

        Assert.Equal(productoId, movimiento.ProductoId);
        Assert.Equal(TipoMovimientoInventario.ConsumoCampo, movimiento.TipoMovimiento);
        Assert.Equal(-3m, movimiento.Cantidad);
        Assert.Equal("ServicioCampo", movimiento.OrigenTipo);
        Assert.Equal(servicioCampoId, movimiento.OrigenId);
    }
}
