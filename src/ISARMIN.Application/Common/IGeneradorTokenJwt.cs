using ISARMIN.Domain.Entities.Identidad;

namespace ISARMIN.Application.Common;

public record TokenGenerado(string Token, DateTime ExpiraEnUtc);

public interface IGeneradorTokenJwt
{
    TokenGenerado Generar(Usuario usuario, IReadOnlyCollection<string> permisos);
}
