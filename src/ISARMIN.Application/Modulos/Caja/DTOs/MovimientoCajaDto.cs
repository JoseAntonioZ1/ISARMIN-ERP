using ISARMIN.Domain.Entities.Caja;

namespace ISARMIN.Application.Modulos.Caja.DTOs;

public record MovimientoCajaDto(
    Guid Id,
    Guid CajaId,
    string Tipo,
    decimal Monto,
    string Concepto,
    string? Descripcion,
    Guid UsuarioId,
    DateTime Fecha);

public static class MovimientoCajaMapper
{
    public static MovimientoCajaDto ADto(this MovimientoCaja movimiento) => new(
        movimiento.Id,
        movimiento.CajaId,
        movimiento.Tipo.ToString(),
        movimiento.Monto,
        movimiento.Concepto.ToString(),
        movimiento.Descripcion,
        movimiento.UsuarioId,
        movimiento.Fecha);
}
