using ISARMIN.Domain.Entities.Taller;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Application.Modulos.Taller;

public interface IOrdenTrabajoRepository
{
    Task<OrdenTrabajo?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyCollection<OrdenTrabajo> OrdenesTrabajo, int Total)> BuscarAsync(
        EstadoOrdenTrabajo? estado, Guid? clienteId, int pagina, int tamanoPagina, CancellationToken cancellationToken = default);

    void Agregar(OrdenTrabajo ordenTrabajo);

    /// <summary>Registra explícitamente un Diagnostico recién creado en una OT ya rastreada por el
    /// contexto (ObtenerPorIdAsync). Necesario porque asignar la propiedad de navegación 1:1 en una
    /// entidad ya rastreada no basta para que EF Core detecte el hijo como Added: su Id (GUID
    /// generado en el cliente) nunca está en el valor CLR por defecto, así que el heurístico de EF
    /// lo clasifica como Modified y genera un UPDATE contra una fila que no existe.</summary>
    void AgregarDiagnostico(Diagnostico diagnostico);

    /// <summary>Mismo motivo que <see cref="AgregarDiagnostico"/>, para la cotización de reparación.</summary>
    void AgregarCotizacion(CotizacionReparacion cotizacion);

    /// <summary>Mismo motivo que <see cref="AgregarDiagnostico"/>, para los repuestos consumidos
    /// (colección agregada sobre una OT ya rastreada, no vía <c>Agregar</c> del agregado completo).</summary>
    void AgregarConsumosRepuesto(IEnumerable<ConsumoRepuesto> consumos);

    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
