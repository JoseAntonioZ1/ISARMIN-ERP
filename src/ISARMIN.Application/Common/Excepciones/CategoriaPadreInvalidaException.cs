namespace ISARMIN.Application.Common.Excepciones;

public class CategoriaPadreInvalidaException : ExcepcionAplicacion
{
    public CategoriaPadreInvalidaException(Guid categoriaPadreId)
        : base($"La categoría padre '{categoriaPadreId}' no existe.")
    {
    }

    public override int CodigoHttp => 400;

    public override string Codigo => "CATEGORIA_PADRE_INVALIDA";
}
