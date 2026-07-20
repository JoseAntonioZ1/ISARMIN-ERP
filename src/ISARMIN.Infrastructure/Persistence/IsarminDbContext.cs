using ISARMIN.Domain.Entities.Compras;
using ISARMIN.Domain.Entities.Configuracion;
using ISARMIN.Domain.Entities.Identidad;
using ISARMIN.Domain.Entities.Inventario;
using ISARMIN.Domain.Entities.Taller;
using ISARMIN.Domain.Entities.Terceros;
using Microsoft.EntityFrameworkCore;
using CajaEntity = ISARMIN.Domain.Entities.Caja.Caja;
using MovimientoCajaEntity = ISARMIN.Domain.Entities.Caja.MovimientoCaja;

namespace ISARMIN.Infrastructure.Persistence;

public class IsarminDbContext : DbContext
{
    public IsarminDbContext(DbContextOptions<IsarminDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Permiso> Permisos => Set<Permiso>();
    public DbSet<UsuarioRol> UsuarioRoles => Set<UsuarioRol>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<MedioPago> MediosPago => Set<MedioPago>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<UnidadMedida> UnidadesMedida => Set<UnidadMedida>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<MovimientoInventario> MovimientosInventario => Set<MovimientoInventario>();
    public DbSet<Compra> Compras => Set<Compra>();
    public DbSet<CompraDetalle> ComprasDetalle => Set<CompraDetalle>();
    public DbSet<CajaEntity> Cajas => Set<CajaEntity>();
    public DbSet<MovimientoCajaEntity> MovimientosCaja => Set<MovimientoCajaEntity>();
    public DbSet<OrdenTrabajo> OrdenesTrabajo => Set<OrdenTrabajo>();
    public DbSet<Diagnostico> Diagnosticos => Set<Diagnostico>();
    public DbSet<CotizacionReparacion> CotizacionesReparacion => Set<CotizacionReparacion>();
    public DbSet<ConsumoRepuesto> ConsumosRepuesto => Set<ConsumoRepuesto>();
    public DbSet<Garantia> Garantias => Set<Garantia>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IsarminDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
