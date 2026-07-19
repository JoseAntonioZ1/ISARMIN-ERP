namespace ISARMIN.Application.Common;

public interface ICommandHandler<in TCommand, TResultado>
{
    Task<TResultado> ManejarAsync(TCommand comando, CancellationToken cancellationToken = default);
}
