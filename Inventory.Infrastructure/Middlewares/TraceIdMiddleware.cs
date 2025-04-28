using Serilog;
using Microsoft.AspNetCore.Http;

namespace Inventory.Infrastructure.Middlewares
{
    public class TraceIdMiddleware
    {
        private readonly RequestDelegate _next;

        public TraceIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Generar un TraceId único para cada solicitud
            var traceId = Guid.NewGuid().ToString();
            context.Items["TraceId"] = traceId; // Guardar TraceId en el contexto

            // Loguear el TraceId en Serilog para trazabilidad
            Log.Information("Request TraceId: {TraceId}", traceId);

            // Continuar con la siguiente etapa del pipeline
            await _next(context);
        }
    }
}
