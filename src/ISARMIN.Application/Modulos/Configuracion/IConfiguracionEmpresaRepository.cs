using ISARMIN.Domain.Entities.Configuracion;

namespace ISARMIN.Application.Modulos.Configuracion;

/// <summary>Fila única sembrada en la migración (patrón singleton) — no hay Agregar.</summary>
public interface IConfiguracionEmpresaRepository
{
    Task<ConfiguracionEmpresa> ObtenerAsync(CancellationToken cancellationToken = default);

    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
