namespace ISARMIN.Application.Common.Excepciones;

public class UnidadMedidaNoEncontradaException : ExcepcionAplicacion
{
    public UnidadMedidaNoEncontradaException(Guid id)
        : base($"No se encontró la unidad de medida '{id}'.")
    {
    }

    public override int CodigoHttp => 404;

    public override string Codigo => "UNIDAD_MEDIDA_NO_ENCONTRADA";
}
