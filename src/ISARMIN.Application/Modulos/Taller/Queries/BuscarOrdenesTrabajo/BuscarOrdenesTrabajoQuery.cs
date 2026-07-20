using ISARMIN.Domain.Enums;

namespace ISARMIN.Application.Modulos.Taller.Queries.BuscarOrdenesTrabajo;

public record BuscarOrdenesTrabajoQuery(EstadoOrdenTrabajo? Estado = null, Guid? ClienteId = null, int Pagina = 1, int TamanoPagina = 20);
