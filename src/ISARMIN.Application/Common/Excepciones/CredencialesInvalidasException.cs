namespace ISARMIN.Application.Common.Excepciones;

public class CredencialesInvalidasException : Exception
{
    public CredencialesInvalidasException()
        : base("El nombre de usuario o la credencial son incorrectos.")
    {
    }
}
