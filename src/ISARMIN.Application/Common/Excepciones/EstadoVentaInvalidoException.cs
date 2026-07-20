namespace ISARMIN.Application.Common.Excepciones;

/// <summary>La venta no está en el estado requerido para la acción solicitada (UC-14, UC-16).</summary>
public class EstadoVentaInvalidoException : ExcepcionAplicacion
{
    public EstadoVentaInvalidoException(Guid ventaId, string mensaje)
        : base(mensaje)
    {
        VentaId = ventaId;
    }

    public Guid VentaId { get; }

    public override int CodigoHttp => 409;

    public override string Codigo => "ESTADO_VENTA_INVALIDO";
}
