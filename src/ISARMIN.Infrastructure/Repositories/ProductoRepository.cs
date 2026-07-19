using ISARMIN.Application.Modulos.Inventario;
using ISARMIN.Domain.Entities.Inventario;
using ISARMIN.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ISARMIN.Infrastructure.Repositories;

public class ProductoRepository : IProductoRepository
{
    private readonly IsarminDbContext _dbContext;

    public ProductoRepository(IsarminDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Producto?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _dbContext.Productos.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<Producto?> ObtenerPorCodigoInternoAsync(string codigoInterno, CancellationToken cancellationToken = default) =>
        _dbContext.Productos.FirstOrDefaultAsync(p => p.CodigoInterno == codigoInterno, cancellationToken);

    public Task<Producto?> ObtenerPorCodigoBarrasAsync(string codigoBarras, CancellationToken cancellationToken = default) =>
        _dbContext.Productos.FirstOrDefaultAsync(p => p.CodigoBarras == codigoBarras, cancellationToken);

    public async Task<(IReadOnlyCollection<Producto> Productos, int Total)> BuscarAsync(
        string? termino, Guid? categoriaId, int pagina, int tamanoPagina, CancellationToken cancellationToken = default)
    {
        var consulta = _dbContext.Productos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(termino))
        {
            consulta = consulta.Where(p =>
                EF.Functions.ILike(p.Nombre, $"%{termino}%") ||
                EF.Functions.ILike(p.CodigoInterno, $"%{termino}%") ||
                (p.CodigoBarras != null && EF.Functions.ILike(p.CodigoBarras, $"%{termino}%")));
        }

        if (categoriaId is { } id)
        {
            consulta = consulta.Where(p => p.CategoriaId == id);
        }

        var total = await consulta.CountAsync(cancellationToken);

        var productos = await consulta
            .OrderBy(p => p.Nombre)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync(cancellationToken);

        return (productos, total);
    }

    public void Agregar(Producto producto) => _dbContext.Productos.Add(producto);

    public Task GuardarCambiosAsync(CancellationToken cancellationToken = default) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
