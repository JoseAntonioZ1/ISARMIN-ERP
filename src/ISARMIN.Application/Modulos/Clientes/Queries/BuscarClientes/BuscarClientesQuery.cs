namespace ISARMIN.Application.Modulos.Clientes.Queries.BuscarClientes;

/// <summary>UC-05, paso 3 — buscar por nombre o número de documento.</summary>
public record BuscarClientesQuery(string? Termino = null, int Pagina = 1, int TamanoPagina = 20);
