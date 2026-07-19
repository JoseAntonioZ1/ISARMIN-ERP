using ISARMIN.Domain.Entities.Terceros;
using Xunit;

namespace ISARMIN.Domain.Tests.Entities.Terceros;

public class ProveedorTests
{
    [Fact]
    public void Constructor_SinNombre_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() => new Proveedor("", null, null, null));
    }

    [Fact]
    public void Constructor_SoloNombre_CreaProveedorActivo()
    {
        var proveedor = new Proveedor("Distribuidora ACME", null, null, null);

        Assert.True(proveedor.EstaActivo);
        Assert.Null(proveedor.Documento);
    }

    [Fact]
    public void ActualizarDatos_CambiaLosCampos()
    {
        var proveedor = new Proveedor("Distribuidora ACME", null, null, null);

        proveedor.ActualizarDatos("ACME SAC", "20100070970", "999888777", "Av. Industrial 456");

        Assert.Equal("ACME SAC", proveedor.NombreRazonSocial);
        Assert.Equal("20100070970", proveedor.Documento);
        Assert.Equal("999888777", proveedor.Telefono);
        Assert.Equal("Av. Industrial 456", proveedor.Direccion);
    }

    [Fact]
    public void Desactivar_LuegoActivar_RestauraElEstado()
    {
        var proveedor = new Proveedor("Distribuidora ACME", null, null, null);

        proveedor.Desactivar();
        Assert.False(proveedor.EstaActivo);

        proveedor.Activar();
        Assert.True(proveedor.EstaActivo);
    }
}
