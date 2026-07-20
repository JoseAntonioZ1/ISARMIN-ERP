namespace ISARMIN.Application.Common.Excepciones;

/// <summary>RN-014 — no puede registrarse un cobro/movimiento ni cerrarse una caja sin una caja abierta.</summary>
public class CajaNoAbiertaException : ExcepcionAplicacion
{
    public CajaNoAbiertaException()
        : base("No hay una caja abierta actualmente.")
    {
    }

    public override int CodigoHttp => 409;

    public override string Codigo => "CAJA_NO_ABIERTA";
}
