//using Microsoft.EntityFrameworkCore.Metadata;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using Polly;
//using RabbitMQ.Client;
//using RabbitMQ.Client.Events;
//using System.Text;
//using System.Text.Json;

//namespace Inventory.Infrastructure.RabbitMQ
//{
//    public class RabbitMQConsumer : BackgroundService
//    {
//        private readonly ILogger<RabbitMQConsumer> _logger;
//        private readonly IServiceScopeFactory _serviceScopeFactory;
//        private IConnection _connection;
//        private IModel _channel;
//        private readonly string _exchangeName = "inventory_exchange";

//        public RabbitMQConsumer(ILogger<RabbitMQConsumer> logger, IServiceScopeFactory serviceScopeFactory)
//        {
//            _logger = logger;
//            _serviceScopeFactory = serviceScopeFactory;
//            InitializeRabbitMQ();
//        }

//        private void InitializeRabbitMQ()
//        {
//            var factory = new ConnectionFactory()
//            {
//                HostName = "rabbitmq",
//                UserName = "guest",
//                Password = "guest"
//            };

//            _connection = factory.CreateConnection();
//            _channel = _connection.CreateModel();

//            _channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Direct, durable: true);

//            // Declarar las 3 colas
//            _channel.QueueDeclare(queue: "product_created", durable: true, exclusive: false, autoDelete: false, arguments: null);
//            _channel.QueueDeclare(queue: "product_updated", durable: true, exclusive: false, autoDelete: false, arguments: null);
//            _channel.QueueDeclare(queue: "product_deleted", durable: true, exclusive: false, autoDelete: false, arguments: null);

//            _channel.QueueBind("product_created", _exchangeName, "created");
//            _channel.QueueBind("product_updated", _exchangeName, "updated");
//            _channel.QueueBind("product_deleted", _exchangeName, "deleted");

//            _logger.LogInformation("[RabbitMQConsumer] RabbitMQ connection established.");
//        }

//        protected override Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            var consumer = new EventingBasicConsumer(_channel);

//            consumer.Received += async (sender, eventArgs) =>
//            {
//                var body = eventArgs.Body.ToArray();
//                var message = Encoding.UTF8.GetString(body);
//                var routingKey = eventArgs.RoutingKey;

//                var retryPolicy = Policy
//                    .Handle<Exception>()
//                    .WaitAndRetryAsync(
//                        retryCount: 2,
//                        sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
//                        onRetry: (ex, delay, retryCount, ctx) =>
//                        {
//                            _logger.LogWarning(ex, "[RabbitMQConsumer] Retry {RetryAttempt} after {Delay}s for message: {Message}", retryCount, delay.TotalSeconds, message);
//                        });

//                bool success = false;

//                try
//                {
//                    await retryPolicy.ExecuteAsync(async () =>
//                    {
//                        await HandleMessageAsync(routingKey, message);
//                        success = true;
//                    });
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "[RabbitMQConsumer] Failed to process message after retries: {Message}", message);
//                }

//                if (success)
//                {
//                    _channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
//                }
//                else
//                {
//                    _channel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: false);
//                }
//            };

//            _channel.BasicConsume(queue: "product_created", autoAck: false, consumer: consumer);
//            _channel.BasicConsume(queue: "product_updated", autoAck: false, consumer: consumer);
//            _channel.BasicConsume(queue: "product_deleted", autoAck: false, consumer: consumer);

//            return Task.CompletedTask;
//        }

//        private async Task HandleMessageAsync(string eventType, string payload)
//        {
//            using var scope = _serviceScopeFactory.CreateScope();
//            var dbContext = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();

//            dbContext.InventoryEventLogs.Add(new Models.InventoryEventLog
//            {
//                EventType = eventType,
//                Payload = payload
//            });

//            await dbContext.SaveChangesAsync();
//            _logger.LogInformation("[RabbitMQConsumer] Processed {EventType} event.", eventType);
//        }

//        public override void Dispose()
//        {
//            _channel?.Dispose();
//            _connection?.Dispose();
//            base.Dispose();
//        }
//    }
//}
