using CajaEntity = ISARMIN.Domain.Entities.Caja.Caja;
using ISARMIN.Domain.Enums;
using Xunit;

namespace ISARMIN.Domain.Tests.Entities.Caja;

public class CajaTests
{
    private static readonly DateTime FechaApertura = new(2026, 7, 20, 8, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Constructor_MontoAperturaNegativo_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() => new CajaEntity(-1m, Guid.NewGuid(), FechaApertura));
    }

    [Fact]
    public void Constructor_DatosValidos_CreaLaCajaAbierta()
    {
        var caja = new CajaEntity(100m, Guid.NewGuid(), FechaApertura);

        Assert.True(caja.EstaAbierta);
        Assert.Equal(EstadoCaja.Abierta, caja.Estado);
        Assert.Equal(100m, caja.MontoApertura);
        Assert.Null(caja.FechaCierre);
        Assert.Null(caja.Diferencia);
    }

    [Fact]
    public void Cerrar_CajaAbierta_CalculaDiferenciaYCambiaEstado()
    {
        var caja = new CajaEntity(100m, Guid.NewGuid(), FechaApertura);
        var fechaCierre = FechaApertura.AddHours(8);

        caja.Cerrar(500m, 480m, fechaCierre);

        Assert.False(caja.EstaAbierta);
        Assert.Equal(EstadoCaja.Cerrada, caja.Estado);
        Assert.Equal(500m, caja.MontoTeoricoCierre);
        Assert.Equal(480m, caja.MontoFisicoDeclarado);
        Assert.Equal(-20m, caja.Diferencia);
        Assert.Equal(fechaCierre, caja.FechaCierre);
    }

    [Fact]
    public void Cerrar_CajaYaCerrada_LanzaExcepcion()
    {
        var caja = new CajaEntity(100m, Guid.NewGuid(), FechaApertura);
        caja.Cerrar(500m, 500m, FechaApertura.AddHours(8));

        Assert.Throws<InvalidOperationException>(() => caja.Cerrar(500m, 500m, FechaApertura.AddHours(9)));
    }

    [Fact]
    public void Cerrar_MontoFisicoNegativo_LanzaExcepcion()
    {
        var caja = new CajaEntity(100m, Guid.NewGuid(), FechaApertura);

        Assert.Throws<ArgumentException>(() => caja.Cerrar(500m, -1m, FechaApertura.AddHours(8)));
    }
}
