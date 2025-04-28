using inventory_exchange;
using MassTransit;
using Serilog;
using Newtonsoft.Json;
using Notification.Service.Data;
using Notification.Service.Models;

namespace Notification.Service.RabbitMQ.Handlers.Products
{
    public class ProductDeletedEventHandler :
        IConsumer<ProductDeletedEvent>
    {
        private readonly Serilog.ILogger _logger;

        public ProductDeletedEventHandler()
        {
            _logger = Log.ForContext<ProductDeletedEventHandler>();

        }
        public async Task Consume(ConsumeContext<ProductDeletedEvent> context)
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
