namespace ISARMIN.Application.Common.Excepciones;

public class CategoriaInvalidaException : ExcepcionAplicacion
{
    public CategoriaInvalidaException(Guid categoriaId)
        : base($"La categoría '{categoriaId}' no existe.")
    {
    }

    public override int CodigoHttp => 400;

    public override string Codigo => "CATEGORIA_INVALIDA";
}
