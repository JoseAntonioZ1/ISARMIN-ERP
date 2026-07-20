using CajaEntity = ISARMIN.Domain.Entities.Caja.Caja;

namespace ISARMIN.Application.Modulos.Caja.DTOs;

public record CajaDto(
    Guid Id,
    DateTime FechaApertura,
    decimal MontoApertura,
    DateTime? FechaCierre,
    decimal? MontoTeoricoCierre,
    decimal? MontoFisicoDeclarado,
    decimal? Diferencia,
    Guid UsuarioId,
    string Estado);

public static class CajaMapper
{
    public static CajaDto ADto(this CajaEntity caja) => new(
        caja.Id,
        caja.FechaApertura,
        caja.MontoApertura,
        caja.FechaCierre,
        caja.MontoTeoricoCierre,
        caja.MontoFisicoDeclarado,
        caja.Diferencia,
        caja.UsuarioId,
        caja.Estado.ToString());
}
