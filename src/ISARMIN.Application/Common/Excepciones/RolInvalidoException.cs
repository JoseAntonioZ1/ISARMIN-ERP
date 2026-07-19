namespace ISARMIN.Application.Common.Excepciones;

public class RolInvalidoException : ExcepcionAplicacion
{
    public RolInvalidoException(Guid rolId)
        : base($"El rol '{rolId}' no existe.")
    {
    }

    public override int CodigoHttp => 400;

    public override string Codigo => "ROL_INVALIDO";
}
