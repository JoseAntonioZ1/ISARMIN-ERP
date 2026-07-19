using ISARMIN.Domain.Entities.Inventario;
using Xunit;

namespace ISARMIN.Domain.Tests.Entities.Inventario;

public class UnidadMedidaTests
{
    [Fact]
    public void Constructor_NombreVacio_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() => new UnidadMedida(""));
    }

    [Fact]
    public void ActualizarDatos_NombreValido_ActualizaNombre()
    {
        var unidadMedida = new UnidadMedida("Unidad");

        unidadMedida.ActualizarDatos("Metro");

        Assert.Equal("Metro", unidadMedida.Nombre);
    }

    [Fact]
    public void ActualizarDatos_NombreVacio_LanzaExcepcion()
    {
        var unidadMedida = new UnidadMedida("Unidad");

        Assert.Throws<ArgumentException>(() => unidadMedida.ActualizarDatos(""));
    }
}
