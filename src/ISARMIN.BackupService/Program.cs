using ISARMIN.BackupService;

var modo = args.Length > 0 ? args[0] : "backup";

try
{
    switch (modo)
    {
        case "backup":
            await EjecutarBackupAsync();
            break;

        case "restaurar":
            if (args.Length < 3)
            {
                Console.Error.WriteLine("Uso: ISARMIN.BackupService restaurar <archivo.dump.enc> <destino.dump>");
                return 1;
            }
            await EjecutarRestauracionAsync(args[1], args[2]);
            break;

        default:
            Console.Error.WriteLine($"Modo desconocido: '{modo}'. Usa 'backup' o 'restaurar'.");
            return 1;
    }

    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ERROR: {ex.Message}");
    return 1;
}

async Task EjecutarBackupAsync()
{
    var config = ConfiguracionBackup.DesdeVariablesDeEntorno();
    var conexion = ConexionPostgres.DesdeCadena(config.CadenaConexion);
    var carpetaTemporal = Path.Combine(Path.GetTempPath(), "isarmin-backup");

    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Generando dump de '{conexion.BaseDatos}'...");
    var rutaDump = await ServicioDump.GenerarDumpAsync(conexion, config.RutaPgDump, carpetaTemporal);

    try
    {
        Console.WriteLine("Cifrando dump...");
        var rutaCifrada = await ServicioCifrado.CifrarArchivoAsync(rutaDump, config.Passphrase);

        try
        {
            Console.WriteLine("Subiendo a Google Drive (rclone)...");
            var rclone = new ServicioRclone(config.RutaRclone, config.DestinoRclone);
            await rclone.SubirAsync(rutaCifrada, CancellationToken.None);
            Console.WriteLine("Backup subido correctamente.");

            Console.WriteLine($"Aplicando rotación (conservar {config.CantidadAConservar} más recientes)...");
            await rclone.RotarAntiguosAsync(config.CantidadAConservar, CancellationToken.None);
        }
        finally
        {
            File.Delete(rutaCifrada);
        }
    }
    finally
    {
        File.Delete(rutaDump);
    }

    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Backup completado con éxito.");
}

async Task EjecutarRestauracionAsync(string rutaArchivoCifrado, string rutaDestino)
{
    var passphrase = Environment.GetEnvironmentVariable("ISARMIN_BACKUP_ENCRYPTION_KEY")
        ?? throw new InvalidOperationException("Falta la variable de entorno 'ISARMIN_BACKUP_ENCRYPTION_KEY'.");

    Console.WriteLine("Descifrando backup...");
    await ServicioCifrado.DescifrarArchivoAsync(rutaArchivoCifrado, passphrase, rutaDestino);
    Console.WriteLine($"Archivo descifrado en: {rutaDestino}");
    Console.WriteLine("Restaura con: pg_restore -h localhost -U postgres -d isarmin_erp -c \"" + rutaDestino + "\"");
}
