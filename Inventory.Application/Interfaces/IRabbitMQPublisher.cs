namespace Inventory.Application.Interfaces
{
    public interface IRabbitMQPublisher
    {
        public Task PublishEvent<TEvent>(TEvent eventMessage) where TEvent : class;
    }
}