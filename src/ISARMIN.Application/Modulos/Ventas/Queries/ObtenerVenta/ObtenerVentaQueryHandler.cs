using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Ventas.DTOs;

namespace ISARMIN.Application.Modulos.Ventas.Queries.ObtenerVenta;

public class ObtenerVentaQueryHandler : IQueryHandler<ObtenerVentaQuery, VentaDto>
{
    private readonly IVentaRepository _ventaRepository;

    public ObtenerVentaQueryHandler(IVentaRepository ventaRepository)
    {
        _ventaRepository = ventaRepository;
    }

    public async Task<VentaDto> ManejarAsync(ObtenerVentaQuery consulta, CancellationToken cancellationToken = default)
    {
        var venta = await _ventaRepository.ObtenerPorIdAsync(consulta.Id, cancellationToken)
            ?? throw new VentaNoEncontradaException(consulta.Id);

        return venta.ADto();
    }
}
