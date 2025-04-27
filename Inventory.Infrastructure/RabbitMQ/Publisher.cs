using MassTransit;
using Inventory.Application.Interfaces;
using Serilog;

namespace Inventory.Infrastructure.Messaging
{
    public class RabbitMQPublisher : IRabbitMQPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger _logger;

        public RabbitMQPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
            _logger = Log.ForContext<RabbitMQPublisher>();  // Usar Serilog directamente
        }

        public async Task PublishAsync<T>(T message) where T : class
        {
            try
            {
                // Publicar el mensaje con MassTransit
                await _publishEndpoint.Publish(message);
                _logger.Information("Mensaje de tipo {MessageType} publicado correctamente.", typeof(T).Name);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error al publicar el mensaje.");
                throw;
            }
        }
    }
}
