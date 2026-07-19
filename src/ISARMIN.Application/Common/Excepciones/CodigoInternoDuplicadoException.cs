namespace ISARMIN.Application.Common.Excepciones;

public class CodigoInternoDuplicadoException : ExcepcionAplicacion
{
    public CodigoInternoDuplicadoException(string codigoInterno)
        : base($"Ya existe un producto con el código interno '{codigoInterno}'.")
    {
    }

    public override int CodigoHttp => 409;

    public override string Codigo => "CODIGO_INTERNO_DUPLICADO";
}
