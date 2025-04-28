using MassTransit;
using Notification.Service.Middleware;
using Notification.Service.RabbitMQ.Handlers.Products;

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
                        // Asociar la cola al exchange y configurar el enrutamiento
                        e.Bind("inventory_exchange", s =>
                        {
                            s.RoutingKey = "product_created"; // Clave de enrutamiento para el mensaje
                            s.ExchangeType = "direct"; // Tipo de exchange directo
                        });
                        e.Consumer<ProductCreatedEventHandler>();
                        // Configuración de Dead Letter Queue (DLQ)
                        e.UseMessageRetry(r => r.Immediate(3)); // 3 reintentos antes de pasar a la DLQ
                        e.UseDelayedRedelivery(r => r.Intervals(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10))); // Retrasos entre los reintentos
                                                                                                                     // Configurar Dead Letter Queue (DLQ) con MassTransit
                        e.BindDeadLetterQueue("inventory_exchange_failed", "failed_product_created_queue", dlq =>
                        {
                            dlq.ExchangeType = "direct";
                        });

                        // Configurar reintentos automáticos para la cola
                        e.UseMessageRetry(r => r.Immediate(3)); // 3 reintentos antes de pasar a la DLQ
                    });

                    cfg.ReceiveEndpoint("product_updated_queue", e =>
                    {
                        // Asociar la cola al exchange y configurar el enrutamiento
                        e.Bind("inventory_exchange", s =>
                        {
                            s.RoutingKey = "product_updated"; // Clave de enrutamiento para el mensaje
                            s.ExchangeType = "direct"; // Tipo de exchange directo
                        });
                        e.Consumer<ProductUpdatedEventHandler>();
                    });

                    cfg.ReceiveEndpoint("product_deleted_queue", e =>
                    {
                        // Asociar la cola al exchange y configurar el enrutamiento
                        e.Bind("inventory_exchange", s =>
                        {
                            s.RoutingKey = "product_deleted"; // Clave de enrutamiento para el mensaje
                            s.ExchangeType = "direct"; // Tipo de exchange directo
                        });
                        e.Consumer<ProductDeletedEventHandler>();
                    });

                });
            });
        }
    }
}
