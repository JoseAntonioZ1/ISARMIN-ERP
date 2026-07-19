namespace ISARMIN.Application.Common.Excepciones;

public class CodigoBarrasDuplicadoException : ExcepcionAplicacion
{
    public CodigoBarrasDuplicadoException(string codigoBarras)
        : base($"Ya existe un producto con el código de barras '{codigoBarras}'.")
    {
    }

    public override int CodigoHttp => 409;

    public override string Codigo => "CODIGO_BARRAS_DUPLICADO";
}
