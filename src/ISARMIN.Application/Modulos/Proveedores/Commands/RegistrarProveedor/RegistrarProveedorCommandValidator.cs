using FluentValidation;

namespace ISARMIN.Application.Modulos.Proveedores.Commands.RegistrarProveedor;

public class RegistrarProveedorCommandValidator : AbstractValidator<RegistrarProveedorCommand>
{
    public RegistrarProveedorCommandValidator()
    {
        RuleFor(c => c.NombreRazonSocial).NotEmpty().MaximumLength(200);
        RuleFor(c => c.Documento).MaximumLength(20);
        RuleFor(c => c.Telefono).MaximumLength(30);
        RuleFor(c => c.Direccion).MaximumLength(255);
    }
}
