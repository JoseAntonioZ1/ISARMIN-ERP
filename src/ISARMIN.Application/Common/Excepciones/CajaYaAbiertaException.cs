namespace ISARMIN.Application.Common.Excepciones;

public class CajaYaAbiertaException : ExcepcionAplicacion
{
    public CajaYaAbiertaException()
        : base("Ya existe una caja abierta. Debe cerrarla antes de abrir una nueva (RN-025).")
    {
    }

    public override int CodigoHttp => 409;

    public override string Codigo => "CAJA_YA_ABIERTA";
}
