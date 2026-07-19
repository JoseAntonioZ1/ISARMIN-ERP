using ISARMIN.Application.Modulos.Clientes;
using ISARMIN.Domain.Entities.Terceros;
using ISARMIN.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ISARMIN.Infrastructure.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly IsarminDbContext _dbContext;

    public ClienteRepository(IsarminDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Cliente?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _dbContext.Clientes.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<(IReadOnlyCollection<Cliente> Clientes, int Total)> BuscarAsync(
        string? termino, int pagina, int tamanoPagina, CancellationToken cancellationToken = default)
    {
        var consulta = _dbContext.Clientes.AsQueryable();

        if (!string.IsNullOrWhiteSpace(termino))
        {
            consulta = consulta.Where(c =>
                EF.Functions.ILike(c.NombreRazonSocial, $"%{termino}%") ||
                (c.NumeroDocumento != null && EF.Functions.ILike(c.NumeroDocumento, $"%{termino}%")));
        }

        var total = await consulta.CountAsync(cancellationToken);

        var clientes = await consulta
            .OrderBy(c => c.NombreRazonSocial)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync(cancellationToken);

        return (clientes, total);
    }

    public void Agregar(Cliente cliente) => _dbContext.Clientes.Add(cliente);

    public Task GuardarCambiosAsync(CancellationToken cancellationToken = default) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
