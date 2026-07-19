using ISARMIN.Domain.Common;

namespace ISARMIN.Domain.Entities.Inventario;

/// <summary>CAT-014 — catálogo configurable de unidades de medida (RN-040), resuelve BQ-008.</summary>
public class UnidadMedida : Entity
{
    public string Nombre { get; private set; } = null!;

    private UnidadMedida() { }

    public UnidadMedida(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre de la unidad de medida es obligatorio.", nameof(nombre));
        }

        Nombre = nombre;
    }

    public void ActualizarDatos(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre de la unidad de medida es obligatorio.", nameof(nombre));
        }

        Nombre = nombre;
    }
}
