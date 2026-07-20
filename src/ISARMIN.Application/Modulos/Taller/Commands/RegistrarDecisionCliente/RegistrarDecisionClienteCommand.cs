using ISARMIN.Domain.Enums;

namespace ISARMIN.Application.Modulos.Taller.Commands.RegistrarDecisionCliente;

public record RegistrarDecisionClienteCommand(
    Guid OrdenTrabajoId, DecisionCliente Decision, decimal? CobroDiagnosticoRechazo, string? EvidenciaAprobacion);
