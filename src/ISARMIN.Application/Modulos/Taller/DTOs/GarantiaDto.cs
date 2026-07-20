using ISARMIN.Domain.Entities.Taller;

namespace ISARMIN.Application.Modulos.Taller.DTOs;

public record GarantiaDto(Guid Id, Guid OrdenTrabajoId, DateOnly FechaInicio, DateOnly FechaFin, Guid? OrdenTrabajoReingresoId);

public static class GarantiaMapper
{
    public static GarantiaDto ADto(this Garantia garantia) => new(
        garantia.Id, garantia.OrdenTrabajoId, garantia.FechaInicio, garantia.FechaFin, garantia.OrdenTrabajoReingresoId);
}
