namespace ISARMIN.Application.Modulos.Compras.Queries.BuscarCompras;

public record BuscarComprasQuery(Guid? ProveedorId = null, Guid? ProductoId = null, int Pagina = 1, int TamanoPagina = 20);
