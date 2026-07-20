using ISARMIN.Domain.Common;

namespace ISARMIN.Domain.Entities.Taller;

/// <summary>UC-23/RF-054 — diagnóstico técnico de un equipo, 1:1 con su Orden de Trabajo.</summary>
public class Diagnostico : Entity
{
    public Guid OrdenTrabajoId { get; private set; }
    public string Descripcion { get; private set; } = null!;
    public Guid UsuarioId { get; private set; }
    public DateTime Fecha { get; private set; }

    private Diagnostico() { }

    public Diagnostico(Guid ordenTrabajoId, string descripcion, Guid usuarioId, DateTime fecha)
    {
        if (string.IsNullOrWhiteSpace(descripcion))
        {
            throw new ArgumentException("La descripción del diagnóstico es obligatoria.", nameof(descripcion));
        }

        OrdenTrabajoId = ordenTrabajoId;
        Descripcion = descripcion;
        UsuarioId = usuarioId;
        Fecha = fecha;
    }
}
