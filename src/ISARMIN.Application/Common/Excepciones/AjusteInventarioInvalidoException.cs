namespace ISARMIN.Application.Common.Excepciones;

public class AjusteInventarioInvalidoException : ExcepcionAplicacion
{
    public AjusteInventarioInvalidoException(Guid productoId)
        : base($"El ajuste dejaría el stock del producto '{productoId}' en un valor negativo.")
    {
    }

    public override int CodigoHttp => 400;

    public override string Codigo => "AJUSTE_INVENTARIO_INVALIDO";
}
