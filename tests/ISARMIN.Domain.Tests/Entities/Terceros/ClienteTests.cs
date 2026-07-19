using ISARMIN.Domain.Entities.Terceros;
using ISARMIN.Domain.Enums;
using Xunit;

namespace ISARMIN.Domain.Tests.Entities.Terceros;

public class ClienteTests
{
    [Fact]
    public void Constructor_SinTelefono_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() => new Cliente("Juan Pérez", "", null, null, null));
    }

    [Fact]
    public void Constructor_SinNombre_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() => new Cliente("", "999888777", null, null, null));
    }

    [Fact]
    public void Constructor_SoloNombreYTelefono_CreaClienteSinDocumento()
    {
        var cliente = new Cliente("Juan Pérez", "999888777", null, null, null);

        Assert.Null(cliente.TipoDocumento);
        Assert.Null(cliente.TipoCliente);
        Assert.True(cliente.EstaActivo);
    }

    [Fact]
    public void Constructor_ConDniValido_DerivaTipoClienteNatural()
    {
        var cliente = new Cliente("Juan Pérez", "999888777", null, TipoDocumento.Dni, "12345678");

        Assert.Equal(TipoCliente.Natural, cliente.TipoCliente);
    }

    [Fact]
    public void Constructor_ConRucValido_DerivaTipoClienteJuridica()
    {
        var cliente = new Cliente("ISARMIN SAC", "999888777", null, TipoDocumento.Ruc, "20100070970");

        Assert.Equal(TipoCliente.Juridica, cliente.TipoCliente);
    }

    [Fact]
    public void Constructor_TipoDocumentoSinNumero_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() => new Cliente("Juan Pérez", "999888777", null, TipoDocumento.Dni, null));
    }

    [Fact]
    public void Constructor_NumeroDeDniInvalido_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() => new Cliente("Juan Pérez", "999888777", null, TipoDocumento.Dni, "123"));
    }

    [Fact]
    public void ActualizarDatos_QuitaDocumento_LimpiaTipoCliente()
    {
        var cliente = new Cliente("Juan Pérez", "999888777", null, TipoDocumento.Dni, "12345678");

        cliente.ActualizarDatos("Juan Pérez", "999888777", null, null, null);

        Assert.Null(cliente.TipoDocumento);
        Assert.Null(cliente.TipoCliente);
    }

    [Fact]
    public void Desactivar_LuegoActivar_RestauraElEstado()
    {
        var cliente = new Cliente("Juan Pérez", "999888777", null, null, null);

        cliente.Desactivar();
        Assert.False(cliente.EstaActivo);

        cliente.Activar();
        Assert.True(cliente.EstaActivo);
    }
}
