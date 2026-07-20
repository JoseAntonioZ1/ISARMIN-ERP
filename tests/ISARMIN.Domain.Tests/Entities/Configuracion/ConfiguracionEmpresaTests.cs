using ISARMIN.Domain.Entities.Configuracion;
using Xunit;

namespace ISARMIN.Domain.Tests.Entities.Configuracion;

public class ConfiguracionEmpresaTests
{
    [Fact]
    public void Constructor_RazonSocialVacia_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() => new ConfiguracionEmpresa("  "));
    }

    [Fact]
    public void Constructor_DatosValidos_CreaLaConfiguracion()
    {
        var configuracion = new ConfiguracionEmpresa("ISARMIN PERÚ S.A.C.");

        Assert.Equal("ISARMIN PERÚ S.A.C.", configuracion.RazonSocial);
        Assert.Null(configuracion.Ruc);
        Assert.Null(configuracion.MontoAperturaCajaPredeterminado);
        Assert.Null(configuracion.ColorAcento);
        Assert.Null(configuracion.MensajeBienvenida);
    }

    [Fact]
    public void Actualizar_DatosValidos_ActualizaTodosLosCampos()
    {
        var configuracion = new ConfiguracionEmpresa("ISARMIN PERÚ S.A.C.");

        configuracion.Actualizar(
            "ISARMIN PERÚ S.A.C.", "20123456789", "Av. Principal 123", "https://ejemplo.com/logo.png", 100m, "#1E293B", "Bienvenido al equipo");

        Assert.Equal("20123456789", configuracion.Ruc);
        Assert.Equal("Av. Principal 123", configuracion.Direccion);
        Assert.Equal("https://ejemplo.com/logo.png", configuracion.Logo);
        Assert.Equal(100m, configuracion.MontoAperturaCajaPredeterminado);
        Assert.Equal("#1E293B", configuracion.ColorAcento);
        Assert.Equal("Bienvenido al equipo", configuracion.MensajeBienvenida);
    }

    [Fact]
    public void Actualizar_RazonSocialVacia_LanzaExcepcion()
    {
        var configuracion = new ConfiguracionEmpresa("ISARMIN PERÚ S.A.C.");

        Assert.Throws<ArgumentException>(() => configuracion.Actualizar("", null, null, null, null, null, null));
    }

    [Fact]
    public void Actualizar_MontoAperturaNegativo_LanzaExcepcion()
    {
        var configuracion = new ConfiguracionEmpresa("ISARMIN PERÚ S.A.C.");

        Assert.Throws<ArgumentException>(() => configuracion.Actualizar("ISARMIN PERÚ S.A.C.", null, null, null, -10m, null, null));
    }

    [Fact]
    public void Actualizar_MontoAperturaNulo_LimpiaElValorPrevio()
    {
        var configuracion = new ConfiguracionEmpresa("ISARMIN PERÚ S.A.C.");
        configuracion.Actualizar("ISARMIN PERÚ S.A.C.", null, null, null, 100m, null, null);

        configuracion.Actualizar("ISARMIN PERÚ S.A.C.", null, null, null, null, null, null);

        Assert.Null(configuracion.MontoAperturaCajaPredeterminado);
    }

    [Theory]
    [InlineData("1E293B")]
    [InlineData("#1E293")]
    [InlineData("#GGGGGG")]
    [InlineData("azul")]
    public void Actualizar_ColorAcentoConFormatoInvalido_LanzaExcepcion(string colorInvalido)
    {
        var configuracion = new ConfiguracionEmpresa("ISARMIN PERÚ S.A.C.");

        Assert.Throws<ArgumentException>(() => configuracion.Actualizar("ISARMIN PERÚ S.A.C.", null, null, null, null, colorInvalido, null));
    }
}
