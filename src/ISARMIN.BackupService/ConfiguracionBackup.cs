namespace ISARMIN.BackupService;

internal sealed record ConfiguracionBackup(
    string CadenaConexion,
    string Passphrase,
    string RutaRclone,
    string DestinoRclone,
    string RutaPgDump,
    int CantidadAConservar)
{
    public static ConfiguracionBackup DesdeVariablesDeEntorno()
    {
        return new ConfiguracionBackup(
            Requerida("ISARMIN_BACKUP_DB_CONNECTION"),
            Requerida("ISARMIN_BACKUP_ENCRYPTION_KEY"),
            Environment.GetEnvironmentVariable("ISARMIN_BACKUP_RCLONE_PATH") ?? "rclone",
            Requerida("ISARMIN_BACKUP_RCLONE_DESTINO"),
            Environment.GetEnvironmentVariable("ISARMIN_BACKUP_PG_DUMP_PATH") ?? "pg_dump",
            int.TryParse(Environment.GetEnvironmentVariable("ISARMIN_BACKUP_CANTIDAD_A_CONSERVAR"), out var n) ? n : 30);
    }

    private static string Requerida(string nombre) =>
        Environment.GetEnvironmentVariable(nombre)
        ?? throw new InvalidOperationException(
            $"Falta la variable de entorno '{nombre}'. Revisa el README de ISARMIN.BackupService para configurarla.");
}
