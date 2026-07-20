using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Caja;
using ISARMIN.Application.Modulos.Clientes;
using ISARMIN.Application.Modulos.Compras;
using ISARMIN.Application.Modulos.Configuracion;
using ISARMIN.Application.Modulos.Inventario;
using ISARMIN.Application.Modulos.Proveedores;
using ISARMIN.Application.Modulos.Reportes;
using ISARMIN.Application.Modulos.ServiciosCampo;
using ISARMIN.Application.Modulos.Taller;
using ISARMIN.Application.Modulos.Usuarios;
using ISARMIN.Application.Modulos.Ventas;
using ISARMIN.Infrastructure.Auth;
using ISARMIN.Infrastructure.Persistence;
using ISARMIN.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ISARMIN.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'DefaultConnection'.");

        services.AddDbContext<IsarminDbContext>(options =>
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IRolRepository, RolRepository>();
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        services.AddScoped<IMedioPagoRepository, MedioPagoRepository>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IProveedorRepository, ProveedorRepository>();
        services.AddScoped<IUnidadMedidaRepository, UnidadMedidaRepository>();
        services.AddScoped<IProductoRepository, ProductoRepository>();
        services.AddScoped<IMovimientoInventarioRepository, MovimientoInventarioRepository>();
        services.AddScoped<ICompraRepository, CompraRepository>();
        services.AddScoped<ICajaRepository, CajaRepository>();
        services.AddScoped<IMovimientoCajaRepository, MovimientoCajaRepository>();
        services.AddScoped<IOrdenTrabajoRepository, OrdenTrabajoRepository>();
        services.AddScoped<IGarantiaRepository, GarantiaRepository>();
        services.AddScoped<IServicioCampoRepository, ServicioCampoRepository>();
        services.AddScoped<IVentaRepository, VentaRepository>();
        services.AddScoped<IReporteRepository, ReporteRepository>();
        services.AddSingleton<IPasswordHasher, PasswordHasherAdapter>();
        services.AddSingleton<IGeneradorTokenJwt, GeneradorTokenJwt>();
        services.AddSingleton<IFechaHoraProvider, FechaHoraProvider>();

        return services;
    }
}
