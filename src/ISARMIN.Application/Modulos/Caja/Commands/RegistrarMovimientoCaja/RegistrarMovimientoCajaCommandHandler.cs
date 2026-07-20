using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Caja.DTOs;
using ISARMIN.Domain.Entities.Caja;

namespace ISARMIN.Application.Modulos.Caja.Commands.RegistrarMovimientoCaja;

/// <summary>UC-20/RF-047 — movimiento manual de caja (RN-026: gasto operativo, retiro del
/// propietario o aporte de capital), sin origen transaccional. RN-014: no puede registrarse sin una
/// caja abierta.</summary>
public class RegistrarMovimientoCajaCommandHandler : ICommandHandler<RegistrarMovimientoCajaCommand, MovimientoCajaDto>
{
    private readonly ICajaRepository _cajaRepository;
    private readonly IMovimientoCajaRepository _movimientoCajaRepository;
    private readonly IFechaHoraProvider _fechaHoraProvider;
    private readonly IValidator<RegistrarMovimientoCajaCommand> _validator;

    public RegistrarMovimientoCajaCommandHandler(
        ICajaRepository cajaRepository,
        IMovimientoCajaRepository movimientoCajaRepository,
        IFechaHoraProvider fechaHoraProvider,
        IValidator<RegistrarMovimientoCajaCommand> validator)
    {
        _cajaRepository = cajaRepository;
        _movimientoCajaRepository = movimientoCajaRepository;
        _fechaHoraProvider = fechaHoraProvider;
        _validator = validator;
    }

    public async Task<MovimientoCajaDto> ManejarAsync(RegistrarMovimientoCajaCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        var caja = await _cajaRepository.ObtenerAbiertaAsync(cancellationToken)
            ?? throw new CajaNoAbiertaException();

        var movimiento = MovimientoCaja.Registrar(
            caja.Id, comando.Concepto, comando.Monto, comando.Descripcion, comando.UsuarioId, _fechaHoraProvider.UtcAhora);

        _movimientoCajaRepository.Agregar(movimiento);
        await _movimientoCajaRepository.GuardarCambiosAsync(cancellationToken);

        return movimiento.ADto();
    }
}
