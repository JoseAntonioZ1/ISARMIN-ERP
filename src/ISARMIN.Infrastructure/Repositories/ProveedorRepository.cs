using ISARMIN.Application.Modulos.Proveedores;
using ISARMIN.Domain.Entities.Terceros;
using ISARMIN.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ISARMIN.Infrastructure.Repositories;

public class ProveedorRepository : IProveedorRepository
{
    private readonly IsarminDbContext _dbContext;

    public ProveedorRepository(IsarminDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Proveedor?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _dbContext.Proveedores.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<(IReadOnlyCollection<Proveedor> Proveedores, int Total)> BuscarAsync(
        string? termino, int pagina, int tamanoPagina, CancellationToken cancellationToken = default)
    {
        var consulta = _dbContext.Proveedores.AsQueryable();

        if (!string.IsNullOrWhiteSpace(termino))
        {
            consulta = consulta.Where(p =>
                EF.Functions.ILike(p.NombreRazonSocial, $"%{termino}%") ||
                (p.Documento != null && EF.Functions.ILike(p.Documento, $"%{termino}%")));
        }

        var total = await consulta.CountAsync(cancellationToken);

        var proveedores = await consulta
            .OrderBy(p => p.NombreRazonSocial)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync(cancellationToken);

        return (proveedores, total);
    }

    public void Agregar(Proveedor proveedor) => _dbContext.Proveedores.Add(proveedor);

    public Task GuardarCambiosAsync(CancellationToken cancellationToken = default) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
