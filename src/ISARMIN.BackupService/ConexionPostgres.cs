namespace ISARMIN.BackupService;

internal sealed record ConexionPostgres(string Host, string Puerto, string BaseDatos, string Usuario, string Password)
{
    public static ConexionPostgres DesdeCadena(string cadenaConexion)
    {
        var valores = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var parte in cadenaConexion.Split(';', StringSplitOptions.RemoveEmptyEntries))
        {
            var separador = parte.IndexOf('=');
            if (separador <= 0) continue;
            valores[parte[..separador].Trim()] = parte[(separador + 1)..].Trim();
        }

        return new ConexionPostgres(
            valores.GetValueOrDefault("Host", "localhost"),
            valores.GetValueOrDefault("Port", "5432"),
            valores["Database"],
            valores["Username"],
            valores["Password"]);
    }
}
