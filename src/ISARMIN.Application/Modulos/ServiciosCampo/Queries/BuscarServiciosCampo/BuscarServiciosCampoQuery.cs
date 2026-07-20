using ISARMIN.Domain.Enums;

namespace ISARMIN.Application.Modulos.ServiciosCampo.Queries.BuscarServiciosCampo;

public record BuscarServiciosCampoQuery(EstadoServicioCampo? Estado = null, Guid? ClienteId = null, int Pagina = 1, int TamanoPagina = 20);
