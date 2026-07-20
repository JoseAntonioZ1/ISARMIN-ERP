namespace ISARMIN.Application.Modulos.Configuracion.DTOs;

/// <summary>Subconjunto público de <see cref="ConfiguracionEmpresaDto"/> — identidad visual y textos
/// pensados para verse sin (o independientemente de) sesión iniciada: login, encabezado, pestaña del
/// navegador, pantalla de inicio, encabezado de reportes. Ninguno de estos campos es sensible (el RUC
/// ya se imprime en cualquier comprobante); quedan fuera Direccion y MontoAperturaCajaPredeterminado,
/// que sí siguen detrás de `Configuracion.Consultar` por ser datos más operativos.</summary>
public record BrandingDto(string RazonSocial, string? Ruc, string? Logo, string? ColorAcento, string? MensajeBienvenida);
