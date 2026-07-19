namespace ISARMIN.Application.Common.Excepciones;

public class ProductoNoEncontradoException : ExcepcionAplicacion
{
    public ProductoNoEncontradoException(Guid id)
        : base($"No se encontró el producto '{id}'.")
    {
    }

    public override int CodigoHttp => 404;

    public override string Codigo => "PRODUCTO_NO_ENCONTRADO";
}
