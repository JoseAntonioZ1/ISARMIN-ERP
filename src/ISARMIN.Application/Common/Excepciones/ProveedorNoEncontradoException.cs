namespace ISARMIN.Application.Common.Excepciones;

public class ProveedorNoEncontradoException : ExcepcionAplicacion
{
    public ProveedorNoEncontradoException(Guid id)
        : base($"No se encontró el proveedor '{id}'.")
    {
    }

    public override int CodigoHttp => 404;

    public override string Codigo => "PROVEEDOR_NO_ENCONTRADO";
}
