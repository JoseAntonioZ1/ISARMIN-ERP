namespace ISARMIN.Application.Common.Excepciones;

public class RefreshTokenInvalidoException : ExcepcionAplicacion
{
    public RefreshTokenInvalidoException()
        : base("La sesión expiró o no es válida. Vuelve a iniciar sesión.")
    {
    }

    public override int CodigoHttp => 401;

    public override string Codigo => "REFRESH_TOKEN_INVALIDO";
}
