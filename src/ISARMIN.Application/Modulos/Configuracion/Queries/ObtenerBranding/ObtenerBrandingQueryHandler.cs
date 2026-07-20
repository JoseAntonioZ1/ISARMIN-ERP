using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Configuracion.DTOs;

namespace ISARMIN.Application.Modulos.Configuracion.Queries.ObtenerBranding;

/// <summary>Endpoint público (sin autenticación) — expone solo identidad visual y textos (ver
/// <see cref="BrandingDto"/>), para personalizar el login, el encabezado y la pantalla de inicio
/// independientemente de si hay sesión iniciada o del rol del usuario.</summary>
public class ObtenerBrandingQueryHandler : IQueryHandler<ObtenerBrandingQuery, BrandingDto>
{
    private readonly IConfiguracionEmpresaRepository _configuracionEmpresaRepository;

    public ObtenerBrandingQueryHandler(IConfiguracionEmpresaRepository configuracionEmpresaRepository)
    {
        _configuracionEmpresaRepository = configuracionEmpresaRepository;
    }

    public async Task<BrandingDto> ManejarAsync(ObtenerBrandingQuery consulta, CancellationToken cancellationToken = default)
    {
        var configuracion = await _configuracionEmpresaRepository.ObtenerAsync(cancellationToken);
        return new BrandingDto(
            configuracion.RazonSocial, configuracion.Ruc, configuracion.Logo, configuracion.ColorAcento, configuracion.MensajeBienvenida);
    }
}
