using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.ServiciosCampo.DTOs;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Application.Modulos.ServiciosCampo.Commands.CotizarServicioCampo;

/// <summary>UC-31/RF-068 — el servicio debe estar en estado Solicitado.</summary>
public class CotizarServicioCampoCommandHandler : ICommandHandler<CotizarServicioCampoCommand, ServicioCampoDto>
{
    private readonly IServicioCampoRepository _servicioCampoRepository;
    private readonly IValidator<CotizarServicioCampoCommand> _validator;

    public CotizarServicioCampoCommandHandler(IServicioCampoRepository servicioCampoRepository, IValidator<CotizarServicioCampoCommand> validator)
    {
        _servicioCampoRepository = servicioCampoRepository;
        _validator = validator;
    }

    public async Task<ServicioCampoDto> ManejarAsync(CotizarServicioCampoCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        var servicio = await _servicioCampoRepository.ObtenerPorIdAsync(comando.ServicioCampoId, cancellationToken)
            ?? throw new ServicioCampoNoEncontradoException(comando.ServicioCampoId);

        if (servicio.Estado != EstadoServicioCampo.Solicitado)
        {
            throw new EstadoServicioCampoInvalidoException(servicio.Id, "Solo se puede cotizar un servicio en estado Solicitado.");
        }

        servicio.Cotizar(comando.MontoEstimado);
        await _servicioCampoRepository.GuardarCambiosAsync(cancellationToken);

        return servicio.ADto();
    }
}
