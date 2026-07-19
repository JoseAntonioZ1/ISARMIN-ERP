namespace ISARMIN.Application.Modulos.Clientes.Commands.CambiarEstadoCliente;

/// <summary>RF-014/RN-023 — baja lógica, nunca eliminación física.</summary>
public record CambiarEstadoClienteCommand(Guid Id, bool Activo);
