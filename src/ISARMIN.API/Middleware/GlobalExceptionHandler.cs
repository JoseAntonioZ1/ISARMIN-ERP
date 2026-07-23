using FluentValidation;
using ISARMIN.Application.Common.Excepciones;
using Microsoft.AspNetCore.Diagnostics;

namespace ISARMIN.API.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is ValidationException validationException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsJsonAsync(new
            {
                error = new
                {
                    codigo = "VALIDACION_FALLIDA",
                    mensaje = "La solicitud contiene datos inválidos.",
                    detalles = validationException.Errors.Select(e => new { campo = e.PropertyName, error = e.ErrorMessage })
                }
            }, cancellationToken);
            return true;
        }

        if (exception is ExcepcionAplicacion excepcionAplicacion)
        {
            httpContext.Response.StatusCode = excepcionAplicacion.CodigoHttp;
            await httpContext.Response.WriteAsJsonAsync(new
            {
                error = new
                {
                    codigo = excepcionAplicacion.Codigo,
                    mensaje = excepcionAplicacion.Message,
                    detalles = (object?)null
                }
            }, cancellationToken);
            return true;
        }

        // Las entidades de Domain lanzan ArgumentException para sus invariantes propias (ej. RN-009:
        // Factura exige RUC) que no siempre pasan primero por un validador de FluentValidation — sin
        // este caso, cualquier violación de una regla de Domain caía como 500 genérico en vez de un
        // 400 con el mensaje claro que la propia entidad ya construye.
        if (exception is ArgumentException argumentException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsJsonAsync(new
            {
                error = new
                {
                    codigo = "SOLICITUD_INVALIDA",
                    mensaje = argumentException.Message,
                    detalles = (object?)null
                }
            }, cancellationToken);
            return true;
        }

        _logger.LogError(exception, "Excepción no controlada: {Mensaje}", exception.Message);

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(new
        {
            error = new
            {
                codigo = "ERROR_INTERNO",
                mensaje = "Ocurrió un error inesperado al procesar la solicitud.",
                detalles = (object?)null
            }
        }, cancellationToken);

        return true;
    }
}
