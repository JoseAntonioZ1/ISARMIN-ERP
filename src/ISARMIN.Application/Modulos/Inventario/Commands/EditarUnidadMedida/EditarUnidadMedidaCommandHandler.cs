using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Inventario.DTOs;

namespace ISARMIN.Application.Modulos.Inventario.Commands.EditarUnidadMedida;

public class EditarUnidadMedidaCommandHandler : ICommandHandler<EditarUnidadMedidaCommand, UnidadMedidaDto>
{
    private readonly IUnidadMedidaRepository _unidadMedidaRepository;
    private readonly IValidator<EditarUnidadMedidaCommand> _validator;

    public EditarUnidadMedidaCommandHandler(IUnidadMedidaRepository unidadMedidaRepository, IValidator<EditarUnidadMedidaCommand> validator)
    {
        _unidadMedidaRepository = unidadMedidaRepository;
        _validator = validator;
    }

    public async Task<UnidadMedidaDto> ManejarAsync(EditarUnidadMedidaCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        var unidadMedida = await _unidadMedidaRepository.ObtenerPorIdAsync(comando.Id, cancellationToken)
            ?? throw new UnidadMedidaNoEncontradaException(comando.Id);

        if (!string.Equals(unidadMedida.Nombre, comando.Nombre, StringComparison.Ordinal))
        {
            var existente = await _unidadMedidaRepository.ObtenerPorNombreAsync(comando.Nombre, cancellationToken);
            if (existente is not null)
            {
                throw new NombreUnidadMedidaDuplicadoException(comando.Nombre);
            }
        }

        unidadMedida.ActualizarDatos(comando.Nombre);
        await _unidadMedidaRepository.GuardarCambiosAsync(cancellationToken);

        return unidadMedida.ADto();
    }
}
