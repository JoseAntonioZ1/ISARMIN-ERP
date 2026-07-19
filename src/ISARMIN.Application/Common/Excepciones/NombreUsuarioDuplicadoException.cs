namespace ISARMIN.Application.Common.Excepciones;

/// <summary>RN-036 — nombre_usuario único.</summary>
public class NombreUsuarioDuplicadoException : ExcepcionAplicacion
{
    public NombreUsuarioDuplicadoException(string nombreUsuario)
        : base($"Ya existe un usuario con el nombre de usuario '{nombreUsuario}'.")
    {
    }

    public override int CodigoHttp => 409;

    public override string Codigo => "NOMBRE_USUARIO_DUPLICADO";
}
