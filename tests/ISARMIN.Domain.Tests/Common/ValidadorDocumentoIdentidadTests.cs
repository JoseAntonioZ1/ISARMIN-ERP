using ISARMIN.Domain.Common;
using ISARMIN.Domain.Enums;
using Xunit;

namespace ISARMIN.Domain.Tests.Common;

public class ValidadorDocumentoIdentidadTests
{
    [Theory]
    [InlineData("12345678")]
    [InlineData("00000001")]
    public void EsValido_DniDeOchoDigitos_EsValido(string dni)
    {
        Assert.True(ValidadorDocumentoIdentidad.EsValido(TipoDocumento.Dni, dni));
    }

    [Theory]
    [InlineData("1234567")] // 7 dígitos
    [InlineData("123456789")] // 9 dígitos
    [InlineData("1234567A")] // no numérico
    public void EsValido_DniConFormatoInvalido_NoEsValido(string dni)
    {
        Assert.False(ValidadorDocumentoIdentidad.EsValido(TipoDocumento.Dni, dni));
    }

    [Fact]
    public void EsValido_RucRealDeSunat_EsValido()
    {
        // 20100070970 — RUC público de SUNAT, verificado manualmente contra el algoritmo módulo 11.
        Assert.True(ValidadorDocumentoIdentidad.EsValido(TipoDocumento.Ruc, "20100070970"));
    }

    [Theory]
    [InlineData("20100070971")] // dígito verificador incorrecto
    [InlineData("2010007097")] // 10 dígitos
    [InlineData("201000709700")] // 12 dígitos
    [InlineData("2010007097A")] // no numérico
    public void EsValido_RucConFormatoODigitoVerificadorInvalido_NoEsValido(string ruc)
    {
        Assert.False(ValidadorDocumentoIdentidad.EsValido(TipoDocumento.Ruc, ruc));
    }

    [Fact]
    public void EsValido_CarneExtranjeriaAlfanumerico_EsValido()
    {
        Assert.True(ValidadorDocumentoIdentidad.EsValido(TipoDocumento.CarneExtranjeria, "AB123456"));
    }

    [Fact]
    public void EsValido_NumeroVacio_NoEsValido()
    {
        Assert.False(ValidadorDocumentoIdentidad.EsValido(TipoDocumento.Dni, ""));
    }
}
