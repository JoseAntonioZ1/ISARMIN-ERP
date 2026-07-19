namespace ISARMIN.Application.Common.Excepciones;

public class UsuarioNoEncontradoException : ExcepcionAplicacion
{
    public UsuarioNoEncontradoException(Guid id)
        : base($"No se encontró el usuario '{id}'.")
    {
    }

    public override int CodigoHttp => 404;

    public override string Codigo => "USUARIO_NO_ENCONTRADO";
}
