namespace ISARMIN.Application.Common.Excepciones;

public class CategoriaNoEncontradaException : ExcepcionAplicacion
{
    public CategoriaNoEncontradaException(Guid id)
        : base($"No se encontró la categoría '{id}'.")
    {
    }

    public override int CodigoHttp => 404;

    public override string Codigo => "CATEGORIA_NO_ENCONTRADA";
}
