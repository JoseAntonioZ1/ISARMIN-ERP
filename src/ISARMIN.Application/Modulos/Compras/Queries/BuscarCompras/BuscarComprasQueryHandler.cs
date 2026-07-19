using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Compras.DTOs;

namespace ISARMIN.Application.Modulos.Compras.Queries.BuscarCompras;

public class BuscarComprasQueryHandler : IQueryHandler<BuscarComprasQuery, ListadoPaginadoDto<CompraDto>>
{
    private readonly ICompraRepository _compraRepository;

    public BuscarComprasQueryHandler(ICompraRepository compraRepository)
    {
        _compraRepository = compraRepository;
    }

    public async Task<ListadoPaginadoDto<CompraDto>> ManejarAsync(BuscarComprasQuery consulta, CancellationToken cancellationToken = default)
    {
        var (compras, total) = await _compraRepository.BuscarAsync(
            consulta.ProveedorId, consulta.ProductoId, consulta.Pagina, consulta.TamanoPagina, cancellationToken);

        return new ListadoPaginadoDto<CompraDto>(compras.Select(c => c.ADto()).ToList(), total, consulta.Pagina, consulta.TamanoPagina);
    }
}
