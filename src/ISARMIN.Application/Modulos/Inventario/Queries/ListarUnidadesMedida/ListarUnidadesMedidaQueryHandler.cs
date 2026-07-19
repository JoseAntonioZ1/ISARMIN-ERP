using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Inventario.DTOs;

namespace ISARMIN.Application.Modulos.Inventario.Queries.ListarUnidadesMedida;

public class ListarUnidadesMedidaQueryHandler : IQueryHandler<ListarUnidadesMedidaQuery, IReadOnlyCollection<UnidadMedidaDto>>
{
    private readonly IUnidadMedidaRepository _unidadMedidaRepository;

    public ListarUnidadesMedidaQueryHandler(IUnidadMedidaRepository unidadMedidaRepository)
    {
        _unidadMedidaRepository = unidadMedidaRepository;
    }

    public async Task<IReadOnlyCollection<UnidadMedidaDto>> ManejarAsync(ListarUnidadesMedidaQuery consulta, CancellationToken cancellationToken = default)
    {
        var unidadesMedida = await _unidadMedidaRepository.ListarTodasAsync(cancellationToken);
        return unidadesMedida.Select(u => u.ADto()).ToList();
    }
}
