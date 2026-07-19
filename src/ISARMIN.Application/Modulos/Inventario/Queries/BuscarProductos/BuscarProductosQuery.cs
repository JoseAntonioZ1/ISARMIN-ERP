namespace ISARMIN.Application.Modulos.Inventario.Queries.BuscarProductos;

public record BuscarProductosQuery(string? Termino = null, Guid? CategoriaId = null, int Pagina = 1, int TamanoPagina = 20);
