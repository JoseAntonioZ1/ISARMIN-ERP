namespace ISARMIN.Application.Common;

public interface IQueryHandler<in TQuery, TResultado>
{
    Task<TResultado> ManejarAsync(TQuery consulta, CancellationToken cancellationToken = default);
}
