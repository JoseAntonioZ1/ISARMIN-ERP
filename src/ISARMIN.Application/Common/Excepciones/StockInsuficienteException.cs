namespace ISARMIN.Application.Common.Excepciones;

/// <summary>RN-003 — no puede consumirse un producto sin stock disponible.</summary>
public class StockInsuficienteException : ExcepcionAplicacion
{
    public StockInsuficienteException(Guid productoId)
        : base($"No hay stock suficiente del producto '{productoId}' para el consumo solicitado.")
    {
    }

    public override int CodigoHttp => 400;

    public override string Codigo => "STOCK_INSUFICIENTE";
}
