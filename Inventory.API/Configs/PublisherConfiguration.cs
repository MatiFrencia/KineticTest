using inventory_exchange;
using MassTransit;

namespace Inventory.API.Configs
{
    public static class PublisherConfiguration
    {
        public static void ConfigureMassTransit(this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("rabbitmq", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });
                    cfg.UseMessageRetry(r => r.Immediate(3));


                    //cfg.Publish<ProductCreatedEvent>(x =>
                    //{
                    //    x.BindQueue("inventory_exchange", "product_created_queue",
                    //        s =>
                    //        {
                    //            s.RoutingKey = "product_created"; // Clave de enrutamiento para el mensaje
                    //            s.ExchangeType = "direct"; // Tipo de exchange directo
                    //        });
                    //});
                    //cfg.Publish<ProductUpdatedEvent>(x =>
                    //{
                    //    x.BindQueue("inventory_exchange", "product_updated_queue",
                    //        s =>
                    //        {
                    //            s.RoutingKey = "product_updated"; // Clave de enrutamiento para el mensaje
                    //            s.ExchangeType = "direct"; // Tipo de exchange directo
                    //        });
                    //});
                    //cfg.Publish<ProductDeletedEvent>(x =>
                    //{
                    //    x.BindQueue("inventory_exchange", "product_deleted_queue",
                    //        s =>
                    //        {
                    //            s.RoutingKey = "product_deleted"; // Clave de enrutamiento para el mensaje
                    //            s.ExchangeType = "direct"; // Tipo de exchange directo
                    //        });
                    //});

                });
            });
        }
    }
}
