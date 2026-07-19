using System.Reflection;
using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Usuarios.Commands.CambiarEstadoUsuario;
using ISARMIN.Application.Modulos.Usuarios.Commands.CrearUsuario;
using ISARMIN.Application.Modulos.Usuarios.Commands.EditarUsuario;
using ISARMIN.Application.Modulos.Usuarios.Commands.IniciarSesion;
using ISARMIN.Application.Modulos.Usuarios.Commands.RestablecerCredencial;
using ISARMIN.Application.Modulos.Usuarios.DTOs;
using ISARMIN.Application.Modulos.Usuarios.Queries.ListarRoles;
using ISARMIN.Application.Modulos.Usuarios.Queries.ListarUsuarios;
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
        services.AddScoped<ICommandHandler<CrearUsuarioCommand, UsuarioDto>, CrearUsuarioCommandHandler>();
        services.AddScoped<ICommandHandler<EditarUsuarioCommand, UsuarioDto>, EditarUsuarioCommandHandler>();
        services.AddScoped<ICommandHandler<CambiarEstadoUsuarioCommand, Unit>, CambiarEstadoUsuarioCommandHandler>();
        services.AddScoped<ICommandHandler<RestablecerCredencialCommand, Unit>, RestablecerCredencialCommandHandler>();
        services.AddScoped<IQueryHandler<ListarUsuariosQuery, ListadoPaginadoDto<UsuarioDto>>, ListarUsuariosQueryHandler>();
        services.AddScoped<IQueryHandler<ListarRolesQuery, IReadOnlyCollection<RolResumenDto>>, ListarRolesQueryHandler>();

        return services;
    }
}
