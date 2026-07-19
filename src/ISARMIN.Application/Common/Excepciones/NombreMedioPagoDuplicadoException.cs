namespace ISARMIN.Application.Common.Excepciones;

public class NombreMedioPagoDuplicadoException : ExcepcionAplicacion
{
    public NombreMedioPagoDuplicadoException(string nombre)
        : base($"Ya existe un medio de pago con el nombre '{nombre}'.")
    {
    }

    public override int CodigoHttp => 409;

    public override string Codigo => "NOMBRE_MEDIO_PAGO_DUPLICADO";
}
