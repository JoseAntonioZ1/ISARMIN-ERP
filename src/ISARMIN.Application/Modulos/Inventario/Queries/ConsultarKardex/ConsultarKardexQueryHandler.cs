using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Inventario.DTOs;

namespace ISARMIN.Application.Modulos.Inventario.Queries.ConsultarKardex;

public class ConsultarKardexQueryHandler : IQueryHandler<ConsultarKardexQuery, IReadOnlyCollection<MovimientoInventarioDto>>
{
    private readonly IProductoRepository _productoRepository;
    private readonly IMovimientoInventarioRepository _movimientoInventarioRepository;

    public ConsultarKardexQueryHandler(IProductoRepository productoRepository, IMovimientoInventarioRepository movimientoInventarioRepository)
    {
        _productoRepository = productoRepository;
        _movimientoInventarioRepository = movimientoInventarioRepository;
    }

    public async Task<IReadOnlyCollection<MovimientoInventarioDto>> ManejarAsync(ConsultarKardexQuery consulta, CancellationToken cancellationToken = default)
    {
        if (await _productoRepository.ObtenerPorIdAsync(consulta.ProductoId, cancellationToken) is null)
        {
            throw new ProductoNoEncontradoException(consulta.ProductoId);
        }

        var movimientos = await _movimientoInventarioRepository.ListarPorProductoAsync(
            consulta.ProductoId, consulta.Desde, consulta.Hasta, cancellationToken);

        return movimientos.Select(m => m.ADto()).ToList();
    }
}
