namespace ISARMIN.Application.Modulos.Clientes.Commands.RegistrarCliente;

public record RegistrarClienteCommand(
    string NombreRazonSocial,
    string Telefono,
    string? Direccion,
    string? TipoDocumento,
    string? NumeroDocumento);
