using System.Reflection;
using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Clientes.Commands.CambiarEstadoCliente;
using ISARMIN.Application.Modulos.Clientes.Commands.EditarCliente;
using ISARMIN.Application.Modulos.Clientes.Commands.RegistrarCliente;
using ISARMIN.Application.Modulos.Clientes.DTOs;
using ISARMIN.Application.Modulos.Clientes.Queries.BuscarClientes;
using ISARMIN.Application.Modulos.Configuracion.Commands.CambiarEstadoMedioPago;
using ISARMIN.Application.Modulos.Configuracion.Commands.CrearMedioPago;
using ISARMIN.Application.Modulos.Configuracion.DTOs;
using ISARMIN.Application.Modulos.Configuracion.Queries.ListarMediosPago;
using ISARMIN.Application.Modulos.Inventario.Commands.CrearCategoria;
using ISARMIN.Application.Modulos.Inventario.Commands.EditarCategoria;
using ISARMIN.Application.Modulos.Inventario.DTOs;
using ISARMIN.Application.Modulos.Inventario.Queries.ListarCategorias;
using ISARMIN.Application.Modulos.Usuarios.Commands.AsignarPermisos;
using ISARMIN.Application.Modulos.Usuarios.Commands.CambiarEstadoUsuario;
using ISARMIN.Application.Modulos.Usuarios.Commands.CrearRol;
using ISARMIN.Application.Modulos.Usuarios.Commands.CrearUsuario;
using ISARMIN.Application.Modulos.Usuarios.Commands.EditarRol;
using ISARMIN.Application.Modulos.Usuarios.Commands.EditarUsuario;
using ISARMIN.Application.Modulos.Usuarios.Commands.IniciarSesion;
using ISARMIN.Application.Modulos.Usuarios.Commands.RestablecerCredencial;
using ISARMIN.Application.Modulos.Usuarios.DTOs;
using ISARMIN.Application.Modulos.Usuarios.Queries.ListarRoles;
using ISARMIN.Application.Modulos.Usuarios.Queries.ListarRolesConPermisos;
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
        services.AddScoped<ICommandHandler<CrearRolCommand, RolDto>, CrearRolCommandHandler>();
        services.AddScoped<ICommandHandler<EditarRolCommand, RolDto>, EditarRolCommandHandler>();
        services.AddScoped<ICommandHandler<AsignarPermisosCommand, RolDto>, AsignarPermisosCommandHandler>();
        services.AddScoped<IQueryHandler<ListarRolesConPermisosQuery, IReadOnlyCollection<RolDto>>, ListarRolesConPermisosQueryHandler>();

        services.AddScoped<ICommandHandler<CrearCategoriaCommand, CategoriaDto>, CrearCategoriaCommandHandler>();
        services.AddScoped<ICommandHandler<EditarCategoriaCommand, CategoriaDto>, EditarCategoriaCommandHandler>();
        services.AddScoped<IQueryHandler<ListarCategoriasQuery, IReadOnlyCollection<CategoriaDto>>, ListarCategoriasQueryHandler>();
        services.AddScoped<ICommandHandler<CrearMedioPagoCommand, MedioPagoDto>, CrearMedioPagoCommandHandler>();
        services.AddScoped<ICommandHandler<CambiarEstadoMedioPagoCommand, Unit>, CambiarEstadoMedioPagoCommandHandler>();
        services.AddScoped<IQueryHandler<ListarMediosPagoQuery, IReadOnlyCollection<MedioPagoDto>>, ListarMediosPagoQueryHandler>();

        services.AddScoped<ICommandHandler<RegistrarClienteCommand, ClienteDto>, RegistrarClienteCommandHandler>();
        services.AddScoped<ICommandHandler<EditarClienteCommand, ClienteDto>, EditarClienteCommandHandler>();
        services.AddScoped<ICommandHandler<CambiarEstadoClienteCommand, Unit>, CambiarEstadoClienteCommandHandler>();
        services.AddScoped<IQueryHandler<BuscarClientesQuery, ListadoPaginadoDto<ClienteDto>>, BuscarClientesQueryHandler>();

        return services;
    }
}
