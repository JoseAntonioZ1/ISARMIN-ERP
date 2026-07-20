using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Caja.DTOs;
using CajaEntity = ISARMIN.Domain.Entities.Caja.Caja;

namespace ISARMIN.Application.Modulos.Caja.Commands.AbrirCaja;

/// <summary>UC-19 (apertura) — RN-025: no puede haber más de una caja abierta a la vez.</summary>
public class AbrirCajaCommandHandler : ICommandHandler<AbrirCajaCommand, CajaDto>
{
    private readonly ICajaRepository _cajaRepository;
    private readonly IFechaHoraProvider _fechaHoraProvider;
    private readonly IValidator<AbrirCajaCommand> _validator;

    public AbrirCajaCommandHandler(ICajaRepository cajaRepository, IFechaHoraProvider fechaHoraProvider, IValidator<AbrirCajaCommand> validator)
    {
        _cajaRepository = cajaRepository;
        _fechaHoraProvider = fechaHoraProvider;
        _validator = validator;
    }

    public async Task<CajaDto> ManejarAsync(AbrirCajaCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        if (await _cajaRepository.ObtenerAbiertaAsync(cancellationToken) is not null)
        {
            throw new CajaYaAbiertaException();
        }

        var caja = new CajaEntity(comando.MontoApertura, comando.UsuarioId, _fechaHoraProvider.UtcAhora);

        _cajaRepository.Agregar(caja);
        await _cajaRepository.GuardarCambiosAsync(cancellationToken);

        return caja.ADto();
    }
}
