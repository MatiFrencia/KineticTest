using Inventory.Domain.Exceptions.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Inventory.Domain.Middlewares
{
    public class ExceptionLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionLoggingMiddleware> _logger;

        public ExceptionLoggingMiddleware(RequestDelegate next, ILogger<ExceptionLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
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
                throw; // rethrow to preserve behavior
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

                _logger.LogError(ex, "Custom Exception Caught | Entity: {EntityName} | Value: {Value} | Inner: {InnerExceptionMessage}",
                    entityName, value, innerMessage);
            }
            else
            {
                _logger.LogError(ex, "Unhandled Exception Caught");
            }
        }
    }

}
