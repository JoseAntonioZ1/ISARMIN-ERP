using ISARMIN.Domain.Enums;

namespace ISARMIN.Application.Modulos.Ventas.Queries.BuscarVentas;

public record BuscarVentasQuery(EstadoVenta? Estado = null, Guid? ClienteId = null, int Pagina = 1, int TamanoPagina = 20);
