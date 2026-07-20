using ISARMIN.Application.Modulos.Taller.DTOs;

namespace ISARMIN.Application.Modulos.Reportes.DTOs;

/// <summary>RF-074 — filtra por estado y período (FechaRecepcion); no filtra por técnico porque
/// OrdenTrabajo no tiene un campo de técnico asignado en el modelo confirmado (a diferencia de
/// ServicioCampo).</summary>
public record ReporteOrdenesTrabajoDto(
    DateTime? Desde,
    DateTime? Hasta,
    int CantidadTotal,
    IReadOnlyCollection<OrdenTrabajoDto> Ordenes);
