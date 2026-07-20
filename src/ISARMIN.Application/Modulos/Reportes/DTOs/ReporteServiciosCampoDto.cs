using ISARMIN.Application.Modulos.ServiciosCampo.DTOs;

namespace ISARMIN.Application.Modulos.Reportes.DTOs;

/// <summary>RF-075 — filtra por técnico asignado y período (FechaSolicitud).</summary>
public record ReporteServiciosCampoDto(
    DateTime? Desde,
    DateTime? Hasta,
    int CantidadTotal,
    IReadOnlyCollection<ServicioCampoDto> Servicios);
