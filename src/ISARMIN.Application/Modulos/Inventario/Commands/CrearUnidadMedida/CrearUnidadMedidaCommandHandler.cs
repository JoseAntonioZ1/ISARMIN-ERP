using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Inventario.DTOs;
using ISARMIN.Domain.Entities.Inventario;

namespace ISARMIN.Application.Modulos.Inventario.Commands.CrearUnidadMedida;

public class CrearUnidadMedidaCommandHandler : ICommandHandler<CrearUnidadMedidaCommand, UnidadMedidaDto>
{
    private readonly IUnidadMedidaRepository _unidadMedidaRepository;
    private readonly IValidator<CrearUnidadMedidaCommand> _validator;

    public CrearUnidadMedidaCommandHandler(IUnidadMedidaRepository unidadMedidaRepository, IValidator<CrearUnidadMedidaCommand> validator)
    {
        _unidadMedidaRepository = unidadMedidaRepository;
        _validator = validator;
    }

    public async Task<UnidadMedidaDto> ManejarAsync(CrearUnidadMedidaCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        var existente = await _unidadMedidaRepository.ObtenerPorNombreAsync(comando.Nombre, cancellationToken);
        if (existente is not null)
        {
            throw new NombreUnidadMedidaDuplicadoException(comando.Nombre);
        }

        var unidadMedida = new UnidadMedida(comando.Nombre);
        _unidadMedidaRepository.Agregar(unidadMedida);
        await _unidadMedidaRepository.GuardarCambiosAsync(cancellationToken);

        return unidadMedida.ADto();
    }
}
