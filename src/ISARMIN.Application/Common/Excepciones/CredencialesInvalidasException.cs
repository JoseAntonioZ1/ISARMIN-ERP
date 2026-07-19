namespace ISARMIN.Application.Common.Excepciones;

public class CredencialesInvalidasException : ExcepcionAplicacion
{
    public CredencialesInvalidasException()
        : base("El nombre de usuario o la credencial son incorrectos.")
    {
    }

    public override int CodigoHttp => 401;

    public override string Codigo => "CREDENCIALES_INVALIDAS";
}
