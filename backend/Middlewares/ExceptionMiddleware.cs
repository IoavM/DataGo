using System.Net;
using System.Text.Json;

namespace backend.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción no controlada capturada por ExceptionMiddleware: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, title, message) = exception switch
            {
                ArgumentException argEx => (HttpStatusCode.BadRequest, "Error de Validación", argEx.Message),
                InvalidOperationException invEx => (HttpStatusCode.BadRequest, "Regla de Negocio Incumplida", invEx.Message),
                KeyNotFoundException keyEx => (HttpStatusCode.NotFound, "Recurso No Encontrado", keyEx.Message),
                _ => (HttpStatusCode.InternalServerError, "Error Interno del Servidor", "Ocurrió un error inesperado al procesar la solicitud.")
            };

            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                status = (int)statusCode,
                title,
                detalle = message,
                mensaje = message, // retrocompatibilidad
                timestamp = DateTime.UtcNow,
                traceId = context.TraceIdentifier
            };

            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            return context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
        }
    }
}
