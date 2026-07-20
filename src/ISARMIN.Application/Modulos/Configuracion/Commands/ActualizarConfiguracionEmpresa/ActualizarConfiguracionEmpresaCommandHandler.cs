using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Configuracion.DTOs;

namespace ISARMIN.Application.Modulos.Configuracion.Commands.ActualizarConfiguracionEmpresa;

/// <summary>UC-37/RF-080 — solo hay una fila (sembrada), así que este comando siempre actualiza,
/// nunca crea.</summary>
public class ActualizarConfiguracionEmpresaCommandHandler : ICommandHandler<ActualizarConfiguracionEmpresaCommand, ConfiguracionEmpresaDto>
{
    private readonly IConfiguracionEmpresaRepository _configuracionEmpresaRepository;
    private readonly IValidator<ActualizarConfiguracionEmpresaCommand> _validator;

    public ActualizarConfiguracionEmpresaCommandHandler(
        IConfiguracionEmpresaRepository configuracionEmpresaRepository, IValidator<ActualizarConfiguracionEmpresaCommand> validator)
    {
        _configuracionEmpresaRepository = configuracionEmpresaRepository;
        _validator = validator;
    }

    public async Task<ConfiguracionEmpresaDto> ManejarAsync(ActualizarConfiguracionEmpresaCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        var configuracion = await _configuracionEmpresaRepository.ObtenerAsync(cancellationToken);
        configuracion.Actualizar(
            comando.RazonSocial, comando.Ruc, comando.Direccion, comando.Logo, comando.MontoAperturaCajaPredeterminado,
            comando.ColorAcento, comando.MensajeBienvenida);

        await _configuracionEmpresaRepository.GuardarCambiosAsync(cancellationToken);

        return configuracion.ADto();
    }
}
