using ISARMIN.Domain.Common;

namespace ISARMIN.Domain.Entities.Inventario;

/// <summary>CAT-002 — catálogo configurable de categorías de producto (RF-025).</summary>
public class Categoria : Entity
{
    public string Nombre { get; private set; } = null!;

    /// <summary>Referencia opcional a una categoría padre — esquema previsto para BQ-007 (jerarquía), aún sin confirmar ni usado.</summary>
    public Guid? CategoriaPadreId { get; private set; }

    private Categoria() { }

    public Categoria(string nombre, Guid? categoriaPadreId = null)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre de la categoría es obligatorio.", nameof(nombre));
        }

        if (categoriaPadreId == Id)
        {
            throw new ArgumentException("Una categoría no puede ser su propia categoría padre.", nameof(categoriaPadreId));
        }

        Nombre = nombre;
        CategoriaPadreId = categoriaPadreId;
    }

    public void ActualizarDatos(string nombre, Guid? categoriaPadreId)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre de la categoría es obligatorio.", nameof(nombre));
        }

        if (categoriaPadreId == Id)
        {
            throw new ArgumentException("Una categoría no puede ser su propia categoría padre.", nameof(categoriaPadreId));
        }

        Nombre = nombre;
        CategoriaPadreId = categoriaPadreId;
    }
}
