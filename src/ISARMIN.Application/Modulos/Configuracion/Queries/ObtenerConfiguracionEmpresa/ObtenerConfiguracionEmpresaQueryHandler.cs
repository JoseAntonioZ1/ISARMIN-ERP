using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Configuracion.DTOs;

namespace ISARMIN.Application.Modulos.Configuracion.Queries.ObtenerConfiguracionEmpresa;

public class ObtenerConfiguracionEmpresaQueryHandler : IQueryHandler<ObtenerConfiguracionEmpresaQuery, ConfiguracionEmpresaDto>
{
    private readonly IConfiguracionEmpresaRepository _configuracionEmpresaRepository;

    public ObtenerConfiguracionEmpresaQueryHandler(IConfiguracionEmpresaRepository configuracionEmpresaRepository)
    {
        _configuracionEmpresaRepository = configuracionEmpresaRepository;
    }

    public async Task<ConfiguracionEmpresaDto> ManejarAsync(ObtenerConfiguracionEmpresaQuery consulta, CancellationToken cancellationToken = default)
    {
        var configuracion = await _configuracionEmpresaRepository.ObtenerAsync(cancellationToken);
        return configuracion.ADto();
    }
}
