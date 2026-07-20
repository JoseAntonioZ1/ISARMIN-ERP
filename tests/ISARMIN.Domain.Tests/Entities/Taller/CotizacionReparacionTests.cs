using ISARMIN.Domain.Entities.Taller;
using ISARMIN.Domain.Enums;
using Xunit;

namespace ISARMIN.Domain.Tests.Entities.Taller;

public class CotizacionReparacionTests
{
    [Fact]
    public void Constructor_MontoNegativo_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() => new CotizacionReparacion(Guid.NewGuid(), -1m, DateTime.UtcNow));
    }

    [Fact]
    public void RegistrarDecision_AprobadaConCobroDiagnostico_LanzaExcepcion()
    {
        var cotizacion = new CotizacionReparacion(Guid.NewGuid(), 100m, DateTime.UtcNow);

        Assert.Throws<ArgumentException>(() => cotizacion.RegistrarDecision(DecisionCliente.Aprobada, 20m, null));
    }

    [Fact]
    public void RegistrarDecision_YaRegistrada_LanzaExcepcion()
    {
        var cotizacion = new CotizacionReparacion(Guid.NewGuid(), 100m, DateTime.UtcNow);
        cotizacion.RegistrarDecision(DecisionCliente.Aprobada, null, null);

        Assert.Throws<InvalidOperationException>(() => cotizacion.RegistrarDecision(DecisionCliente.Rechazada, null, null));
    }
}
