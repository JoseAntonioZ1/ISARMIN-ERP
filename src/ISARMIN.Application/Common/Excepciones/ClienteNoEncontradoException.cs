namespace ISARMIN.Application.Common.Excepciones;

public class ClienteNoEncontradoException : ExcepcionAplicacion
{
    public ClienteNoEncontradoException(Guid id)
        : base($"No se encontró el cliente '{id}'.")
    {
    }

    public override int CodigoHttp => 404;

    public override string Codigo => "CLIENTE_NO_ENCONTRADO";
}
