namespace ISARMIN.Application.Common.Excepciones;

public class NombreRolDuplicadoException : ExcepcionAplicacion
{
    public NombreRolDuplicadoException(string nombre)
        : base($"Ya existe un rol con el nombre '{nombre}'.")
    {
    }

    public override int CodigoHttp => 409;

    public override string Codigo => "NOMBRE_ROL_DUPLICADO";
}
