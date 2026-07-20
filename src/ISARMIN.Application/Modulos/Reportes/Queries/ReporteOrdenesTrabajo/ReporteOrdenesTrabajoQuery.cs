using ISARMIN.Domain.Enums;

namespace ISARMIN.Application.Modulos.Reportes.Queries.ReporteOrdenesTrabajo;

public record ReporteOrdenesTrabajoQuery(EstadoOrdenTrabajo? Estado, DateTime? Desde, DateTime? Hasta);
