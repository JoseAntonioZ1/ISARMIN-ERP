namespace ISARMIN.Application.Common.Excepciones;

public class NombreUnidadMedidaDuplicadoException : ExcepcionAplicacion
{
    public NombreUnidadMedidaDuplicadoException(string nombre)
        : base($"Ya existe una unidad de medida con el nombre '{nombre}'.")
    {
    }

    public override int CodigoHttp => 409;

    public override string Codigo => "NOMBRE_UNIDAD_MEDIDA_DUPLICADO";
}
