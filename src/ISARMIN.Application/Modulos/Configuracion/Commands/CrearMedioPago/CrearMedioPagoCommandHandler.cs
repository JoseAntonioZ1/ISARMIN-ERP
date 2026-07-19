using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Configuracion.DTOs;
using ISARMIN.Domain.Entities.Configuracion;

namespace ISARMIN.Application.Modulos.Configuracion.Commands.CrearMedioPago;

public class CrearMedioPagoCommandHandler : ICommandHandler<CrearMedioPagoCommand, MedioPagoDto>
{
    private readonly IMedioPagoRepository _medioPagoRepository;
    private readonly IValidator<CrearMedioPagoCommand> _validator;

    public CrearMedioPagoCommandHandler(IMedioPagoRepository medioPagoRepository, IValidator<CrearMedioPagoCommand> validator)
    {
        _medioPagoRepository = medioPagoRepository;
        _validator = validator;
    }

    public async Task<MedioPagoDto> ManejarAsync(CrearMedioPagoCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        var existente = await _medioPagoRepository.ObtenerPorNombreAsync(comando.Nombre, cancellationToken);
        if (existente is not null)
        {
            throw new NombreMedioPagoDuplicadoException(comando.Nombre);
        }

        var medioPago = new MedioPago(comando.Nombre);
        _medioPagoRepository.Agregar(medioPago);
        await _medioPagoRepository.GuardarCambiosAsync(cancellationToken);

        return medioPago.ADto();
    }
}
