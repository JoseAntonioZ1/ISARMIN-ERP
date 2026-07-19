using ISARMIN.Domain.Entities.Identidad;

namespace ISARMIN.Application.Modulos.Usuarios;

public interface IRolRepository
{
    Task<IReadOnlyCollection<Guid>> ObtenerIdsExistentesAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Rol>> ListarTodosAsync(CancellationToken cancellationToken = default);
}
