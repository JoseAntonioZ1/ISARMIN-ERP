using ISARMIN.Domain.Entities.Terceros;

namespace ISARMIN.Application.Modulos.Clientes.DTOs;

public record ClienteDto(
    Guid Id,
    string NombreRazonSocial,
    string Telefono,
    string? Direccion,
    string? TipoDocumento,
    string? NumeroDocumento,
    string? TipoCliente,
    string Estado);

public static class ClienteMapper
{
    public static ClienteDto ADto(this Cliente cliente) => new(
        cliente.Id,
        cliente.NombreRazonSocial,
        cliente.Telefono,
        cliente.Direccion,
        cliente.TipoDocumento?.ToString(),
        cliente.NumeroDocumento,
        cliente.TipoCliente?.ToString(),
        cliente.Estado.ToString());
}
