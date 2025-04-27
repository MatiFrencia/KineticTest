namespace Inventory.Application.Interfaces
{
    public interface IRabbitMQPublisher
    {
        public Task PublishAsync<T>(T message) where T : class;
    }
}