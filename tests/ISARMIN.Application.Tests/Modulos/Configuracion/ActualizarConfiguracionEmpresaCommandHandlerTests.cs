using FluentValidation;
using ISARMIN.Application.Modulos.Configuracion;
using ISARMIN.Application.Modulos.Configuracion.Commands.ActualizarConfiguracionEmpresa;
using ISARMIN.Domain.Entities.Configuracion;
using Moq;
using Xunit;

namespace ISARMIN.Application.Tests.Modulos.Configuracion;

public class ActualizarConfiguracionEmpresaCommandHandlerTests
{
    private readonly Mock<IConfiguracionEmpresaRepository> _configuracionEmpresaRepository = new();
    private readonly ActualizarConfiguracionEmpresaCommandValidator _validator = new();

    private ActualizarConfiguracionEmpresaCommandHandler CrearHandler() => new(_configuracionEmpresaRepository.Object, _validator);

    [Fact]
    public async Task ManejarAsync_DatosValidos_ActualizaLaConfiguracion()
    {
        var configuracion = new ConfiguracionEmpresa("ISARMIN PERÚ S.A.C.");
        _configuracionEmpresaRepository.Setup(r => r.ObtenerAsync(It.IsAny<CancellationToken>())).ReturnsAsync(configuracion);

        var handler = CrearHandler();
        var comando = new ActualizarConfiguracionEmpresaCommand(
            "ISARMIN PERÚ S.A.C.", "20123456789", "Av. Principal 123", null, 150m, "#1E293B", "Bienvenido al equipo");
        var resultado = await handler.ManejarAsync(comando);

        Assert.Equal("20123456789", resultado.Ruc);
        Assert.Equal(150m, resultado.MontoAperturaCajaPredeterminado);
        Assert.Equal("#1E293B", resultado.ColorAcento);
        Assert.Equal("Bienvenido al equipo", resultado.MensajeBienvenida);
        _configuracionEmpresaRepository.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ManejarAsync_RazonSocialVacia_FallaValidacion()
    {
        var handler = CrearHandler();
        var comando = new ActualizarConfiguracionEmpresaCommand("", null, null, null, null, null, null);

        await Assert.ThrowsAsync<ValidationException>(() => handler.ManejarAsync(comando));
    }

    [Fact]
    public async Task ManejarAsync_MontoAperturaNegativo_FallaValidacion()
    {
        var handler = CrearHandler();
        var comando = new ActualizarConfiguracionEmpresaCommand("ISARMIN PERÚ S.A.C.", null, null, null, -50m, null, null);

        await Assert.ThrowsAsync<ValidationException>(() => handler.ManejarAsync(comando));
    }

    [Fact]
    public async Task ManejarAsync_ColorAcentoConFormatoInvalido_FallaValidacion()
    {
        var handler = CrearHandler();
        var comando = new ActualizarConfiguracionEmpresaCommand("ISARMIN PERÚ S.A.C.", null, null, null, null, "azul", null);

        await Assert.ThrowsAsync<ValidationException>(() => handler.ManejarAsync(comando));
    }
}
