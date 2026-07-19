using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Compras.DTOs;

namespace ISARMIN.Application.Modulos.Compras.Queries.ObtenerCompra;

public class ObtenerCompraQueryHandler : IQueryHandler<ObtenerCompraQuery, CompraDto>
{
    private readonly ICompraRepository _compraRepository;

    public ObtenerCompraQueryHandler(ICompraRepository compraRepository)
    {
        _compraRepository = compraRepository;
    }

    public async Task<CompraDto> ManejarAsync(ObtenerCompraQuery consulta, CancellationToken cancellationToken = default)
    {
        var compra = await _compraRepository.ObtenerPorIdAsync(consulta.Id, cancellationToken)
            ?? throw new CompraNoEncontradaException(consulta.Id);

        return compra.ADto();
    }
}
