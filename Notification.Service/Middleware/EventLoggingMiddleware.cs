using MassTransit;
using Notification.Service.Data;
using Notification.Service.Models;
using Newtonsoft.Json;
using Serilog;

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
                // Guardar el evento en la base de datos
                await _dbContext.EventLogs.AddAsync(eventLog);
                await _dbContext.SaveChangesAsync();

                _logger.Information("Event {EventId} for {EventType} saved to database.", eventLog.EventId, eventLog.EventType);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error while saving event {EventId} for {EventType} to database.", eventLog.EventId, eventLog.EventType);
            }

            // Llamar al siguiente middleware o handler
            await next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("eventLogging");
        }
    }
}
