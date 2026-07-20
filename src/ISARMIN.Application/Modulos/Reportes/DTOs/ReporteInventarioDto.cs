using ISARMIN.Application.Modulos.Inventario.DTOs;

namespace ISARMIN.Application.Modulos.Reportes.DTOs;

/// <summary>RF-073 — quiebre: producto activo con stock mínimo definido cuyo stock actual llegó a ese mínimo o menos.</summary>
public record ReporteInventarioDto(
    IReadOnlyCollection<ProductoDto> Productos,
    IReadOnlyCollection<ProductoDto> ProductosEnQuiebre);
