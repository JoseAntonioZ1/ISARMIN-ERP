using System.Diagnostics;

namespace ISARMIN.BackupService;

internal static class ServicioDump
{
    public static async Task<string> GenerarDumpAsync(ConexionPostgres conexion, string rutaPgDump, string carpetaTemporal)
    {
        Directory.CreateDirectory(carpetaTemporal);
        var rutaSalida = Path.Combine(carpetaTemporal, $"isarmin_{DateTime.Now:yyyyMMdd_HHmmss}.dump");

        var psi = new ProcessStartInfo
        {
            FileName = rutaPgDump,
            ArgumentList =
            {
                "-h", conexion.Host,
                "-p", conexion.Puerto,
                "-U", conexion.Usuario,
                "-F", "c", // formato custom: comprimido y restaurable selectivamente con pg_restore
                "-f", rutaSalida,
                conexion.BaseDatos,
            },
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
        };
        psi.Environment["PGPASSWORD"] = conexion.Password;

        using var proceso = Process.Start(psi) ?? throw new InvalidOperationException("No se pudo iniciar pg_dump.");
        var error = await proceso.StandardError.ReadToEndAsync();
        await proceso.WaitForExitAsync();

        if (proceso.ExitCode != 0)
        {
            throw new InvalidOperationException($"pg_dump terminó con código {proceso.ExitCode}: {error}");
        }

        return rutaSalida;
    }
}
