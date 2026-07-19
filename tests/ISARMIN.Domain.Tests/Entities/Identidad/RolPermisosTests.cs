using ISARMIN.Domain.Entities.Identidad;
using ISARMIN.Domain.Enums;
using Xunit;

namespace ISARMIN.Domain.Tests.Entities.Identidad;

public class RolPermisosTests
{
    [Fact]
    public void AsignarPermiso_MismoPermisoDosVeces_NoDuplica()
    {
        var rol = new Rol("Ventas");

        rol.AsignarPermiso("Clientes", AccionPermiso.Crear);
        rol.AsignarPermiso("Clientes", AccionPermiso.Crear);

        Assert.Single(rol.Permisos);
    }

    [Fact]
    public void QuitarPermiso_PermisoAsignado_LoElimina()
    {
        var rol = new Rol("Ventas");
        rol.AsignarPermiso("Clientes", AccionPermiso.Crear);

        rol.QuitarPermiso("Clientes", AccionPermiso.Crear);

        Assert.Empty(rol.Permisos);
    }

    [Fact]
    public void ReemplazarPermisos_AgregaLosNuevosYQuitaLosNoIncluidos()
    {
        var rol = new Rol("Ventas");
        rol.AsignarPermiso("Clientes", AccionPermiso.Crear);
        rol.AsignarPermiso("Clientes", AccionPermiso.Consultar);

        rol.ReemplazarPermisos([("Clientes", AccionPermiso.Consultar), ("Ventas", AccionPermiso.Crear)]);

        var claves = rol.Permisos.Select(p => (p.Modulo, p.Accion)).ToHashSet();
        Assert.Equal(2, claves.Count);
        Assert.Contains(("Clientes", AccionPermiso.Consultar), claves);
        Assert.Contains(("Ventas", AccionPermiso.Crear), claves);
        Assert.DoesNotContain(("Clientes", AccionPermiso.Crear), claves);
    }

    [Fact]
    public void ActualizarDatos_NombreVacio_LanzaExcepcion()
    {
        var rol = new Rol("Ventas");

        Assert.Throws<ArgumentException>(() => rol.ActualizarDatos("", null));
    }
}
