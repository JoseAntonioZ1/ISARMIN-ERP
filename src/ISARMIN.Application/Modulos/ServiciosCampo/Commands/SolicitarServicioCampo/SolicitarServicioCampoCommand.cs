namespace ISARMIN.Application.Modulos.ServiciosCampo.Commands.SolicitarServicioCampo;

public record SolicitarServicioCampoCommand(Guid ClienteId, string DescripcionTrabajo, Guid? TecnicoAsignadoId);
