using ISARMIN.Domain.Entities.Configuracion;

namespace ISARMIN.Application.Modulos.Configuracion.DTOs;

public record MedioPagoDto(Guid Id, string Nombre, bool Activo);

public static class MedioPagoMapper
{
    public static MedioPagoDto ADto(this MedioPago medioPago) => new(medioPago.Id, medioPago.Nombre, medioPago.Activo);
}
