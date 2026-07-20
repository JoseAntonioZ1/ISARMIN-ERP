namespace ISARMIN.Application.Common.Excepciones;

public class VentaNoEncontradaException : ExcepcionAplicacion
{
    public VentaNoEncontradaException(Guid id)
        : base($"No se encontró la venta '{id}'.")
    {
    }

    public override int CodigoHttp => 404;

    public override string Codigo => "VENTA_NO_ENCONTRADA";
}
