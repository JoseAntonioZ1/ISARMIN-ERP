using System.Security.Cryptography;
using System.Text;

namespace ISARMIN.Application.Common;

/// <summary>
/// Hash de valores ya aleatorios de alta entropía (ej. refresh tokens), donde no aplica
/// el costo/sal de un hasher de contraseñas: solo se necesita no guardar el valor en claro.
/// </summary>
public static class HashSha256
{
    public static string Calcular(string valor) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(valor)));
}
