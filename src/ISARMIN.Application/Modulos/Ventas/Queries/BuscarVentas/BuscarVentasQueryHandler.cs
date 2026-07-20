using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Ventas.DTOs;

namespace ISARMIN.Application.Modulos.Ventas.Queries.BuscarVentas;

public class BuscarVentasQueryHandler : IQueryHandler<BuscarVentasQuery, ListadoPaginadoDto<VentaDto>>
{
    private readonly IVentaRepository _ventaRepository;

    public BuscarVentasQueryHandler(IVentaRepository ventaRepository)
    {
        _ventaRepository = ventaRepository;
    }

    public async Task<ListadoPaginadoDto<VentaDto>> ManejarAsync(BuscarVentasQuery consulta, CancellationToken cancellationToken = default)
    {
        var (ventas, total) = await _ventaRepository.BuscarAsync(
            consulta.Estado, consulta.ClienteId, consulta.Pagina, consulta.TamanoPagina, cancellationToken);

        return new ListadoPaginadoDto<VentaDto>(ventas.Select(v => v.ADto()).ToList(), total, consulta.Pagina, consulta.TamanoPagina);
    }
}
