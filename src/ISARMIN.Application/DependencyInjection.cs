using System.Reflection;
using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Clientes.Commands.CambiarEstadoCliente;
using ISARMIN.Application.Modulos.Clientes.Commands.EditarCliente;
using ISARMIN.Application.Modulos.Clientes.Commands.RegistrarCliente;
using ISARMIN.Application.Modulos.Clientes.DTOs;
using ISARMIN.Application.Modulos.Caja.Commands.AbrirCaja;
using ISARMIN.Application.Modulos.Caja.Commands.CerrarCaja;
using ISARMIN.Application.Modulos.Caja.Commands.RegistrarMovimientoCaja;
using ISARMIN.Application.Modulos.Caja.DTOs;
using ISARMIN.Application.Modulos.Caja.Queries.ListarMovimientosCaja;
using ISARMIN.Application.Modulos.Caja.Queries.ObtenerCajaActual;
using ISARMIN.Application.Modulos.Clientes.Queries.BuscarClientes;
using ISARMIN.Application.Modulos.Compras.Commands.RegistrarCompra;
using ISARMIN.Application.Modulos.Compras.DTOs;
using ISARMIN.Application.Modulos.Compras.Queries.BuscarCompras;
using ISARMIN.Application.Modulos.Compras.Queries.ObtenerCompra;
using ISARMIN.Application.Modulos.Configuracion.Commands.CambiarEstadoMedioPago;
using ISARMIN.Application.Modulos.Configuracion.Commands.CrearMedioPago;
using ISARMIN.Application.Modulos.Configuracion.Commands.ActualizarConfiguracionEmpresa;
using ISARMIN.Application.Modulos.Configuracion.DTOs;
using ISARMIN.Application.Modulos.Configuracion.Queries.ListarMediosPago;
using ISARMIN.Application.Modulos.Configuracion.Queries.ObtenerConfiguracionEmpresa;
using ISARMIN.Application.Modulos.Inventario.Commands.AjustarInventario;
using ISARMIN.Application.Modulos.Inventario.Commands.CambiarEstadoProducto;
using ISARMIN.Application.Modulos.Inventario.Commands.CrearCategoria;
using ISARMIN.Application.Modulos.Inventario.Commands.CrearUnidadMedida;
using ISARMIN.Application.Modulos.Inventario.Commands.EditarCategoria;
using ISARMIN.Application.Modulos.Inventario.Commands.EditarProducto;
using ISARMIN.Application.Modulos.Inventario.Commands.EditarUnidadMedida;
using ISARMIN.Application.Modulos.Inventario.Commands.RegistrarProducto;
using ISARMIN.Application.Modulos.Inventario.DTOs;
using ISARMIN.Application.Modulos.Inventario.Queries.BuscarProductos;
using ISARMIN.Application.Modulos.Inventario.Queries.ConsultarKardex;
using ISARMIN.Application.Modulos.Inventario.Queries.ListarCategorias;
using ISARMIN.Application.Modulos.Inventario.Queries.ListarUnidadesMedida;
using ISARMIN.Application.Modulos.Proveedores.Commands.CambiarEstadoProveedor;
using ISARMIN.Application.Modulos.Proveedores.Commands.EditarProveedor;
using ISARMIN.Application.Modulos.Proveedores.Commands.RegistrarProveedor;
using ISARMIN.Application.Modulos.Proveedores.DTOs;
using ISARMIN.Application.Modulos.Proveedores.Queries.BuscarProveedores;
using ISARMIN.Application.Modulos.Taller.Commands.EntregarEquipo;
using ISARMIN.Application.Modulos.Taller.Commands.GenerarCotizacionReparacion;
using ISARMIN.Application.Modulos.Taller.Commands.RegistrarDecisionCliente;
using ISARMIN.Application.Modulos.Taller.Commands.RegistrarDiagnostico;
using ISARMIN.Application.Modulos.Taller.Commands.RegistrarGarantia;
using ISARMIN.Application.Modulos.Taller.Commands.RegistrarRecepcion;
using ISARMIN.Application.Modulos.Taller.Commands.RegistrarReparacion;
using ISARMIN.Application.Modulos.Taller.DTOs;
using ISARMIN.Application.Modulos.Taller.Queries.BuscarOrdenesTrabajo;
using ISARMIN.Application.Modulos.Taller.Queries.ObtenerOrdenTrabajo;
using ISARMIN.Application.Modulos.ServiciosCampo.Commands.SolicitarServicioCampo;
using ISARMIN.Application.Modulos.ServiciosCampo.Commands.CotizarServicioCampo;
using ISARMIN.Application.Modulos.ServiciosCampo.Commands.CerrarServicioCampo;
using ISARMIN.Application.Modulos.ServiciosCampo.Commands.CobrarServicioCampo;
using ISARMIN.Application.Modulos.ServiciosCampo.DTOs;
using ISARMIN.Application.Modulos.ServiciosCampo.Queries.BuscarServiciosCampo;
using ISARMIN.Application.Modulos.ServiciosCampo.Queries.ObtenerServicioCampo;
using ISARMIN.Application.Modulos.Ventas.Commands.RegistrarVenta;
using ISARMIN.Application.Modulos.Ventas.Commands.AnularVenta;
using ISARMIN.Application.Modulos.Ventas.Commands.RegistrarDevolucion;
using ISARMIN.Application.Modulos.Ventas.DTOs;
using ISARMIN.Application.Modulos.Ventas.Queries.BuscarVentas;
using ISARMIN.Application.Modulos.Ventas.Queries.ObtenerVenta;
using ISARMIN.Application.Modulos.Reportes.DTOs;
using ISARMIN.Application.Modulos.Reportes.Queries.ReporteVentas;
using ISARMIN.Application.Modulos.Reportes.Queries.ReporteInventario;
using ISARMIN.Application.Modulos.Reportes.Queries.ReporteOrdenesTrabajo;
using ISARMIN.Application.Modulos.Reportes.Queries.ReporteServiciosCampo;
using ISARMIN.Application.Modulos.Reportes.Queries.ReporteCaja;
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
        services.AddScoped<ICommandHandler<CrearUnidadMedidaCommand, UnidadMedidaDto>, CrearUnidadMedidaCommandHandler>();
        services.AddScoped<ICommandHandler<EditarUnidadMedidaCommand, UnidadMedidaDto>, EditarUnidadMedidaCommandHandler>();
        services.AddScoped<IQueryHandler<ListarUnidadesMedidaQuery, IReadOnlyCollection<UnidadMedidaDto>>, ListarUnidadesMedidaQueryHandler>();
        services.AddScoped<ICommandHandler<RegistrarProductoCommand, ProductoDto>, RegistrarProductoCommandHandler>();
        services.AddScoped<ICommandHandler<EditarProductoCommand, ProductoDto>, EditarProductoCommandHandler>();
        services.AddScoped<ICommandHandler<CambiarEstadoProductoCommand, Unit>, CambiarEstadoProductoCommandHandler>();
        services.AddScoped<IQueryHandler<BuscarProductosQuery, ListadoPaginadoDto<ProductoDto>>, BuscarProductosQueryHandler>();
        services.AddScoped<ICommandHandler<AjustarInventarioCommand, ProductoDto>, AjustarInventarioCommandHandler>();
        services.AddScoped<IQueryHandler<ConsultarKardexQuery, IReadOnlyCollection<MovimientoInventarioDto>>, ConsultarKardexQueryHandler>();
        services.AddScoped<ICommandHandler<CrearMedioPagoCommand, MedioPagoDto>, CrearMedioPagoCommandHandler>();
        services.AddScoped<ICommandHandler<CambiarEstadoMedioPagoCommand, Unit>, CambiarEstadoMedioPagoCommandHandler>();
        services.AddScoped<IQueryHandler<ListarMediosPagoQuery, IReadOnlyCollection<MedioPagoDto>>, ListarMediosPagoQueryHandler>();
        services.AddScoped<IQueryHandler<ObtenerConfiguracionEmpresaQuery, ConfiguracionEmpresaDto>, ObtenerConfiguracionEmpresaQueryHandler>();
        services.AddScoped<ICommandHandler<ActualizarConfiguracionEmpresaCommand, ConfiguracionEmpresaDto>, ActualizarConfiguracionEmpresaCommandHandler>();

        services.AddScoped<ICommandHandler<RegistrarClienteCommand, ClienteDto>, RegistrarClienteCommandHandler>();
        services.AddScoped<ICommandHandler<EditarClienteCommand, ClienteDto>, EditarClienteCommandHandler>();
        services.AddScoped<ICommandHandler<CambiarEstadoClienteCommand, Unit>, CambiarEstadoClienteCommandHandler>();
        services.AddScoped<IQueryHandler<BuscarClientesQuery, ListadoPaginadoDto<ClienteDto>>, BuscarClientesQueryHandler>();

        services.AddScoped<ICommandHandler<RegistrarProveedorCommand, ProveedorDto>, RegistrarProveedorCommandHandler>();
        services.AddScoped<ICommandHandler<EditarProveedorCommand, ProveedorDto>, EditarProveedorCommandHandler>();
        services.AddScoped<ICommandHandler<CambiarEstadoProveedorCommand, Unit>, CambiarEstadoProveedorCommandHandler>();
        services.AddScoped<IQueryHandler<BuscarProveedoresQuery, ListadoPaginadoDto<ProveedorDto>>, BuscarProveedoresQueryHandler>();

        services.AddScoped<ICommandHandler<RegistrarCompraCommand, CompraDto>, RegistrarCompraCommandHandler>();
        services.AddScoped<IQueryHandler<BuscarComprasQuery, ListadoPaginadoDto<CompraDto>>, BuscarComprasQueryHandler>();
        services.AddScoped<IQueryHandler<ObtenerCompraQuery, CompraDto>, ObtenerCompraQueryHandler>();

        services.AddScoped<ICommandHandler<AbrirCajaCommand, CajaDto>, AbrirCajaCommandHandler>();
        services.AddScoped<ICommandHandler<CerrarCajaCommand, CajaDto>, CerrarCajaCommandHandler>();
        services.AddScoped<ICommandHandler<RegistrarMovimientoCajaCommand, MovimientoCajaDto>, RegistrarMovimientoCajaCommandHandler>();
        services.AddScoped<IQueryHandler<ObtenerCajaActualQuery, CajaDto?>, ObtenerCajaActualQueryHandler>();
        services.AddScoped<IQueryHandler<ListarMovimientosCajaQuery, IReadOnlyCollection<MovimientoCajaDto>>, ListarMovimientosCajaQueryHandler>();

        services.AddScoped<ICommandHandler<RegistrarRecepcionCommand, OrdenTrabajoDto>, RegistrarRecepcionCommandHandler>();
        services.AddScoped<ICommandHandler<RegistrarDiagnosticoCommand, OrdenTrabajoDto>, RegistrarDiagnosticoCommandHandler>();
        services.AddScoped<ICommandHandler<GenerarCotizacionReparacionCommand, OrdenTrabajoDto>, GenerarCotizacionReparacionCommandHandler>();
        services.AddScoped<ICommandHandler<RegistrarDecisionClienteCommand, OrdenTrabajoDto>, RegistrarDecisionClienteCommandHandler>();
        services.AddScoped<ICommandHandler<RegistrarReparacionCommand, OrdenTrabajoDto>, RegistrarReparacionCommandHandler>();
        services.AddScoped<ICommandHandler<EntregarEquipoCommand, OrdenTrabajoDto>, EntregarEquipoCommandHandler>();
        services.AddScoped<ICommandHandler<RegistrarGarantiaCommand, GarantiaDto>, RegistrarGarantiaCommandHandler>();
        services.AddScoped<IQueryHandler<BuscarOrdenesTrabajoQuery, ListadoPaginadoDto<OrdenTrabajoDto>>, BuscarOrdenesTrabajoQueryHandler>();
        services.AddScoped<IQueryHandler<ObtenerOrdenTrabajoQuery, OrdenTrabajoDetalleDto>, ObtenerOrdenTrabajoQueryHandler>();

        services.AddScoped<ICommandHandler<SolicitarServicioCampoCommand, ServicioCampoDto>, SolicitarServicioCampoCommandHandler>();
        services.AddScoped<ICommandHandler<CotizarServicioCampoCommand, ServicioCampoDto>, CotizarServicioCampoCommandHandler>();
        services.AddScoped<ICommandHandler<CerrarServicioCampoCommand, ServicioCampoDto>, CerrarServicioCampoCommandHandler>();
        services.AddScoped<ICommandHandler<CobrarServicioCampoCommand, ServicioCampoDto>, CobrarServicioCampoCommandHandler>();
        services.AddScoped<IQueryHandler<BuscarServiciosCampoQuery, ListadoPaginadoDto<ServicioCampoDto>>, BuscarServiciosCampoQueryHandler>();
        services.AddScoped<IQueryHandler<ObtenerServicioCampoQuery, ServicioCampoDto>, ObtenerServicioCampoQueryHandler>();

        services.AddScoped<ICommandHandler<RegistrarVentaCommand, VentaDto>, RegistrarVentaCommandHandler>();
        services.AddScoped<ICommandHandler<AnularVentaCommand, VentaDto>, AnularVentaCommandHandler>();
        services.AddScoped<ICommandHandler<RegistrarDevolucionCommand, VentaDto>, RegistrarDevolucionCommandHandler>();
        services.AddScoped<IQueryHandler<BuscarVentasQuery, ListadoPaginadoDto<VentaDto>>, BuscarVentasQueryHandler>();
        services.AddScoped<IQueryHandler<ObtenerVentaQuery, VentaDto>, ObtenerVentaQueryHandler>();

        services.AddScoped<IQueryHandler<ReporteVentasQuery, ReporteVentasDto>, ReporteVentasQueryHandler>();
        services.AddScoped<IQueryHandler<ReporteInventarioQuery, ReporteInventarioDto>, ReporteInventarioQueryHandler>();
        services.AddScoped<IQueryHandler<ReporteOrdenesTrabajoQuery, ReporteOrdenesTrabajoDto>, ReporteOrdenesTrabajoQueryHandler>();
        services.AddScoped<IQueryHandler<ReporteServiciosCampoQuery, ReporteServiciosCampoDto>, ReporteServiciosCampoQueryHandler>();
        services.AddScoped<IQueryHandler<ReporteCajaQuery, ReporteCajaDto>, ReporteCajaQueryHandler>();

        return services;
    }
}
