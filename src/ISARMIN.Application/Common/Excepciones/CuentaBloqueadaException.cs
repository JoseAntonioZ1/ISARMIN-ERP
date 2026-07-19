namespace ISARMIN.Application.Common.Excepciones;

public class CuentaBloqueadaException : ExcepcionAplicacion
{
    public DateTime BloqueadoHastaUtc { get; }

    public CuentaBloqueadaException(DateTime bloqueadoHastaUtc)
        : base($"Cuenta bloqueada por intentos fallidos. Intente nuevamente después de las {bloqueadoHastaUtc:HH:mm} UTC.")
    {
        BloqueadoHastaUtc = bloqueadoHastaUtc;
    }

    public override int CodigoHttp => 423;

    public override string Codigo => "CUENTA_BLOQUEADA_TEMPORALMENTE";
}
