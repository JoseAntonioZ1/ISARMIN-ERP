using ISARMIN.Domain.Entities.Configuracion;
using Xunit;

namespace ISARMIN.Domain.Tests.Entities.Configuracion;

public class MedioPagoTests
{
    [Fact]
    public void Constructor_CreaActivoPorDefecto()
    {
        var medioPago = new MedioPago("Efectivo");

        Assert.True(medioPago.Activo);
    }

    [Fact]
    public void Desactivar_LuegoActivar_RestauraElEstado()
    {
        var medioPago = new MedioPago("Efectivo");

        medioPago.Desactivar();
        Assert.False(medioPago.Activo);

        medioPago.Activar();
        Assert.True(medioPago.Activo);
    }

    [Fact]
    public void Constructor_NombreVacio_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() => new MedioPago(""));
    }
}
