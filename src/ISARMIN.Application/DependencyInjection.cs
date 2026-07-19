using System.Reflection;
using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Usuarios.Commands.IniciarSesion;
using ISARMIN.Application.Modulos.Usuarios.DTOs;
using Microsoft.Extensions.DependencyInjection;

namespace ISARMIN.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var applicationAssembly = Assembly.GetExecutingAssembly();

        services.AddAutoMapper(cfg => { }, applicationAssembly);
        services.AddValidatorsFromAssembly(applicationAssembly);

        services.AddScoped<ICommandHandler<IniciarSesionCommand, SesionDto>, IniciarSesionCommandHandler>();

        return services;
    }
}
