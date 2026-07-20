namespace ISARMIN.Application.Common.Excepciones;

/// <summary>El servicio de campo no está en el estado requerido para la acción solicitada (UC-30 a UC-33).</summary>
public class EstadoServicioCampoInvalidoException : ExcepcionAplicacion
{
    public EstadoServicioCampoInvalidoException(Guid servicioCampoId, string mensaje)
        : base(mensaje)
    {
        ServicioCampoId = servicioCampoId;
    }

    public Guid ServicioCampoId { get; }

    public override int CodigoHttp => 409;

    public override string Codigo => "ESTADO_SERVICIO_CAMPO_INVALIDO";
}
