using inventory_exchange;
using MassTransit;
using Serilog;

namespace Notification.Service.RabbitMQ.Handlers.Products
{
    public class ProductCreatedEventHandler :
        IConsumer<ProductCreatedEvent>
    {
        private readonly Serilog.ILogger _logger;

        public ProductCreatedEventHandler()
        {
            _logger = Log.ForContext<ProductCreatedEventHandler>();

        }
        public async Task Consume(ConsumeContext<ProductCreatedEvent> context)
        {
            var message = context.Message;

            _logger.Information("Handling event: {EventType} | EventId: {EventId} | ProductId: {ProductId} | TraceId: {TraceId}",
            message.EventType, message.EventId, message.ProductId, message.TraceId);
            //Send email Notification
            throw new Exception("");
            await Task.CompletedTask;
        }
    }
}

