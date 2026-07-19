using FluentValidation;

namespace ISARMIN.Application.Modulos.Proveedores.Commands.EditarProveedor;

public class EditarProveedorCommandValidator : AbstractValidator<EditarProveedorCommand>
{
    public EditarProveedorCommandValidator()
    {
        RuleFor(c => c.NombreRazonSocial).NotEmpty().MaximumLength(200);
        RuleFor(c => c.Documento).MaximumLength(20);
        RuleFor(c => c.Telefono).MaximumLength(30);
        RuleFor(c => c.Direccion).MaximumLength(255);
    }
}
