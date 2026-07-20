namespace ISARMIN.Application.Common.Excepciones;

/// <summary>La OT no está en el estado requerido para la acción solicitada (máquina de estados de UC-22 a UC-26).</summary>
public class EstadoOrdenTrabajoInvalidoException : ExcepcionAplicacion
{
    public EstadoOrdenTrabajoInvalidoException(Guid ordenTrabajoId, string mensaje)
        : base(mensaje)
    {
        OrdenTrabajoId = ordenTrabajoId;
    }

    public Guid OrdenTrabajoId { get; }

    public override int CodigoHttp => 409;

    public override string Codigo => "ESTADO_ORDEN_TRABAJO_INVALIDO";
}
