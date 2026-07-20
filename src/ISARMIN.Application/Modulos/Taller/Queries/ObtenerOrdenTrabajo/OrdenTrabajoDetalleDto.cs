using ISARMIN.Application.Modulos.Taller.DTOs;

namespace ISARMIN.Application.Modulos.Taller.Queries.ObtenerOrdenTrabajo;

/// <summary>UC-28/RF-060 — historial completo de la OT, incluyendo su garantía si tiene una registrada.</summary>
public record OrdenTrabajoDetalleDto(OrdenTrabajoDto OrdenTrabajo, GarantiaDto? Garantia);
