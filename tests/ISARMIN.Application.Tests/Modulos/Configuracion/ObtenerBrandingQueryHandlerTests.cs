using ISARMIN.Application.Modulos.Configuracion;
using ISARMIN.Application.Modulos.Configuracion.Queries.ObtenerBranding;
using ISARMIN.Domain.Entities.Configuracion;
using Moq;
using Xunit;

namespace ISARMIN.Application.Tests.Modulos.Configuracion;

public class ObtenerBrandingQueryHandlerTests
{
    [Fact]
    public async Task ManejarAsync_DevuelveIdentidadVisualSinDatosOperativos()
    {
        var configuracion = new ConfiguracionEmpresa("ISARMIN PERÚ S.A.C.");
        configuracion.Actualizar(
            "ISARMIN PERÚ S.A.C.", "20123456789", "Av. Principal 123", "https://ejemplo.com/logo.png", 100m, "#1E293B", "Bienvenido al equipo");

        var repositorio = new Mock<IConfiguracionEmpresaRepository>();
        repositorio.Setup(r => r.ObtenerAsync(It.IsAny<CancellationToken>())).ReturnsAsync(configuracion);

        var handler = new ObtenerBrandingQueryHandler(repositorio.Object);
        var resultado = await handler.ManejarAsync(new ObtenerBrandingQuery());

        Assert.Equal("ISARMIN PERÚ S.A.C.", resultado.RazonSocial);
        Assert.Equal("20123456789", resultado.Ruc);
        Assert.Equal("Av. Principal 123", resultado.Direccion);
        Assert.Equal("https://ejemplo.com/logo.png", resultado.Logo);
        Assert.Equal("#1E293B", resultado.ColorAcento);
        Assert.Equal("Bienvenido al equipo", resultado.MensajeBienvenida);
    }
}
