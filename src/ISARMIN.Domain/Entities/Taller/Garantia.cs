using ISARMIN.Domain.Common;

namespace ISARMIN.Domain.Entities.Taller;

/// <summary>UC-27/RF-061, RF-062 — RN-017: alcance V1 acotado a un período simple (sin tipos de
/// garantía ni condiciones de cobertura). <see cref="OrdenTrabajoReingresoId"/> vincularía una nueva
/// OT que reingresa por la misma falla dentro del período vigente (RF-062); esa asociación queda
/// fuera de alcance en esta implementación (flujo condicional futuro, sin endpoint definido en
/// API-Design.md) — la columna existe para no bloquear el dato a futuro.</summary>
public class Garantia : Entity
{
    public Guid OrdenTrabajoId { get; private set; }
    public DateOnly FechaInicio { get; private set; }
    public DateOnly FechaFin { get; private set; }
    public Guid? OrdenTrabajoReingresoId { get; private set; }

    private Garantia() { }

    public Garantia(Guid ordenTrabajoId, DateOnly fechaInicio, DateOnly fechaFin)
    {
        if (fechaFin <= fechaInicio)
        {
            throw new ArgumentException("La fecha de fin debe ser posterior a la fecha de inicio.", nameof(fechaFin));
        }

        OrdenTrabajoId = ordenTrabajoId;
        FechaInicio = fechaInicio;
        FechaFin = fechaFin;
    }
}
