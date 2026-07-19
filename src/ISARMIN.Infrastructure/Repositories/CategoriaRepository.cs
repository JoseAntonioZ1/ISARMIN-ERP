using ISARMIN.Application.Modulos.Inventario;
using ISARMIN.Domain.Entities.Inventario;
using ISARMIN.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ISARMIN.Infrastructure.Repositories;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly IsarminDbContext _dbContext;

    public CategoriaRepository(IsarminDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Categoria?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _dbContext.Categorias.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public Task<Categoria?> ObtenerPorNombreAsync(string nombre, CancellationToken cancellationToken = default) =>
        _dbContext.Categorias.FirstOrDefaultAsync(c => c.Nombre == nombre, cancellationToken);

    public Task<bool> ExisteAsync(Guid id, CancellationToken cancellationToken = default) =>
        _dbContext.Categorias.AnyAsync(c => c.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<Categoria>> ListarTodasAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Categorias.OrderBy(c => c.Nombre).ToListAsync(cancellationToken);

    public void Agregar(Categoria categoria) => _dbContext.Categorias.Add(categoria);

    public Task GuardarCambiosAsync(CancellationToken cancellationToken = default) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
