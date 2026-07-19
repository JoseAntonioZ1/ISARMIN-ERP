using ISARMIN.Domain.Common;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Domain.Entities.Terceros;

/// <summary>UC-09 — Gestionar Proveedor (análogo a Cliente, sin distinción Natural/Jurídica ni validación de documento).</summary>
public class Proveedor : Entity
{
    public string NombreRazonSocial { get; private set; } = null!;
    public string? Documento { get; private set; }
    public string? Telefono { get; private set; }
    public string? Direccion { get; private set; }
    public EstadoRegistro Estado { get; private set; }

    private Proveedor() { }

    public Proveedor(string nombreRazonSocial, string? documento, string? telefono, string? direccion)
    {
        ValidarNombre(nombreRazonSocial);

        NombreRazonSocial = nombreRazonSocial;
        Documento = documento;
        Telefono = telefono;
        Direccion = direccion;
        Estado = EstadoRegistro.Activo;
    }

    public bool EstaActivo => Estado == EstadoRegistro.Activo;

    public void Desactivar() => Estado = EstadoRegistro.Inactivo;

    public void Activar() => Estado = EstadoRegistro.Activo;

    public void ActualizarDatos(string nombreRazonSocial, string? documento, string? telefono, string? direccion)
    {
        ValidarNombre(nombreRazonSocial);

        NombreRazonSocial = nombreRazonSocial;
        Documento = documento;
        Telefono = telefono;
        Direccion = direccion;
    }

    private static void ValidarNombre(string nombreRazonSocial)
    {
        if (string.IsNullOrWhiteSpace(nombreRazonSocial))
        {
            throw new ArgumentException("El nombre o razón social es obligatorio.", nameof(nombreRazonSocial));
        }
    }
}
