using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.ServiciosCampo.DTOs;

namespace ISARMIN.Application.Modulos.ServiciosCampo.Queries.BuscarServiciosCampo;

public class BuscarServiciosCampoQueryHandler : IQueryHandler<BuscarServiciosCampoQuery, ListadoPaginadoDto<ServicioCampoDto>>
{
    private readonly IServicioCampoRepository _servicioCampoRepository;

    public BuscarServiciosCampoQueryHandler(IServicioCampoRepository servicioCampoRepository)
    {
        _servicioCampoRepository = servicioCampoRepository;
    }

    public async Task<ListadoPaginadoDto<ServicioCampoDto>> ManejarAsync(BuscarServiciosCampoQuery consulta, CancellationToken cancellationToken = default)
    {
        var (servicios, total) = await _servicioCampoRepository.BuscarAsync(
            consulta.Estado, consulta.ClienteId, consulta.Pagina, consulta.TamanoPagina, cancellationToken);

        return new ListadoPaginadoDto<ServicioCampoDto>(servicios.Select(s => s.ADto()).ToList(), total, consulta.Pagina, consulta.TamanoPagina);
    }
}
