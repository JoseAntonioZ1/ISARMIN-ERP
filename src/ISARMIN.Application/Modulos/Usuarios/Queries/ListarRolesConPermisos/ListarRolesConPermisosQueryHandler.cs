using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Usuarios.DTOs;

namespace ISARMIN.Application.Modulos.Usuarios.Queries.ListarRolesConPermisos;

public class ListarRolesConPermisosQueryHandler : IQueryHandler<ListarRolesConPermisosQuery, IReadOnlyCollection<RolDto>>
{
    private readonly IRolRepository _rolRepository;

    public ListarRolesConPermisosQueryHandler(IRolRepository rolRepository)
    {
        _rolRepository = rolRepository;
    }

    public async Task<IReadOnlyCollection<RolDto>> ManejarAsync(ListarRolesConPermisosQuery consulta, CancellationToken cancellationToken = default)
    {
        var roles = await _rolRepository.ListarTodosAsync(cancellationToken);
        return roles.Select(r => r.ADto()).ToList();
    }
}
