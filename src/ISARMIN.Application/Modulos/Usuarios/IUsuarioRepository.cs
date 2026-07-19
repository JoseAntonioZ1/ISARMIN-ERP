using ISARMIN.Domain.Entities.Identidad;

namespace ISARMIN.Application.Modulos.Usuarios;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario, CancellationToken cancellationToken = default);

    Task<Usuario?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyCollection<Usuario> Usuarios, int Total)> ListarAsync(int pagina, int tamanoPagina, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<string>> ObtenerPermisosEfectivosAsync(Guid usuarioId, CancellationToken cancellationToken = default);

    void Agregar(Usuario usuario);

    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
