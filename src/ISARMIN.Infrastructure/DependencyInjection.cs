using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Configuracion;
using ISARMIN.Application.Modulos.Inventario;
using ISARMIN.Application.Modulos.Usuarios;
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
        services.AddSingleton<IPasswordHasher, PasswordHasherAdapter>();
        services.AddSingleton<IGeneradorTokenJwt, GeneradorTokenJwt>();
        services.AddSingleton<IFechaHoraProvider, FechaHoraProvider>();

        return services;
    }
}
