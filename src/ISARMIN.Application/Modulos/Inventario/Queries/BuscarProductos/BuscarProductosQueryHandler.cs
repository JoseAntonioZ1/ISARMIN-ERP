using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Inventario.DTOs;

namespace ISARMIN.Application.Modulos.Inventario.Queries.BuscarProductos;

public class BuscarProductosQueryHandler : IQueryHandler<BuscarProductosQuery, ListadoPaginadoDto<ProductoDto>>
{
    private readonly IProductoRepository _productoRepository;

    public BuscarProductosQueryHandler(IProductoRepository productoRepository)
    {
        _productoRepository = productoRepository;
    }

    public async Task<ListadoPaginadoDto<ProductoDto>> ManejarAsync(BuscarProductosQuery consulta, CancellationToken cancellationToken = default)
    {
        var (productos, total) = await _productoRepository.BuscarAsync(consulta.Termino, consulta.CategoriaId, consulta.Pagina, consulta.TamanoPagina, cancellationToken);

        return new ListadoPaginadoDto<ProductoDto>(productos.Select(p => p.ADto()).ToList(), total, consulta.Pagina, consulta.TamanoPagina);
    }
}
