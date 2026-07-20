namespace ISARMIN.Application.Modulos.Configuracion.Commands.ActualizarConfiguracionEmpresa;

public record ActualizarConfiguracionEmpresaCommand(
    string RazonSocial,
    string? Ruc,
    string? Direccion,
    string? Logo,
    decimal? MontoAperturaCajaPredeterminado,
    string? ColorAcento,
    string? MensajeBienvenida);
