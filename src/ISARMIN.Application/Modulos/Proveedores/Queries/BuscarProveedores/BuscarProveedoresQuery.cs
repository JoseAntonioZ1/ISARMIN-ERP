namespace ISARMIN.Application.Modulos.Proveedores.Queries.BuscarProveedores;

public record BuscarProveedoresQuery(string? Termino = null, int Pagina = 1, int TamanoPagina = 20);
