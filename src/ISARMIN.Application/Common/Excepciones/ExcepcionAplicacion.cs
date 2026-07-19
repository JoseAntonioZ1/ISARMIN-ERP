namespace ISARMIN.Application.Common.Excepciones;

/// <summary>
/// Base de las excepciones de Application que representan un error de negocio esperado
/// (no un bug): cada una declara el código HTTP y el código de error con el que la API
/// debe responder, evitando que ISARMIN.API tenga que mantener un mapeo caso por caso.
/// </summary>
public abstract class ExcepcionAplicacion : Exception
{
    protected ExcepcionAplicacion(string mensaje) : base(mensaje)
    {
    }

    public abstract int CodigoHttp { get; }

    public abstract string Codigo { get; }
}
