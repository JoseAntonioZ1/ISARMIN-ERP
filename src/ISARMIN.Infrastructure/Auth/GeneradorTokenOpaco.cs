using System.Security.Cryptography;
using ISARMIN.Application.Common;

namespace ISARMIN.Infrastructure.Auth;

public class GeneradorTokenOpaco : IGeneradorTokenOpaco
{
    public string Generar() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
        .Replace('+', '-')
        .Replace('/', '_')
        .TrimEnd('=');
}
