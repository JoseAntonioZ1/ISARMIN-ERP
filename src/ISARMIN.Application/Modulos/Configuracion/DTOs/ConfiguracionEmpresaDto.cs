using ISARMIN.Domain.Entities.Configuracion;

namespace ISARMIN.Application.Modulos.Configuracion.DTOs;

public record ConfiguracionEmpresaDto(
    Guid Id,
    string RazonSocial,
    string? Ruc,
    string? Direccion,
    string? Logo,
    decimal? MontoAperturaCajaPredeterminado);

public static class ConfiguracionEmpresaMapper
{
    public static ConfiguracionEmpresaDto ADto(this ConfiguracionEmpresa configuracion) => new(
        configuracion.Id,
        configuracion.RazonSocial,
        configuracion.Ruc,
        configuracion.Direccion,
        configuracion.Logo,
        configuracion.MontoAperturaCajaPredeterminado);
}
