using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Clientes.DTOs;

namespace ISARMIN.Application.Modulos.Clientes.Queries.BuscarClientes;

public class BuscarClientesQueryHandler : IQueryHandler<BuscarClientesQuery, ListadoPaginadoDto<ClienteDto>>
{
    private readonly IClienteRepository _clienteRepository;

    public BuscarClientesQueryHandler(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<ListadoPaginadoDto<ClienteDto>> ManejarAsync(BuscarClientesQuery consulta, CancellationToken cancellationToken = default)
    {
        var (clientes, total) = await _clienteRepository.BuscarAsync(consulta.Termino, consulta.Pagina, consulta.TamanoPagina, cancellationToken);

        return new ListadoPaginadoDto<ClienteDto>(clientes.Select(c => c.ADto()).ToList(), total, consulta.Pagina, consulta.TamanoPagina);
    }
}
