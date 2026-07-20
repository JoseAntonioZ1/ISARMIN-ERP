using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Caja.DTOs;

namespace ISARMIN.Application.Modulos.Caja.Commands.CerrarCaja;

/// <summary>UC-19 (cierre)/RF-048/RN-015 — concilia el monto teórico (apertura + ingresos - egresos)
/// contra el monto físico declarado. El mecanismo de resolución de un descuadre no está definido
/// (BQ-031); este comando solo registra ambos montos y expone la diferencia calculada.</summary>
public class CerrarCajaCommandHandler : ICommandHandler<CerrarCajaCommand, CajaDto>
{
    private readonly ICajaRepository _cajaRepository;
    private readonly IMovimientoCajaRepository _movimientoCajaRepository;
    private readonly IFechaHoraProvider _fechaHoraProvider;
    private readonly IValidator<CerrarCajaCommand> _validator;

    public CerrarCajaCommandHandler(
        ICajaRepository cajaRepository,
        IMovimientoCajaRepository movimientoCajaRepository,
        IFechaHoraProvider fechaHoraProvider,
        IValidator<CerrarCajaCommand> validator)
    {
        _cajaRepository = cajaRepository;
        _movimientoCajaRepository = movimientoCajaRepository;
        _fechaHoraProvider = fechaHoraProvider;
        _validator = validator;
    }

    public async Task<CajaDto> ManejarAsync(CerrarCajaCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        var caja = await _cajaRepository.ObtenerAbiertaAsync(cancellationToken)
            ?? throw new CajaNoAbiertaException();

        var (ingresos, egresos) = await _movimientoCajaRepository.ObtenerTotalesAsync(caja.Id, cancellationToken);
        var montoTeorico = caja.MontoApertura + ingresos - egresos;

        caja.Cerrar(montoTeorico, comando.MontoFisicoDeclarado, _fechaHoraProvider.UtcAhora);
        await _cajaRepository.GuardarCambiosAsync(cancellationToken);

        return caja.ADto();
    }
}
