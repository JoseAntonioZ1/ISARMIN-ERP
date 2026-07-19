namespace ISARMIN.Application.Modulos.Proveedores.Commands.EditarProveedor;

public record EditarProveedorCommand(Guid Id, string NombreRazonSocial, string? Documento, string? Telefono, string? Direccion);
