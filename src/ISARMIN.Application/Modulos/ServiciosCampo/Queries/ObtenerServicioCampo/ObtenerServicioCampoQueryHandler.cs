using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.ServiciosCampo.DTOs;

namespace ISARMIN.Application.Modulos.ServiciosCampo.Queries.ObtenerServicioCampo;

public class ObtenerServicioCampoQueryHandler : IQueryHandler<ObtenerServicioCampoQuery, ServicioCampoDto>
{
    private readonly IServicioCampoRepository _servicioCampoRepository;

    public ObtenerServicioCampoQueryHandler(IServicioCampoRepository servicioCampoRepository)
    {
        _servicioCampoRepository = servicioCampoRepository;
    }

    public async Task<ServicioCampoDto> ManejarAsync(ObtenerServicioCampoQuery consulta, CancellationToken cancellationToken = default)
    {
        var servicio = await _servicioCampoRepository.ObtenerPorIdAsync(consulta.Id, cancellationToken)
            ?? throw new ServicioCampoNoEncontradoException(consulta.Id);

        return servicio.ADto();
    }
}
