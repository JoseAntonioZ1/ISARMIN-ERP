using ISARMIN.Domain.Entities.Taller;
using Xunit;

namespace ISARMIN.Domain.Tests.Entities.Taller;

public class GarantiaTests
{
    [Fact]
    public void Constructor_FechaFinAnteriorOIgual_LanzaExcepcion()
    {
        var fecha = new DateOnly(2026, 7, 20);

        Assert.Throws<ArgumentException>(() => new Garantia(Guid.NewGuid(), fecha, fecha));
        Assert.Throws<ArgumentException>(() => new Garantia(Guid.NewGuid(), fecha, fecha.AddDays(-1)));
    }

    [Fact]
    public void Constructor_DatosValidos_CreaLaGarantia()
    {
        var otId = Guid.NewGuid();
        var inicio = new DateOnly(2026, 7, 20);
        var fin = inicio.AddDays(90);

        var garantia = new Garantia(otId, inicio, fin);

        Assert.Equal(otId, garantia.OrdenTrabajoId);
        Assert.Equal(inicio, garantia.FechaInicio);
        Assert.Equal(fin, garantia.FechaFin);
        Assert.Null(garantia.OrdenTrabajoReingresoId);
    }
}
