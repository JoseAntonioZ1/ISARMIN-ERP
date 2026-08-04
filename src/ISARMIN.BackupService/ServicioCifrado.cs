using System.Security.Cryptography;

namespace ISARMIN.BackupService;

// Formato del archivo cifrado: [16 bytes salt][16 bytes IV][contenido cifrado AES-256-CBC]
internal static class ServicioCifrado
{
    private const int TamanoSalt = 16;
    private const int Iteraciones = 100_000;

    public static async Task<string> CifrarArchivoAsync(string rutaOrigen, string passphrase)
    {
        var rutaDestino = rutaOrigen + ".enc";
        var salt = RandomNumberGenerator.GetBytes(TamanoSalt);

        using var derivador = new Rfc2898DeriveBytes(passphrase, salt, Iteraciones, HashAlgorithmName.SHA256);
        using var aes = Aes.Create();
        aes.Key = derivador.GetBytes(32);
        aes.GenerateIV();

        await using var salida = File.Create(rutaDestino);
        await salida.WriteAsync(salt);
        await salida.WriteAsync(aes.IV);

        await using var origen = File.OpenRead(rutaOrigen);
        await using var cripto = new CryptoStream(salida, aes.CreateEncryptor(), CryptoStreamMode.Write);
        await origen.CopyToAsync(cripto);
        await cripto.FlushFinalBlockAsync();

        return rutaDestino;
    }

    public static async Task<string> DescifrarArchivoAsync(string rutaOrigen, string passphrase, string rutaDestino)
    {
        await using var origen = File.OpenRead(rutaOrigen);

        var salt = new byte[TamanoSalt];
        await origen.ReadExactlyAsync(salt);

        using var aesTemp = Aes.Create();
        var iv = new byte[aesTemp.IV.Length];
        await origen.ReadExactlyAsync(iv);

        using var derivador = new Rfc2898DeriveBytes(passphrase, salt, Iteraciones, HashAlgorithmName.SHA256);
        using var aes = Aes.Create();
        aes.Key = derivador.GetBytes(32);
        aes.IV = iv;

        await using var salida = File.Create(rutaDestino);
        await using var cripto = new CryptoStream(origen, aes.CreateDecryptor(), CryptoStreamMode.Read);
        await cripto.CopyToAsync(salida);

        return rutaDestino;
    }
}
