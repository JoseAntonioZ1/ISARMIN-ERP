namespace ISARMIN.Application.Common.Excepciones;

public class ServicioCampoNoEncontradoException : ExcepcionAplicacion
{
    public ServicioCampoNoEncontradoException(Guid id)
        : base($"No se encontró el servicio de campo '{id}'.")
    {
    }

    public override int CodigoHttp => 404;

    public override string Codigo => "SERVICIO_CAMPO_NO_ENCONTRADO";
}
