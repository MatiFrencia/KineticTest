using MassTransit;
using Inventory.Application.Interfaces;
using Serilog;
using Microsoft.AspNetCore.Http;
using inventory_exchange;

namespace Inventory.Infrastructure.RabbitMQ
{
    public class RabbitMQPublisher : IRabbitMQPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RabbitMQPublisher(IPublishEndpoint publishEndpoint, IHttpContextAccessor httpContextAccessor)
        {
            _publishEndpoint = publishEndpoint;
            _httpContextAccessor = httpContextAccessor;
            _logger = Log.ForContext<RabbitMQPublisher>();
        }

        private async Task PublishAsync<T>(T message, string routingKey) where T : class
        {
            try
            {
                string traceId = typeof(T).GetProperty("TraceId")?.GetValue(message)?.ToString() ?? "N/A";
                string eventId = typeof(T).GetProperty("EventId")?.GetValue(message)?.ToString() ?? "N/A";

                // Publica el mensaje con el routingKey configurado
                await _publishEndpoint.Publish(message);

                _logger.Information("Mensaje de tipo {MessageType} publicado correctamente. TraceId: {TraceId} | EventId: {EventId} | RoutingKey: {RoutingKey}",
                    typeof(T).Name, traceId, eventId, routingKey);
            }
            catch (Exception ex)
            {
                string traceId = typeof(T).GetProperty("TraceId")?.GetValue(message)?.ToString() ?? "N/A";
                string eventId = typeof(T).GetProperty("EventId")?.GetValue(message)?.ToString() ?? "N/A";

                _logger.Error(ex, "Error al publicar el mensaje. TraceId: {TraceId} | EventId: {EventId} | RoutingKey: {RoutingKey}",
                    traceId, eventId, routingKey);
                throw;
            }
        }

        private string GetRoutingKeyForEvent<T>()
        {
            var typeName = typeof(T).Name;

            return typeName switch
            {
                nameof(ProductCreatedEvent) => "product_created_queue",
                nameof(ProductUpdatedEvent) => "product.updated",
                nameof(ProductDeletedEvent) => "product.deleted",
                _ => "default.routing"  // Opcional: para no romper si es otro evento
            };
        }

        private string GetTraceId()
        {
            return _httpContextAccessor.HttpContext?.Items["TraceId"]?.ToString() ?? Guid.NewGuid().ToString();
        }

        private string GenerateEventId()
        {
            return Guid.NewGuid().ToString();
        }

        public async Task PublishEvent<TEvent>(TEvent eventMessage) where TEvent : class
        {
            var traceId = GetTraceId();
            var eventId = GenerateEventId();
            var routingKey = GetRoutingKeyForEvent<TEvent>();

            Log.Information("Publishing event: {EventType} | EventId: {EventId} | TraceId: {TraceId} | RoutingKey: {RoutingKey}",
                typeof(TEvent).Name, eventId, traceId, routingKey);

            if (eventMessage is IProductEvent eventBase)
            {
                eventBase.EventId = eventId;
                eventBase.TraceId = traceId;
            }

            _ = PublishAsync(eventMessage, routingKey);
        }
    }
}
