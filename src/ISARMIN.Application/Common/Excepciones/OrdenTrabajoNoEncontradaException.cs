namespace ISARMIN.Application.Common.Excepciones;

public class OrdenTrabajoNoEncontradaException : ExcepcionAplicacion
{
    public OrdenTrabajoNoEncontradaException(Guid id)
        : base($"No se encontró la orden de trabajo '{id}'.")
    {
    }

    public override int CodigoHttp => 404;

    public override string Codigo => "ORDEN_TRABAJO_NO_ENCONTRADA";
}
