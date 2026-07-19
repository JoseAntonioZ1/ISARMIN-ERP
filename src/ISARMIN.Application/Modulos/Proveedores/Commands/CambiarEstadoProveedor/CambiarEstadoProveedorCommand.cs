namespace ISARMIN.Application.Modulos.Proveedores.Commands.CambiarEstadoProveedor;

/// <summary>RF-020/RN-023 — baja lógica, nunca eliminación física.</summary>
public record CambiarEstadoProveedorCommand(Guid Id, bool Activo);
