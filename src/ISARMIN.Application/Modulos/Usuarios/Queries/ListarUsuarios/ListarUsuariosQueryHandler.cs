using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Usuarios.DTOs;

namespace ISARMIN.Application.Modulos.Usuarios.Queries.ListarUsuarios;

public class ListarUsuariosQueryHandler : IQueryHandler<ListarUsuariosQuery, ListadoPaginadoDto<UsuarioDto>>
{
    private readonly IUsuarioRepository _usuarioRepository;

    public ListarUsuariosQueryHandler(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<ListadoPaginadoDto<UsuarioDto>> ManejarAsync(ListarUsuariosQuery consulta, CancellationToken cancellationToken = default)
    {
        var (usuarios, total) = await _usuarioRepository.ListarAsync(consulta.Pagina, consulta.TamanoPagina, cancellationToken);

        var datos = usuarios.Select(u => new UsuarioDto(
            u.Id,
            u.Nombre,
            u.NombreUsuario,
            u.Estado.ToString(),
            u.Roles.Select(ur => new RolResumenDto(ur.RolId, ur.Rol.Nombre)).ToList()))
            .ToList();

        return new ListadoPaginadoDto<UsuarioDto>(datos, total, consulta.Pagina, consulta.TamanoPagina);
    }
}
