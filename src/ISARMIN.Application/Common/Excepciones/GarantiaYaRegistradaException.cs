namespace ISARMIN.Application.Common.Excepciones;

public class GarantiaYaRegistradaException : ExcepcionAplicacion
{
    public GarantiaYaRegistradaException(Guid ordenTrabajoId)
        : base($"La orden de trabajo '{ordenTrabajoId}' ya tiene una garantía registrada.")
    {
    }

    public override int CodigoHttp => 409;

    public override string Codigo => "GARANTIA_YA_REGISTRADA";
}
