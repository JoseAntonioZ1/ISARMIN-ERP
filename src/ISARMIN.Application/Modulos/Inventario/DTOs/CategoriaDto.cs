using ISARMIN.Domain.Entities.Inventario;

namespace ISARMIN.Application.Modulos.Inventario.DTOs;

public record CategoriaDto(Guid Id, string Nombre, Guid? CategoriaPadreId);

public static class CategoriaMapper
{
    public static CategoriaDto ADto(this Categoria categoria) => new(categoria.Id, categoria.Nombre, categoria.CategoriaPadreId);
}
