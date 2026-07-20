using ISARMIN.Application.Modulos.Caja.DTOs;

namespace ISARMIN.Application.Modulos.Reportes.DTOs;

/// <summary>RF-076 — refleja únicamente los movimientos manuales registrados en Caja (RN-026); no
/// incluye cobros de Ventas/Taller/Servicios de Campo porque ese puente ("cobro → Caja") pertenece
/// a un mecanismo unificado de Cobranzas aún no construido.</summary>
public record ReporteCajaDto(
    DateTime? Desde,
    DateTime? Hasta,
    decimal TotalIngresos,
    decimal TotalEgresos,
    decimal SaldoNeto,
    IReadOnlyCollection<CajaDto> Cajas,
    IReadOnlyCollection<MovimientoCajaDto> Movimientos);
