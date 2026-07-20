namespace ISARMIN.Application.Modulos.Caja.Queries.ListarMovimientosCaja;

public record ListarMovimientosCajaQuery(Guid? CajaId = null, DateTime? Desde = null, DateTime? Hasta = null);
