using ISARMIN.Domain.Entities.Identidad;
using Xunit;

namespace ISARMIN.Domain.Tests.Entities.Identidad;

public class UsuarioTests
{
    private static Usuario CrearUsuario(DateTime ahora) =>
        new("Ana Pérez", "aperez", "hash-credencial", ahora);

    [Fact]
    public void RegistrarIntentoFallido_AntesDelMaximo_NoBloquea()
    {
        var ahora = DateTime.UtcNow;
        var usuario = CrearUsuario(ahora);

        for (var i = 0; i < Usuario.MaxIntentosFallidos - 1; i++)
        {
            usuario.RegistrarIntentoFallido(ahora);
        }

        Assert.False(usuario.EstaBloqueado(ahora));
        Assert.Equal(Usuario.MaxIntentosFallidos - 1, usuario.IntentosFallidos);
    }

    [Fact]
    public void RegistrarIntentoFallido_AlAlcanzarElMaximo_Bloquea15Minutos()
    {
        var ahora = DateTime.UtcNow;
        var usuario = CrearUsuario(ahora);

        for (var i = 0; i < Usuario.MaxIntentosFallidos; i++)
        {
            usuario.RegistrarIntentoFallido(ahora);
        }

        Assert.True(usuario.EstaBloqueado(ahora));
        Assert.Equal(ahora.Add(Usuario.DuracionBloqueo), usuario.BloqueadoHasta);
    }

    [Fact]
    public void EstaBloqueado_TrasExpirarElBloqueo_YaNoEstaBloqueado()
    {
        var ahora = DateTime.UtcNow;
        var usuario = CrearUsuario(ahora);

        for (var i = 0; i < Usuario.MaxIntentosFallidos; i++)
        {
            usuario.RegistrarIntentoFallido(ahora);
        }

        var despuesDelBloqueo = ahora.Add(Usuario.DuracionBloqueo).AddSeconds(1);

        Assert.False(usuario.EstaBloqueado(despuesDelBloqueo));
    }

    [Fact]
    public void RegistrarInicioSesionExitoso_ReiniciaIntentosYDesbloquea()
    {
        var ahora = DateTime.UtcNow;
        var usuario = CrearUsuario(ahora);

        for (var i = 0; i < Usuario.MaxIntentosFallidos; i++)
        {
            usuario.RegistrarIntentoFallido(ahora);
        }

        usuario.RegistrarInicioSesionExitoso();

        Assert.Equal(0, usuario.IntentosFallidos);
        Assert.False(usuario.EstaBloqueado(ahora));
    }

    [Fact]
    public void PuedeAutenticarse_UsuarioInactivo_RetornaFalse()
    {
        var ahora = DateTime.UtcNow;
        var usuario = CrearUsuario(ahora);
        usuario.Desactivar();

        Assert.False(usuario.PuedeAutenticarse(ahora));
    }
}
