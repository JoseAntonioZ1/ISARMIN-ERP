using ISARMIN.Application.Common;

namespace ISARMIN.Infrastructure.Auth;

public class FechaHoraProvider : IFechaHoraProvider
{
    public DateTime UtcAhora => DateTime.UtcNow;
}
