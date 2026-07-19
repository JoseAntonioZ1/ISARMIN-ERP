using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Usuarios;
using ISARMIN.Application.Modulos.Usuarios.Commands.IniciarSesion;
using ISARMIN.Domain.Entities.Identidad;
using Moq;
using Xunit;

namespace ISARMIN.Application.Tests.Modulos.Usuarios;

public class IniciarSesionCommandHandlerTests
{
    private static readonly DateTime Ahora = new(2026, 7, 19, 10, 0, 0, DateTimeKind.Utc);

    private readonly Mock<IUsuarioRepository> _usuarioRepository = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<IGeneradorTokenJwt> _generadorTokenJwt = new();
    private readonly Mock<IFechaHoraProvider> _fechaHoraProvider = new();
    private readonly IValidator<IniciarSesionCommand> _validator = new IniciarSesionCommandValidator();

    private IniciarSesionCommandHandler CrearHandler()
    {
        _fechaHoraProvider.Setup(f => f.UtcAhora).Returns(Ahora);

        return new IniciarSesionCommandHandler(
            _usuarioRepository.Object,
            _passwordHasher.Object,
            _generadorTokenJwt.Object,
            _fechaHoraProvider.Object,
            _validator);
    }

    private static Usuario CrearUsuarioActivo() => new("Ana Pérez", "aperez", "hash-almacenado", Ahora);

    [Fact]
    public async Task ManejarAsync_CredencialesCorrectas_RetornaSesionYReiniciaIntentos()
    {
        var usuario = CrearUsuarioActivo();
        _usuarioRepository.Setup(r => r.ObtenerPorNombreUsuarioAsync("aperez", It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);
        _passwordHasher.Setup(h => h.VerificarCredencial("hash-almacenado", "clave-correcta")).Returns(true);
        _usuarioRepository.Setup(r => r.ObtenerPermisosEfectivosAsync(usuario.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { "Ventas.Crear" });
        _generadorTokenJwt.Setup(g => g.Generar(usuario, It.IsAny<IReadOnlyCollection<string>>()))
            .Returns(new TokenGenerado("token-jwt", Ahora.AddHours(1)));

        var handler = CrearHandler();
        var resultado = await handler.ManejarAsync(new IniciarSesionCommand("aperez", "clave-correcta"));

        Assert.Equal("token-jwt", resultado.Token);
        Assert.Equal(0, usuario.IntentosFallidos);
        _usuarioRepository.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ManejarAsync_UsuarioNoExiste_LanzaCredencialesInvalidas()
    {
        _usuarioRepository.Setup(r => r.ObtenerPorNombreUsuarioAsync("desconocido", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario?)null);

        var handler = CrearHandler();

        await Assert.ThrowsAsync<CredencialesInvalidasException>(() =>
            handler.ManejarAsync(new IniciarSesionCommand("desconocido", "cualquiera")));
    }

    [Fact]
    public async Task ManejarAsync_UsuarioInactivo_LanzaCredencialesInvalidas()
    {
        var usuario = CrearUsuarioActivo();
        usuario.Desactivar();
        _usuarioRepository.Setup(r => r.ObtenerPorNombreUsuarioAsync("aperez", It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);

        var handler = CrearHandler();

        await Assert.ThrowsAsync<CredencialesInvalidasException>(() =>
            handler.ManejarAsync(new IniciarSesionCommand("aperez", "clave-correcta")));
    }

    [Fact]
    public async Task ManejarAsync_UsuarioBloqueado_LanzaCuentaBloqueada()
    {
        var usuario = CrearUsuarioActivo();
        for (var i = 0; i < Usuario.MaxIntentosFallidos; i++)
        {
            usuario.RegistrarIntentoFallido(Ahora);
        }

        _usuarioRepository.Setup(r => r.ObtenerPorNombreUsuarioAsync("aperez", It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);

        var handler = CrearHandler();

        await Assert.ThrowsAsync<CuentaBloqueadaException>(() =>
            handler.ManejarAsync(new IniciarSesionCommand("aperez", "clave-incorrecta")));
    }

    [Fact]
    public async Task ManejarAsync_CredencialIncorrecta_RegistraIntentoFallidoYGuarda()
    {
        var usuario = CrearUsuarioActivo();
        _usuarioRepository.Setup(r => r.ObtenerPorNombreUsuarioAsync("aperez", It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);
        _passwordHasher.Setup(h => h.VerificarCredencial("hash-almacenado", "clave-incorrecta")).Returns(false);

        var handler = CrearHandler();

        await Assert.ThrowsAsync<CredencialesInvalidasException>(() =>
            handler.ManejarAsync(new IniciarSesionCommand("aperez", "clave-incorrecta")));

        Assert.Equal(1, usuario.IntentosFallidos);
        _usuarioRepository.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
