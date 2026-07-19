namespace ISARMIN.Application.Common.Excepciones;

public class RolNoEncontradoException : ExcepcionAplicacion
{
    public RolNoEncontradoException(Guid id)
        : base($"No se encontró el rol '{id}'.")
    {
    }

    public override int CodigoHttp => 404;

    public override string Codigo => "ROL_NO_ENCONTRADO";
}
