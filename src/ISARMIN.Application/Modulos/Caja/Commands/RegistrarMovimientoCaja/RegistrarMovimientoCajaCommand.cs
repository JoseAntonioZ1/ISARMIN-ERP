using ISARMIN.Domain.Enums;

namespace ISARMIN.Application.Modulos.Caja.Commands.RegistrarMovimientoCaja;

public record RegistrarMovimientoCajaCommand(ConceptoMovimientoCaja Concepto, decimal Monto, string? Descripcion, Guid UsuarioId);
