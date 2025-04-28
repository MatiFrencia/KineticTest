using Inventory.Domain.Exceptions.Repository;
using Microsoft.AspNetCore.Http;
using Serilog;  // Asegúrate de agregar el espacio de nombres de Serilog

namespace Inventory.Domain.Middlewares
{
    public class ExceptionLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                LogException(ex);
                throw; // Re-throws the exception to preserve the original behavior
            }
        }

        private void LogException(Exception ex)
        {
            if (ex is FailedToAdd_Exception or
                FailedToUpdate_Exception or
                FailedToDelete_Exception or
                NotFound_Exception or
                LostDatabaseConnection_Exception)
            {
                string entityName = (string)ex.GetType().GetProperty("EntityName")?.GetValue(ex) ?? "unknown_entity";
                string value = (string)ex.GetType().GetProperty("Value")?.GetValue(ex) ?? "unknown_value";
                string innerMessage = ex.InnerException?.Message ?? "none";

                // Usando Serilog para loguear la excepción
                Log.Error(ex, "Custom Exception Caught | Entity: {EntityName} | Value: {Value} | Inner: {InnerExceptionMessage}",
                    entityName, value, innerMessage);
            }
            else
            {
                // Usando Serilog para loguear excepciones no manejadas
                Log.Error(ex, "Unhandled Exception Caught");
            }
        }
    }
}
