using ISARMIN.Domain.Common;

namespace ISARMIN.Domain.Entities.Configuracion;

/// <summary>CAT-008 — catálogo configurable de medios de pago (RF-042), ampliable sin cambios de arquitectura.</summary>
public class MedioPago : Entity
{
    public string Nombre { get; private set; } = null!;
    public bool Activo { get; private set; }

    private MedioPago() { }

    public MedioPago(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre del medio de pago es obligatorio.", nameof(nombre));
        }

        Nombre = nombre;
        Activo = true;
    }

    public void Activar() => Activo = true;

    public void Desactivar() => Activo = false;
}
