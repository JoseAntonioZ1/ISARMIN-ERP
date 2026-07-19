using FluentValidation;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Application.Modulos.Usuarios.Commands.AsignarPermisos;

public class AsignarPermisosCommandValidator : AbstractValidator<AsignarPermisosCommand>
{
    public AsignarPermisosCommandValidator()
    {
        RuleForEach(c => c.Permisos).ChildRules(permiso =>
        {
            permiso.RuleFor(p => p.Modulo).NotEmpty().MaximumLength(50);
            permiso.RuleFor(p => p.Accion)
                .Must(accion => Enum.TryParse<AccionPermiso>(accion, out _))
                .WithMessage(p => $"'{p.Accion}' no es una acción válida (Crear, Editar, Eliminar, Consultar, Anular).");
        });
    }
}
