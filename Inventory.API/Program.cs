using Serilog;
using MassTransit;
using Inventory.Application.Interfaces;
using Inventory.Infrastructure.Messaging;
using Inventory.Domain.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Configuraci�n de Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()  // Loguear en la consola
    .CreateLogger();

// Usar Serilog en el sistema de logging glob

// Add services to the container.
builder.Services.AddControllers();

// Configuraci�n de MassTransit con RabbitMQ
builder.Services.AddMassTransit(x =>
{
    // Configuraci�n de RabbitMQ
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        // Aqu� puedes agregar m�s configuraciones de RabbitMQ si lo necesitas
    });
});

// Registrar el publisher en el contenedor DI
builder.Services.AddScoped<IRabbitMQPublisher, RabbitMQPublisher>();

// Swagger/OpenAPI configuraci�n
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

// Iniciar la aplicaci�n
app.Run();
