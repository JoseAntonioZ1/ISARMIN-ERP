namespace ISARMIN.Application.Common.Excepciones;

/// <summary>RN-032 — una devolución solo puede registrarse sobre un producto que fue parte de la venta original.</summary>
public class ProductoNoVendidoException : ExcepcionAplicacion
{
    public ProductoNoVendidoException(Guid productoId, Guid ventaId)
        : base($"El producto '{productoId}' no forma parte de la venta '{ventaId}'.")
    {
    }

    public override int CodigoHttp => 400;

    public override string Codigo => "PRODUCTO_NO_VENDIDO";
}
