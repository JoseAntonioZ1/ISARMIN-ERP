using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Proveedores.DTOs;

namespace ISARMIN.Application.Modulos.Proveedores.Queries.BuscarProveedores;

public class BuscarProveedoresQueryHandler : IQueryHandler<BuscarProveedoresQuery, ListadoPaginadoDto<ProveedorDto>>
{
    private readonly IProveedorRepository _proveedorRepository;

    public BuscarProveedoresQueryHandler(IProveedorRepository proveedorRepository)
    {
        _proveedorRepository = proveedorRepository;
    }

    public async Task<ListadoPaginadoDto<ProveedorDto>> ManejarAsync(BuscarProveedoresQuery consulta, CancellationToken cancellationToken = default)
    {
        var (proveedores, total) = await _proveedorRepository.BuscarAsync(consulta.Termino, consulta.Pagina, consulta.TamanoPagina, cancellationToken);

        return new ListadoPaginadoDto<ProveedorDto>(proveedores.Select(p => p.ADto()).ToList(), total, consulta.Pagina, consulta.TamanoPagina);
    }
}
