using ISARMIN.Domain.Entities.Identidad;
using Xunit;

namespace ISARMIN.Domain.Tests.Entities.Identidad;

public class UsuarioRolesTests
{
    private static Usuario CrearUsuario() => new("Ana Pérez", "aperez", "hash", DateTime.UtcNow);

    [Fact]
    public void AsignarRol_MismoRolDosVeces_NoDuplica()
    {
        var usuario = CrearUsuario();
        var rolId = Guid.NewGuid();

        usuario.AsignarRol(rolId);
        usuario.AsignarRol(rolId);

        Assert.Single(usuario.Roles);
    }

    [Fact]
    public void QuitarRol_RolAsignado_LoElimina()
    {
        var usuario = CrearUsuario();
        var rolId = Guid.NewGuid();
        usuario.AsignarRol(rolId);

        usuario.QuitarRol(rolId);

        Assert.Empty(usuario.Roles);
    }

    [Fact]
    public void ReemplazarRoles_AgregaLosNuevosYQuitaLosNoIncluidos()
    {
        var usuario = CrearUsuario();
        var rolVentas = Guid.NewGuid();
        var rolTecnico = Guid.NewGuid();
        var rolAdmin = Guid.NewGuid();
        usuario.AsignarRol(rolVentas);
        usuario.AsignarRol(rolTecnico);

        usuario.ReemplazarRoles([rolTecnico, rolAdmin]);

        var idsResultantes = usuario.Roles.Select(r => r.RolId).ToHashSet();
        Assert.Equal(2, idsResultantes.Count);
        Assert.Contains(rolTecnico, idsResultantes);
        Assert.Contains(rolAdmin, idsResultantes);
        Assert.DoesNotContain(rolVentas, idsResultantes);
    }
}
