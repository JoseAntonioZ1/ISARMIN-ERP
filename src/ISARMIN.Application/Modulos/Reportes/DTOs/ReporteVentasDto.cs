using ISARMIN.Application.Modulos.Ventas.DTOs;

namespace ISARMIN.Application.Modulos.Reportes.DTOs;

/// <summary>RF-072 — MontoTotal excluye ventas Anuladas; el listado sí las incluye para trazabilidad.</summary>
public record ReporteVentasDto(
    DateTime? Desde,
    DateTime? Hasta,
    int CantidadVentas,
    decimal MontoTotal,
    IReadOnlyCollection<VentaDto> Ventas);
