namespace ISARMIN.Application.Common.Excepciones;

public class NombreCategoriaDuplicadoException : ExcepcionAplicacion
{
    public NombreCategoriaDuplicadoException(string nombre)
        : base($"Ya existe una categoría con el nombre '{nombre}'.")
    {
    }

    public override int CodigoHttp => 409;

    public override string Codigo => "NOMBRE_CATEGORIA_DUPLICADO";
}
