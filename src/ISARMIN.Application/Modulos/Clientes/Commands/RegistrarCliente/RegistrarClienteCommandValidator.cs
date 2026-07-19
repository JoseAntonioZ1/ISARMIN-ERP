using FluentValidation;
using ISARMIN.Domain.Common;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Application.Modulos.Clientes.Commands.RegistrarCliente;

public class RegistrarClienteCommandValidator : AbstractValidator<RegistrarClienteCommand>
{
    public RegistrarClienteCommandValidator()
    {
        RuleFor(c => c.NombreRazonSocial).NotEmpty().MaximumLength(200);
        RuleFor(c => c.Telefono).NotEmpty().MaximumLength(30);
        RuleFor(c => c.Direccion).MaximumLength(255);

        RuleFor(c => c.TipoDocumento)
            .Must(t => t is null || Enum.TryParse<TipoDocumento>(t, out _))
            .WithMessage("El tipo de documento debe ser Dni, Ruc, CarneExtranjeria o Pasaporte.");

        RuleFor(c => c)
            .Must(c => (c.TipoDocumento is null) == (c.NumeroDocumento is null))
            .WithMessage("El tipo y el número de documento deben proporcionarse juntos.")
            .WithName("NumeroDocumento");

        RuleFor(c => c)
            .Must(EsDocumentoValido)
            .WithMessage("El número de documento no es válido para el tipo indicado (RN-033).")
            .WithName("NumeroDocumento")
            .When(c => c.TipoDocumento is not null && Enum.TryParse<TipoDocumento>(c.TipoDocumento, out _));
    }

    private static bool EsDocumentoValido(RegistrarClienteCommand comando)
    {
        if (comando.TipoDocumento is null || comando.NumeroDocumento is null)
        {
            return true;
        }

        return Enum.TryParse<TipoDocumento>(comando.TipoDocumento, out var tipo)
            && ValidadorDocumentoIdentidad.EsValido(tipo, comando.NumeroDocumento);
    }
}
