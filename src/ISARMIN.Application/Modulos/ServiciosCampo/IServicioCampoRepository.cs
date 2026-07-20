using ISARMIN.Domain.Entities.ServiciosCampo;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Application.Modulos.ServiciosCampo;

public interface IServicioCampoRepository
{
    Task<ServicioCampo?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyCollection<ServicioCampo> Servicios, int Total)> BuscarAsync(
        EstadoServicioCampo? estado, Guid? clienteId, int pagina, int tamanoPagina, CancellationToken cancellationToken = default);

    void Agregar(ServicioCampo servicioCampo);

    /// <summary>Los detalles se agregan a una colección de un ServicioCampo ya rastreado (obtenido
    /// vía ObtenerPorIdAsync), no vía Agregar sobre un agregado nuevo, por lo que EF Core no los
    /// detecta como Added automáticamente (mismo problema ya resuelto en OrdenTrabajo/ConsumoRepuesto).</summary>
    void AgregarDetalles(IEnumerable<ServicioCampoDetalle> detalles);

    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
