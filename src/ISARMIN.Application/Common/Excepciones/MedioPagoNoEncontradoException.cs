namespace ISARMIN.Application.Common.Excepciones;

public class MedioPagoNoEncontradoException : ExcepcionAplicacion
{
    public MedioPagoNoEncontradoException(Guid id)
        : base($"No se encontró el medio de pago '{id}'.")
    {
    }

    public override int CodigoHttp => 404;

    public override string Codigo => "MEDIO_PAGO_NO_ENCONTRADO";
}
