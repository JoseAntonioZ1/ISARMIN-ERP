using ISARMIN.Domain.Entities.Terceros;

namespace ISARMIN.Application.Modulos.Clientes;

public interface IClienteRepository
{
    Task<Cliente?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyCollection<Cliente> Clientes, int Total)> BuscarAsync(
        string? termino, int pagina, int tamanoPagina, CancellationToken cancellationToken = default);

    void Agregar(Cliente cliente);

    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
