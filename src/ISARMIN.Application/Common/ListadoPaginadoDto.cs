namespace ISARMIN.Application.Common;

public record ListadoPaginadoDto<T>(IReadOnlyCollection<T> Datos, int Total, int Pagina, int TamanoPagina);
