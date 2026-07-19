namespace ISARMIN.Application.Modulos.Proveedores.Commands.RegistrarProveedor;

public record RegistrarProveedorCommand(string NombreRazonSocial, string? Documento, string? Telefono, string? Direccion);
