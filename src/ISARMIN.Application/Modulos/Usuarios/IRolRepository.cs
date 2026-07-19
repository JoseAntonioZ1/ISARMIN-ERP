using ISARMIN.Domain.Entities.Identidad;

namespace ISARMIN.Application.Modulos.Usuarios;

public interface IRolRepository
{
    Task<IReadOnlyCollection<Guid>> ObtenerIdsExistentesAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Rol>> ListarTodosAsync(CancellationToken cancellationToken = default);

    Task<Rol?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Rol?> ObtenerPorNombreAsync(string nombre, CancellationToken cancellationToken = default);

    void Agregar(Rol rol);

    /// <summary>
    /// Registra explícitamente Permiso nuevos agregados a un Rol ya existente — necesario porque
    /// Permiso.Id ya tiene un valor asignado antes de que EF Core lo vea, y EF no puede distinguir
    /// "nuevo" de "existente" solo por mutación de la colección en ese caso.
    /// </summary>
    void AgregarPermisos(IEnumerable<Permiso> permisos);

    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
