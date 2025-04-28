using inventory_exchange;
using MassTransit;
using Serilog;
using Newtonsoft.Json;
using Notification.Service.Data;
using Notification.Service.Models;

namespace Notification.Service.RabbitMQ.Handlers.Products
{
    public class ProductUpdatedEventHandler :
        IConsumer<ProductUpdatedEvent>
    {
        private readonly Serilog.ILogger _logger;

        public ProductUpdatedEventHandler()
        {
            _logger = Log.ForContext<ProductUpdatedEventHandler>();

        }
        public async Task Consume(ConsumeContext<ProductUpdatedEvent> context)
        {
            var message = context.Message;

            _logger.Information("Handling event: {EventType} | EventId: {EventId} | ProductId: {ProductId} | TraceId: {TraceId}",
            message.EventType, message.EventId, message.ProductId, message.TraceId);
            //Send email Notification
            await Task.CompletedTask;
        }
    }
}
