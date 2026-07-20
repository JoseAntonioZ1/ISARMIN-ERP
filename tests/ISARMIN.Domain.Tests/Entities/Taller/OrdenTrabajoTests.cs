using ISARMIN.Domain.Entities.Taller;
using ISARMIN.Domain.Enums;
using Xunit;

namespace ISARMIN.Domain.Tests.Entities.Taller;

public class OrdenTrabajoTests
{
    private static readonly DateTime Fecha = new(2026, 7, 20, 9, 0, 0, DateTimeKind.Utc);

    private static OrdenTrabajo CrearOtRecibida() =>
        new(Guid.NewGuid(), "Taladro Bosch 1/2", "No enciende", Guid.NewGuid(), Fecha);

    [Fact]
    public void Constructor_ClienteVacio_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() => new OrdenTrabajo(Guid.Empty, "Taladro", "No enciende", Guid.NewGuid(), Fecha));
    }

    [Fact]
    public void Constructor_DatosValidos_CreaOtEnEstadoRecibido()
    {
        var ot = CrearOtRecibida();

        Assert.Equal(EstadoOrdenTrabajo.Recibido, ot.Estado);
        Assert.Null(ot.Diagnostico);
        Assert.Null(ot.CotizacionReparacion);
    }

    [Fact]
    public void RegistrarDiagnostico_OtRecibida_PasaADiagnosticado()
    {
        var ot = CrearOtRecibida();

        ot.RegistrarDiagnostico("Motor quemado", Guid.NewGuid(), Fecha);

        Assert.Equal(EstadoOrdenTrabajo.Diagnosticado, ot.Estado);
        Assert.NotNull(ot.Diagnostico);
    }

    [Fact]
    public void RegistrarDiagnostico_OtNoRecibida_LanzaExcepcion()
    {
        var ot = CrearOtRecibida();
        ot.RegistrarDiagnostico("Motor quemado", Guid.NewGuid(), Fecha);

        Assert.Throws<InvalidOperationException>(() => ot.RegistrarDiagnostico("Otro diagnóstico", Guid.NewGuid(), Fecha));
    }

    [Fact]
    public void GenerarCotizacion_OtDiagnosticada_PasaACotizado()
    {
        var ot = CrearOtRecibida();
        ot.RegistrarDiagnostico("Motor quemado", Guid.NewGuid(), Fecha);

        ot.GenerarCotizacion(150m, Fecha);

        Assert.Equal(EstadoOrdenTrabajo.Cotizado, ot.Estado);
        Assert.Equal(150m, ot.CotizacionReparacion!.MontoEstimado);
    }

    [Fact]
    public void GenerarCotizacion_OtNoDiagnosticada_LanzaExcepcion()
    {
        var ot = CrearOtRecibida();

        Assert.Throws<InvalidOperationException>(() => ot.GenerarCotizacion(150m, Fecha));
    }

    [Fact]
    public void RegistrarDecisionCliente_Aprobada_PasaAAprobado()
    {
        var ot = CrearOtRecibida();
        ot.RegistrarDiagnostico("Motor quemado", Guid.NewGuid(), Fecha);
        ot.GenerarCotizacion(150m, Fecha);

        ot.RegistrarDecisionCliente(DecisionCliente.Aprobada, null, "Firma física");

        Assert.Equal(EstadoOrdenTrabajo.Aprobado, ot.Estado);
    }

    [Fact]
    public void RegistrarDecisionCliente_Rechazada_PasaARechazadoYPermiteCobroDiagnostico()
    {
        var ot = CrearOtRecibida();
        ot.RegistrarDiagnostico("Motor quemado", Guid.NewGuid(), Fecha);
        ot.GenerarCotizacion(150m, Fecha);

        ot.RegistrarDecisionCliente(DecisionCliente.Rechazada, 20m, null);

        Assert.Equal(EstadoOrdenTrabajo.Rechazado, ot.Estado);
        Assert.Equal(20m, ot.CotizacionReparacion!.CobroDiagnosticoRechazo);
    }

    [Fact]
    public void RegistrarDecisionCliente_SinCotizacion_LanzaExcepcion()
    {
        var ot = CrearOtRecibida();
        ot.RegistrarDiagnostico("Motor quemado", Guid.NewGuid(), Fecha);

        Assert.Throws<InvalidOperationException>(() => ot.RegistrarDecisionCliente(DecisionCliente.Aprobada, null, null));
    }

    private static OrdenTrabajo CrearOtAprobada()
    {
        var ot = CrearOtRecibida();
        ot.RegistrarDiagnostico("Motor quemado", Guid.NewGuid(), Fecha);
        ot.GenerarCotizacion(150m, Fecha);
        ot.RegistrarDecisionCliente(DecisionCliente.Aprobada, null, null);
        return ot;
    }

    [Fact]
    public void RegistrarReparacion_OtAprobada_PasaAListoParaEntregaConConsumos()
    {
        var ot = CrearOtAprobada();
        var productoId = Guid.NewGuid();

        ot.RegistrarReparacion([(productoId, 2m)], "Prueba de encendido OK");

        Assert.Equal(EstadoOrdenTrabajo.ListoParaEntrega, ot.Estado);
        Assert.Single(ot.ConsumosRepuesto);
        Assert.Equal("Prueba de encendido OK", ot.ResultadoPruebas);
    }

    [Fact]
    public void RegistrarReparacion_OtNoAprobada_LanzaExcepcion()
    {
        var ot = CrearOtRecibida();

        Assert.Throws<InvalidOperationException>(() => ot.RegistrarReparacion([], null));
    }

    private static OrdenTrabajo CrearOtListaParaEntrega()
    {
        var ot = CrearOtAprobada();
        ot.RegistrarReparacion([], "OK");
        return ot;
    }

    [Fact]
    public void EntregarEquipo_PagoCompleto_PasaAEntregado()
    {
        var ot = CrearOtListaParaEntrega();

        ot.EntregarEquipo(EstadoPagoOrdenTrabajo.CompletoAlMomento, 150m, null, null, Guid.NewGuid(), Fecha);

        Assert.Equal(EstadoOrdenTrabajo.Entregado, ot.Estado);
        Assert.Equal(150m, ot.MontoPagado);
    }

    [Fact]
    public void EntregarEquipo_SaldoPendienteSinUsuarioAutorizante_LanzaExcepcion()
    {
        var ot = CrearOtListaParaEntrega();

        Assert.Throws<ArgumentException>(() =>
            ot.EntregarEquipo(EstadoPagoOrdenTrabajo.SaldoPendiente, 50m, 100m, null, Guid.NewGuid(), Fecha));
    }

    [Fact]
    public void EntregarEquipo_SaldoPendienteConAutorizacion_RegistraElSaldo()
    {
        var ot = CrearOtListaParaEntrega();
        var autorizanteId = Guid.NewGuid();

        ot.EntregarEquipo(EstadoPagoOrdenTrabajo.SaldoPendiente, 50m, 100m, autorizanteId, Guid.NewGuid(), Fecha);

        Assert.Equal(EstadoOrdenTrabajo.Entregado, ot.Estado);
        Assert.Equal(100m, ot.SaldoPendiente);
        Assert.Equal(autorizanteId, ot.UsuarioAutorizoSaldoId);
    }

    [Fact]
    public void EntregarEquipo_OtNoListaParaEntrega_LanzaExcepcion()
    {
        var ot = CrearOtRecibida();

        Assert.Throws<InvalidOperationException>(() =>
            ot.EntregarEquipo(EstadoPagoOrdenTrabajo.CompletoAlMomento, 0m, null, null, Guid.NewGuid(), Fecha));
    }
}
