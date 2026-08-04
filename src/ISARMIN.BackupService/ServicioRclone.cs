using System.Diagnostics;
using System.Text.Json;

namespace ISARMIN.BackupService;

// Sube y rota backups en Google Drive usando rclone (ya autenticado con `rclone config`).
// Se evita la API de Google directamente porque las cuentas de servicio no tienen
// cuota de almacenamiento propia en Drive personal (solo en Shared Drives de Workspace).
internal sealed class ServicioRclone(string rutaRclone, string destino)
{
    public async Task SubirAsync(string rutaArchivo, CancellationToken ct)
    {
        await EjecutarAsync(["copy", rutaArchivo, destino], ct);
    }

    // Conserva únicamente los `cantidadAConservar` backups más recientes del destino y borra el resto.
    public async Task RotarAntiguosAsync(int cantidadAConservar, CancellationToken ct)
    {
        var salidaJson = await EjecutarAsync(["lsjson", destino], ct);
        var archivos = JsonSerializer.Deserialize<List<ArchivoRclone>>(salidaJson, JsonOpciones) ?? [];

        var aBorrar = archivos
            .OrderByDescending(a => a.ModTime)
            .Skip(cantidadAConservar);

        foreach (var archivo in aBorrar)
        {
            await EjecutarAsync(["deletefile", $"{destino}/{archivo.Name}"], ct);
        }
    }

    private async Task<string> EjecutarAsync(string[] argumentos, CancellationToken ct)
    {
        var psi = new ProcessStartInfo
        {
            FileName = rutaRclone,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
        };
        foreach (var arg in argumentos) psi.ArgumentList.Add(arg);

        using var proceso = Process.Start(psi) ?? throw new InvalidOperationException("No se pudo iniciar rclone.");
        var salida = await proceso.StandardOutput.ReadToEndAsync(ct);
        var error = await proceso.StandardError.ReadToEndAsync(ct);
        await proceso.WaitForExitAsync(ct);

        if (proceso.ExitCode != 0)
        {
            throw new InvalidOperationException($"rclone {string.Join(' ', argumentos)} falló: {error}");
        }

        return salida;
    }

    private static readonly JsonSerializerOptions JsonOpciones = new() { PropertyNameCaseInsensitive = true };

    private sealed record ArchivoRclone(string Name, DateTimeOffset ModTime);
}
