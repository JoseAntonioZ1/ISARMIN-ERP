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
}
