namespace ISARMIN.Application.Common.Excepciones;

public class UnidadMedidaInvalidaException : ExcepcionAplicacion
{
    public UnidadMedidaInvalidaException(Guid unidadMedidaId)
        : base($"La unidad de medida '{unidadMedidaId}' no existe.")
    {
    }

    public override int CodigoHttp => 400;

    public override string Codigo => "UNIDAD_MEDIDA_INVALIDA";
}
