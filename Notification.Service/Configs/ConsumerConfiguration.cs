using MassTransit;
using Notification.Service.Middleware;
using Notification.Service.RabbitMQ.Handlers.Products;
using Polly;

namespace Notification.Service.Configs
{
    public static class ConsumerConfiguration
    {
        public static void ConfigureMassTransitConsumers(this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<ProductCreatedEventHandler>();
                x.AddConsumer<ProductUpdatedEventHandler>();
                x.AddConsumer<ProductDeletedEventHandler>();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("rabbitmq", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.UseConsumeFilter(typeof(EventLoggingMiddleware<>), context);



                    cfg.ReceiveEndpoint("product_created_queue", e =>
                    {
                        e.Bind("inventory_exchange", s =>
                        {
                            s.RoutingKey = "product_created";
                            s.ExchangeType = "direct";
                        });

                        e.Consumer<ProductCreatedEventHandler>();

                        e.UseMessageRetry(r => r.Immediate(3));
                        e.UseDelayedRedelivery(r => r.Intervals(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10)));

                        e.UseCircuitBreaker(cb =>
                        {
                            cb.TrackingPeriod = TimeSpan.FromMinutes(15);
                            cb.TripThreshold = 1; // 15% de fallas
                            cb.ActiveThreshold = 1; // Mínimo 10 intentos antes de evaluar
                            cb.ResetInterval = TimeSpan.FromSeconds(20);
                        });

                        e.BindDeadLetterQueue("inventory_exchange_failed", "failed_product_created_queue", dlq =>
                        {
                            dlq.ExchangeType = "direct";
                        });
                    });

                    cfg.ReceiveEndpoint("product_updated_queue", e =>
                    {
                        e.Bind("inventory_exchange", s =>
                        {
                            s.RoutingKey = "product_updated";
                            s.ExchangeType = "direct";
                        });
                        e.Consumer<ProductUpdatedEventHandler>();

                        e.UseMessageRetry(r => r.Immediate(3));
                        e.UseDelayedRedelivery(r => r.Intervals(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10)));

                        e.UseCircuitBreaker(cb =>
                        {
                            cb.TrackingPeriod = TimeSpan.FromMinutes(15);
                            cb.TripThreshold = 1; // 15% de fallas
                            cb.ActiveThreshold = 1; // Mínimo 10 intentos antes de evaluar
                            cb.ResetInterval = TimeSpan.FromSeconds(20);
                        });
                        e.BindDeadLetterQueue("inventory_exchange_failed", "failed_product_updated_queue", dlq =>
                        {
                            dlq.ExchangeType = "direct";
                        });
                    });

                    cfg.ReceiveEndpoint("product_deleted_queue", e =>
                    {
                        e.Bind("inventory_exchange", s =>
                        {
                            s.RoutingKey = "product_deleted";
                            s.ExchangeType = "direct";
                        });
                        e.Consumer<ProductDeletedEventHandler>();

                        e.UseMessageRetry(r => r.Immediate(3));
                        e.UseDelayedRedelivery(r => r.Intervals(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10)));

                        e.UseCircuitBreaker(cb =>
                        {
                            cb.TrackingPeriod = TimeSpan.FromMinutes(15);
                            cb.TripThreshold = 2; // 15% de fallas
                            cb.ActiveThreshold = 1; // Mínimo 10 intentos antes de evaluar
                            cb.ResetInterval = TimeSpan.FromSeconds(20);
                        });
                        e.BindDeadLetterQueue("inventory_exchange_failed", "failed_product_deleted_queue", dlq =>
                        {
                            dlq.ExchangeType = "direct";
                        });
                    });
                });
            });
        }
    }
}
