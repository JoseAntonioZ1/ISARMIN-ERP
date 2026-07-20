using ISARMIN.Domain.Common;

namespace ISARMIN.Domain.Entities.Configuracion;

/// <summary>UC-37/RF-080 — datos generales de la empresa. Fila única sembrada en la migración
/// (patrón singleton, igual que el Administrador semilla): no existe un comando de creación, solo
/// <see cref="Actualizar"/>. RN-026: <see cref="MontoAperturaCajaPredeterminado"/> resuelve el
/// mecanismo de "monto de apertura fijo y configurable" que Caja dejó pendiente para este módulo —
/// solo prellena el formulario de apertura, no lo bloquea (el monto exacto recomendado por RN-026
/// no está confirmado por el propietario, BQ-030/BQ-088).</summary>
public class ConfiguracionEmpresa : Entity
{
    public string RazonSocial { get; private set; } = null!;
    public string? Ruc { get; private set; }
    public string? Direccion { get; private set; }
    public string? Logo { get; private set; }
    public decimal? MontoAperturaCajaPredeterminado { get; private set; }

    private ConfiguracionEmpresa() { }

    public ConfiguracionEmpresa(string razonSocial)
    {
        if (string.IsNullOrWhiteSpace(razonSocial))
        {
            throw new ArgumentException("La razón social es obligatoria.", nameof(razonSocial));
        }

        RazonSocial = razonSocial;
    }

    public void Actualizar(string razonSocial, string? ruc, string? direccion, string? logo, decimal? montoAperturaCajaPredeterminado)
    {
        if (string.IsNullOrWhiteSpace(razonSocial))
        {
            throw new ArgumentException("La razón social es obligatoria.", nameof(razonSocial));
        }

        if (montoAperturaCajaPredeterminado is { } monto && monto < 0)
        {
            throw new ArgumentException("El monto de apertura predeterminado no puede ser negativo.", nameof(montoAperturaCajaPredeterminado));
        }

        RazonSocial = razonSocial;
        Ruc = ruc;
        Direccion = direccion;
        Logo = logo;
        MontoAperturaCajaPredeterminado = montoAperturaCajaPredeterminado;
    }
}
