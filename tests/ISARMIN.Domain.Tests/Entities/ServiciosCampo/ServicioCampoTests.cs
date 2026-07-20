using ISARMIN.Domain.Entities.ServiciosCampo;
using ISARMIN.Domain.Enums;
using Xunit;

namespace ISARMIN.Domain.Tests.Entities.ServiciosCampo;

public class ServicioCampoTests
{
    private static readonly DateTime Fecha = new(2026, 7, 20, 9, 0, 0, DateTimeKind.Utc);

    private static ServicioCampo CrearSolicitado() =>
        new(Guid.NewGuid(), "Instalación eléctrica en almacén", null, Fecha);

    [Fact]
    public void Constructor_ClienteVacio_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() => new ServicioCampo(Guid.Empty, "Trabajo", null, Fecha));
    }

    [Fact]
    public void Constructor_DatosValidos_CreaServicioEnEstadoSolicitado()
    {
        var servicio = CrearSolicitado();

        Assert.Equal(EstadoServicioCampo.Solicitado, servicio.Estado);
        Assert.Null(servicio.MontoEstimado);
    }

    [Fact]
    public void Cotizar_ServicioSolicitado_RegistraElMontoEstimado()
    {
        var servicio = CrearSolicitado();

        servicio.Cotizar(200m);

        Assert.Equal(200m, servicio.MontoEstimado);
        Assert.Equal(EstadoServicioCampo.Solicitado, servicio.Estado);
    }

    [Fact]
    public void Cotizar_MontoNegativo_LanzaExcepcion()
    {
        var servicio = CrearSolicitado();

        Assert.Throws<ArgumentException>(() => servicio.Cotizar(-1m));
    }

    [Fact]
    public void Cerrar_ServicioSolicitado_PasaACerradoConConsumos()
    {
        var servicio = CrearSolicitado();
        var productoId = Guid.NewGuid();

        servicio.Cerrar([(productoId, 3m)], "Conforme", "Cliente satisfecho", Guid.NewGuid(), Fecha);

        Assert.Equal(EstadoServicioCampo.Cerrado, servicio.Estado);
        Assert.Single(servicio.Detalles);
        Assert.Equal("Conforme", servicio.EstadoFinal);
    }

    [Fact]
    public void Cerrar_SinEstadoFinal_LanzaExcepcion()
    {
        var servicio = CrearSolicitado();

        Assert.Throws<ArgumentException>(() => servicio.Cerrar([], "", null, Guid.NewGuid(), Fecha));
    }

    [Fact]
    public void Cerrar_ServicioYaCerrado_LanzaExcepcion()
    {
        var servicio = CrearSolicitado();
        servicio.Cerrar([], "Conforme", null, Guid.NewGuid(), Fecha);

        Assert.Throws<InvalidOperationException>(() => servicio.Cerrar([], "Conforme", null, Guid.NewGuid(), Fecha));
    }

    [Fact]
    public void RegistrarCobro_ServicioCerrado_RegistraElPago()
    {
        var servicio = CrearSolicitado();
        servicio.Cerrar([], "Conforme", null, Guid.NewGuid(), Fecha);
        var medioPagoId = Guid.NewGuid();

        servicio.RegistrarCobro(medioPagoId, 200m, null, null);

        Assert.Equal(medioPagoId, servicio.MedioPagoId);
        Assert.Equal(200m, servicio.MontoPagado);
    }

    [Fact]
    public void RegistrarCobro_ServicioNoCerrado_LanzaExcepcion()
    {
        var servicio = CrearSolicitado();

        Assert.Throws<InvalidOperationException>(() => servicio.RegistrarCobro(Guid.NewGuid(), 200m, null, null));
    }

    [Fact]
    public void RegistrarCobro_SaldoPendienteSinUsuarioAutorizante_LanzaExcepcion()
    {
        var servicio = CrearSolicitado();
        servicio.Cerrar([], "Conforme", null, Guid.NewGuid(), Fecha);

        Assert.Throws<ArgumentException>(() => servicio.RegistrarCobro(Guid.NewGuid(), 100m, 100m, null));
    }

    [Fact]
    public void RegistrarCobro_SaldoPendienteConAutorizacion_RegistraElSaldo()
    {
        var servicio = CrearSolicitado();
        servicio.Cerrar([], "Conforme", null, Guid.NewGuid(), Fecha);
        var autorizanteId = Guid.NewGuid();

        servicio.RegistrarCobro(Guid.NewGuid(), 100m, 100m, autorizanteId);

        Assert.Equal(100m, servicio.SaldoPendiente);
        Assert.Equal(autorizanteId, servicio.UsuarioAutorizoSaldoId);
    }
}
