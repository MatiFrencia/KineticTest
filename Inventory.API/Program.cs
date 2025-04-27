using Serilog;
using MassTransit;
using Inventory.Application.Interfaces;
using Inventory.Infrastructure.Messaging;
using Inventory.Domain.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()  // Loguear en la consola
    .CreateLogger();

// Usar Serilog en el sistema de logging glob

// Add services to the container.
builder.Services.AddControllers();

// Configuración de MassTransit con RabbitMQ
builder.Services.AddMassTransit(x =>
{
    // Configuración de RabbitMQ
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        // Aquí puedes agregar más configuraciones de RabbitMQ si lo necesitas
    });
});

// Registrar el publisher en el contenedor DI
builder.Services.AddScoped<IRabbitMQPublisher, RabbitMQPublisher>();

// Swagger/OpenAPI configuración
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar el pipeline de solicitud HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.UseMiddleware<ExceptionLoggingMiddleware>();

// Iniciar la aplicación
app.Run();
