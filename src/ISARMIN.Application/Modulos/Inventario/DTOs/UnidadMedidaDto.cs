using ISARMIN.Domain.Entities.Inventario;

namespace ISARMIN.Application.Modulos.Inventario.DTOs;

public record UnidadMedidaDto(Guid Id, string Nombre);

public static class UnidadMedidaMapper
{
    public static UnidadMedidaDto ADto(this UnidadMedida unidadMedida) => new(unidadMedida.Id, unidadMedida.Nombre);
}
