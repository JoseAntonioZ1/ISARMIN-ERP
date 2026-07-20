using ISARMIN.Domain.Entities.Caja;
using ISARMIN.Domain.Enums;
using Xunit;

namespace ISARMIN.Domain.Tests.Entities.Caja;

public class MovimientoCajaTests
{
    [Fact]
    public void Registrar_MontoCero_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() =>
            MovimientoCaja.Registrar(Guid.NewGuid(), ConceptoMovimientoCaja.GastoOperativo, 0m, null, Guid.NewGuid(), DateTime.UtcNow));
    }

    [Fact]
    public void Registrar_GastoOperativo_EsSiempreEgreso()
    {
        var movimiento = MovimientoCaja.Registrar(
            Guid.NewGuid(), ConceptoMovimientoCaja.GastoOperativo, 50m, "Pago de luz", Guid.NewGuid(), DateTime.UtcNow);

        Assert.Equal(TipoMovimientoCaja.Egreso, movimiento.Tipo);
        Assert.Equal(ConceptoMovimientoCaja.GastoOperativo, movimiento.Concepto);
    }

    [Fact]
    public void Registrar_RetiroPropietario_EsSiempreEgreso()
    {
        var movimiento = MovimientoCaja.Registrar(
            Guid.NewGuid(), ConceptoMovimientoCaja.RetiroPropietario, 200m, null, Guid.NewGuid(), DateTime.UtcNow);

        Assert.Equal(TipoMovimientoCaja.Egreso, movimiento.Tipo);
    }

    [Fact]
    public void Registrar_AporteCapital_EsSiempreIngreso()
    {
        var movimiento = MovimientoCaja.Registrar(
            Guid.NewGuid(), ConceptoMovimientoCaja.AporteCapital, 1000m, null, Guid.NewGuid(), DateTime.UtcNow);

        Assert.Equal(TipoMovimientoCaja.Ingreso, movimiento.Tipo);
    }
}
