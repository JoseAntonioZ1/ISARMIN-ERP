namespace ISARMIN.Application.Modulos.Configuracion.DTOs;

/// <summary>Subconjunto público de <see cref="ConfiguracionEmpresaDto"/> — identidad visual y textos
/// pensados para verse sin (o independientemente de) sesión iniciada: login, encabezado, pestaña del
/// navegador, pantalla de inicio, encabezado de reportes, y ahora también el comprobante imprimible de
/// Ventas (que un cajero sin `Configuracion.Consultar` debe poder generar). Ninguno de estos campos es
/// sensible: RUC y Direccion ya se imprimen en cualquier comprobante entregado a un cliente. Queda fuera
/// `MontoAperturaCajaPredeterminado`, detrás de `Configuracion.Consultar`, por ser un dato operativo de Caja.</summary>
public record BrandingDto(string RazonSocial, string? Ruc, string? Direccion, string? Logo, string? ColorAcento, string? MensajeBienvenida);
