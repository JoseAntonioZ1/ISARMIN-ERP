using Microsoft.EntityFrameworkCore;

namespace ISARMIN.Infrastructure.Persistence;

public class IsarminDbContext : DbContext
{
    public IsarminDbContext(DbContextOptions<IsarminDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IsarminDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
