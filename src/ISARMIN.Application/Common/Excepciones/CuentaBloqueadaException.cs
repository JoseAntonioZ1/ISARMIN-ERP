namespace ISARMIN.Application.Common.Excepciones;

public class CuentaBloqueadaException : Exception
{
    public DateTime BloqueadoHastaUtc { get; }

    public CuentaBloqueadaException(DateTime bloqueadoHastaUtc)
        : base("La cuenta está bloqueada temporalmente por intentos fallidos de autenticación.")
    {
        BloqueadoHastaUtc = bloqueadoHastaUtc;
    }
}
