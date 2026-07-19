namespace ISARMIN.Application.Modulos.Inventario.Queries.ConsultarKardex;

public record ConsultarKardexQuery(Guid ProductoId, DateTime? Desde = null, DateTime? Hasta = null);
