using Serilog;
using Inventory.Application.Interfaces;
using Inventory.Application.Services;
using Inventory.Infrastructure.Repositories;
using Inventory.Domain.Interfaces;
using Inventory.Infrastructure.Data;  // Agregar espacio de nombres para el contexto de EF
using Microsoft.EntityFrameworkCore; // Necesario para la configuración de EF
using Inventory.Domain.Models.Entities;
using Inventory.Application.Mappings;
using Inventory.Infrastructure.RabbitMQ;
using Inventory.Infrastructure.Middlewares;
using Inventory.API.Configs;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()  // Loguear en la consola
    .CreateLogger();

// Usar Serilog en el sistema de logging global
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();  // Registrar el acceso al contexto HTTP

// Configuración de MassTransit con RabbitMQ
builder.Services.ConfigureMassTransit();

// Registrar el publisher en el contenedor DI
builder.Services.AddScoped<IRabbitMQPublisher, RabbitMQPublisher>();
builder.Services.AddScoped<IProductsService, ProductsService>();
builder.Services.AddScoped<IProductsRepository, ProductsRepository>();
var connString = builder.Configuration.GetConnectionString("DefaultConnection");
Environment.SetEnvironmentVariable("SQLSERVER_CONNECTIONSTRING", connString);

// Configurar Entity Framework Core para SQL Server
builder.Services.AddDbContext<InventoryContext>(options =>
    options.UseSqlServer(connString));  // Cadena de conexión para SQL Server

// Registrar AutoMapper
builder.Services.AddAutoMapper(typeof(ProductProfile));  // Esto escaneará los perfiles en el ensamblaje de Program

// Swagger/OpenAPI configuración
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<InventoryContext>();
    context.Database.EnsureCreated();  // Aplica las migraciones de la base de datos

    // Verificar si ya existen productos en la base de datos
    if (!context.Products.Any())  // Asegúrate de tener la entidad "Products"
    {
        // Crear productos por defecto
        context.Products.AddRange(
            new Product("Producto 1", "Descripción del producto 1", 10.99m, 100, "Categoría A"),
            new Product("Producto 2", "Descripción del producto 2", 19.99m, 50, "Categoría B"),
            new Product("Producto 3", "Descripción del producto 3", 5.99m, 200, "Categoría C")
        );
        context.SaveChanges();  // Guardar los productos en la base de datos
    }
}

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
app.UseMiddleware<TraceIdMiddleware>();

// Iniciar la aplicación
app.Run();
