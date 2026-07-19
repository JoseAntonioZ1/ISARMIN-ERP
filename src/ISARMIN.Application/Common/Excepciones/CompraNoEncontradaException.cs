namespace ISARMIN.Application.Common.Excepciones;

public class CompraNoEncontradaException : ExcepcionAplicacion
{
    public CompraNoEncontradaException(Guid id)
        : base($"No se encontró la compra '{id}'.")
    {
    }

    public override int CodigoHttp => 404;

    public override string Codigo => "COMPRA_NO_ENCONTRADA";
}
