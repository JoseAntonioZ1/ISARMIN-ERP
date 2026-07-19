using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Usuarios.DTOs;

namespace ISARMIN.Application.Modulos.Usuarios.Queries.ListarRoles;

public class ListarRolesQueryHandler : IQueryHandler<ListarRolesQuery, IReadOnlyCollection<RolResumenDto>>
{
    private readonly IRolRepository _rolRepository;

    public ListarRolesQueryHandler(IRolRepository rolRepository)
    {
        _rolRepository = rolRepository;
    }

    public async Task<IReadOnlyCollection<RolResumenDto>> ManejarAsync(ListarRolesQuery consulta, CancellationToken cancellationToken = default)
    {
        var roles = await _rolRepository.ListarTodosAsync(cancellationToken);
        return roles.Select(r => new RolResumenDto(r.Id, r.Nombre)).ToList();
    }
}
