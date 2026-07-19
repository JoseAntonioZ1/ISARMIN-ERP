namespace ISARMIN.Application.Modulos.Clientes.Commands.EditarCliente;

public record EditarClienteCommand(
    Guid Id,
    string NombreRazonSocial,
    string Telefono,
    string? Direccion,
    string? TipoDocumento,
    string? NumeroDocumento);
