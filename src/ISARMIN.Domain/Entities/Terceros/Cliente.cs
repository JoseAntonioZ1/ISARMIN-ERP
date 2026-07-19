using ISARMIN.Domain.Common;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Domain.Entities.Terceros;

/// <summary>UC-05 — Gestionar Cliente.</summary>
public class Cliente : Entity
{
    public string NombreRazonSocial { get; private set; } = null!;

    /// <summary>RN-039 — único dato de contacto obligatorio.</summary>
    public string Telefono { get; private set; } = null!;

    public string? Direccion { get; private set; }

    public TipoDocumento? TipoDocumento { get; private set; }

    public string? NumeroDocumento { get; private set; }

    /// <summary>RN-034 — derivado automáticamente de TipoDocumento, nunca capturado manualmente.</summary>
    public TipoCliente? TipoCliente { get; private set; }

    public EstadoRegistro Estado { get; private set; }

    private Cliente() { }

    public Cliente(
        string nombreRazonSocial,
        string telefono,
        string? direccion,
        Enums.TipoDocumento? tipoDocumento,
        string? numeroDocumento)
    {
        ValidarNombre(nombreRazonSocial);
        ValidarTelefono(telefono);
        ValidarDocumento(tipoDocumento, numeroDocumento);

        NombreRazonSocial = nombreRazonSocial;
        Telefono = telefono;
        Direccion = direccion;
        AsignarDocumento(tipoDocumento, numeroDocumento);
        Estado = EstadoRegistro.Activo;
    }

    public bool EstaActivo => Estado == EstadoRegistro.Activo;

    public void Desactivar() => Estado = EstadoRegistro.Inactivo;

    public void Activar() => Estado = EstadoRegistro.Activo;

    public void ActualizarDatos(
        string nombreRazonSocial,
        string telefono,
        string? direccion,
        Enums.TipoDocumento? tipoDocumento,
        string? numeroDocumento)
    {
        ValidarNombre(nombreRazonSocial);
        ValidarTelefono(telefono);
        ValidarDocumento(tipoDocumento, numeroDocumento);

        NombreRazonSocial = nombreRazonSocial;
        Telefono = telefono;
        Direccion = direccion;
        AsignarDocumento(tipoDocumento, numeroDocumento);
    }

    private void AsignarDocumento(Enums.TipoDocumento? tipoDocumento, string? numeroDocumento)
    {
        TipoDocumento = tipoDocumento;
        NumeroDocumento = numeroDocumento;
        TipoCliente = tipoDocumento switch
        {
            Enums.TipoDocumento.Ruc => Enums.TipoCliente.Juridica,
            not null => Enums.TipoCliente.Natural,
            null => null,
        };
    }

    private static void ValidarNombre(string nombreRazonSocial)
    {
        if (string.IsNullOrWhiteSpace(nombreRazonSocial))
        {
            throw new ArgumentException("El nombre o razón social es obligatorio.", nameof(nombreRazonSocial));
        }
    }

    private static void ValidarTelefono(string telefono)
    {
        if (string.IsNullOrWhiteSpace(telefono))
        {
            throw new ArgumentException("El teléfono es obligatorio (RN-039).", nameof(telefono));
        }
    }

    private static void ValidarDocumento(Enums.TipoDocumento? tipoDocumento, string? numeroDocumento)
    {
        if (tipoDocumento is null && numeroDocumento is null)
        {
            return;
        }

        if (tipoDocumento is null || string.IsNullOrWhiteSpace(numeroDocumento))
        {
            throw new ArgumentException("El tipo y el número de documento deben proporcionarse juntos.");
        }

        if (!ValidadorDocumentoIdentidad.EsValido(tipoDocumento.Value, numeroDocumento))
        {
            throw new ArgumentException($"El número de documento '{numeroDocumento}' no es válido para el tipo '{tipoDocumento}' (RN-033).");
        }
    }
}
