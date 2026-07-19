using ISARMIN.Domain.Entities.Terceros;

namespace ISARMIN.Application.Modulos.Proveedores.DTOs;

public record ProveedorDto(
    Guid Id,
    string NombreRazonSocial,
    string? Documento,
    string? Telefono,
    string? Direccion,
    string Estado);

public static class ProveedorMapper
{
    public static ProveedorDto ADto(this Proveedor proveedor) => new(
        proveedor.Id,
        proveedor.NombreRazonSocial,
        proveedor.Documento,
        proveedor.Telefono,
        proveedor.Direccion,
        proveedor.Estado.ToString());
}
