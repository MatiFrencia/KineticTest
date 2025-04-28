using MassTransit;
using Notification.Service.Data;
using Notification.Service.Models;
using Newtonsoft.Json;
using Serilog;
using Microsoft.EntityFrameworkCore;

namespace Notification.Service.Middleware
{
    public class EventLoggingMiddleware<T> : IFilter<ConsumeContext<T>> where T : class
    {
        private readonly NotificationContext _dbContext;
        private readonly Serilog.ILogger _logger;

        public EventLoggingMiddleware(NotificationContext dbContext)
        {
            _dbContext = dbContext;
            _logger = Log.ForContext<EventLoggingMiddleware<T>>();
        }

        public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            var message = context.Message;
            int retryCount = 0;
            bool processed = false;

            // Crear el evento genérico para guardar en la base de datos
            var eventLog = new EventLogs
            {
                EventId = (string)message.GetType().GetProperty("EventId")?.GetValue(message),
                EventType = (string)message.GetType().GetProperty("EventType")?.GetValue(message),
                TraceId = (string)message.GetType().GetProperty("TraceId")?.GetValue(message),
                CreatedAt = DateTime.UtcNow.AddHours(-3),
                Payload = JsonConvert.SerializeObject(message)  // Serializamos el mensaje completo
            };

            try
            {
                // Llamar al siguiente middleware o handler si todo sale bien
                await next.Send(context);

                // Guardar el evento en la base de datos (para eventos procesados correctamente)
                await _dbContext.EventLogs.AddAsync(eventLog);
                var failedEventLog = await _dbContext.FailedEventLogs
                    .FirstOrDefaultAsync(e => e.EventId == eventLog.EventId);
                if (failedEventLog != null)
                {
                    failedEventLog.SuccessfullyHandled = true;
                }
                await _dbContext.SaveChangesAsync();
                _logger.Information("Event {EventId} for {EventType} saved to database.", eventLog.EventId, eventLog.EventType);
            }
            catch (Exception ex)
            {
                // Verificar si ya existe un evento fallido con el mismo EventId
                var failedEventLog = await _dbContext.FailedEventLogs
                    .FirstOrDefaultAsync(e => e.EventId == eventLog.EventId);
                if (failedEventLog != null)
                {
                    if (failedEventLog.SuccessfullyHandled)
                        return;
                    // Si ya existe, incrementar los reintentos
                    failedEventLog.Retries += 1;

                    // Si el número de reintentos ha alcanzado 3, no volver a intentar más
                    if (failedEventLog.Retries > 3)
                    {
                        _logger.Warning("Event {EventId} for {EventType} reached maximum retries.", failedEventLog.EventId, failedEventLog.EventType);
                        return; // No procesamos más el evento
                    }

                    // Actualizar el evento fallido con el nuevo número de reintentos y detalles del error
                    failedEventLog.ErrorDetails = ex.Message;
                    _dbContext.FailedEventLogs.Update(failedEventLog);
                }
                else
                {
                    // Si no existe, crear un nuevo evento fallido
                    var newFailedEventLog = new FailedEventLogs
                    {
                        EventId = eventLog.EventId,
                        EventType = eventLog.EventType,
                        TraceId = eventLog.TraceId,
                        CreatedAt = DateTime.UtcNow.AddHours(-3),
                        Payload = eventLog.Payload,
                        Retries = 1,  // Primer reintento
                        ErrorDetails = ex.Message // Detalle del error inicial
                    };

                    await _dbContext.FailedEventLogs.AddAsync(newFailedEventLog);
                }

                // Guardar los cambios en la base de datos
                await _dbContext.SaveChangesAsync();

                _logger.Error(ex, "Error processing event {EventId} for {EventType}, saved to FailedEventLogs.", eventLog.EventId, eventLog.EventType);
                throw ex;
            }
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("eventLogging");
        }
    }
}
