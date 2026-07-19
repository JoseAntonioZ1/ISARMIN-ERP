using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Inventario.DTOs;

namespace ISARMIN.Application.Modulos.Inventario.Queries.ListarCategorias;

public class ListarCategoriasQueryHandler : IQueryHandler<ListarCategoriasQuery, IReadOnlyCollection<CategoriaDto>>
{
    private readonly ICategoriaRepository _categoriaRepository;

    public ListarCategoriasQueryHandler(ICategoriaRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }

    public async Task<IReadOnlyCollection<CategoriaDto>> ManejarAsync(ListarCategoriasQuery consulta, CancellationToken cancellationToken = default)
    {
        var categorias = await _categoriaRepository.ListarTodasAsync(cancellationToken);
        return categorias.Select(c => c.ADto()).ToList();
    }
}
