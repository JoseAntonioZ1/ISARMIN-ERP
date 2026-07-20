using System.Text.RegularExpressions;
using ISARMIN.Domain.Common;

namespace ISARMIN.Domain.Entities.Configuracion;

/// <summary>UC-37/RF-080 — datos generales de la empresa. Fila única sembrada en la migración
/// (patrón singleton, igual que el Administrador semilla): no existe un comando de creación, solo
/// <see cref="Actualizar"/>. RN-026: <see cref="MontoAperturaCajaPredeterminado"/> resuelve el
/// mecanismo de "monto de apertura fijo y configurable" que Caja dejó pendiente para este módulo —
/// solo prellena el formulario de apertura, no lo bloquea (el monto exacto recomendado por RN-026
/// no está confirmado por el propietario, BQ-030/BQ-088). <see cref="ColorAcento"/> y
/// <see cref="MensajeBienvenida"/> son personalización visual pura, sin regla de negocio detrás —
/// pedido directo del propietario, no derivado de un RF.</summary>
public class ConfiguracionEmpresa : Entity
{
    private static readonly Regex FormatoColorHex = new(@"^#[0-9A-Fa-f]{6}$", RegexOptions.Compiled);

    public string RazonSocial { get; private set; } = null!;
    public string? Ruc { get; private set; }
    public string? Direccion { get; private set; }
    public string? Logo { get; private set; }
    public decimal? MontoAperturaCajaPredeterminado { get; private set; }
    public string? ColorAcento { get; private set; }
    public string? MensajeBienvenida { get; private set; }

    private ConfiguracionEmpresa() { }

    public ConfiguracionEmpresa(string razonSocial)
    {
        if (string.IsNullOrWhiteSpace(razonSocial))
        {
            throw new ArgumentException("La razón social es obligatoria.", nameof(razonSocial));
        }

        RazonSocial = razonSocial;
    }

    public void Actualizar(
        string razonSocial,
        string? ruc,
        string? direccion,
        string? logo,
        decimal? montoAperturaCajaPredeterminado,
        string? colorAcento,
        string? mensajeBienvenida)
    {
        if (string.IsNullOrWhiteSpace(razonSocial))
        {
            throw new ArgumentException("La razón social es obligatoria.", nameof(razonSocial));
        }

        if (montoAperturaCajaPredeterminado is { } monto && monto < 0)
        {
            throw new ArgumentException("El monto de apertura predeterminado no puede ser negativo.", nameof(montoAperturaCajaPredeterminado));
        }

        if (colorAcento is not null && !FormatoColorHex.IsMatch(colorAcento))
        {
            throw new ArgumentException("El color de acento debe tener el formato hexadecimal '#RRGGBB'.", nameof(colorAcento));
        }

        RazonSocial = razonSocial;
        Ruc = ruc;
        Direccion = direccion;
        Logo = logo;
        MontoAperturaCajaPredeterminado = montoAperturaCajaPredeterminado;
        ColorAcento = colorAcento;
        MensajeBienvenida = mensajeBienvenida;
    }
}
