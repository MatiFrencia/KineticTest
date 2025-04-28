using Microsoft.EntityFrameworkCore;
using Notification.Service;
using Notification.Service.Configs;
using Notification.Service.Data;
using Serilog;

// 1. Configurar Serilog *antes* del builder
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = Host.CreateApplicationBuilder(args);

// 2. Limpiar los providers de logging predeterminados
builder.Logging.ClearProviders();

// 3. Usar sólo Serilog
builder.Logging.AddSerilog(Log.Logger);
builder.Services.ConfigureMassTransitConsumers();
var connString = builder.Configuration.GetConnectionString("DefaultConnection");
Environment.SetEnvironmentVariable("SQLSERVER_CONNECTIONSTRING", connString);

// Configurar Entity Framework Core para SQL Server
builder.Services.AddDbContext<NotificationContext>(options =>
    options.UseSqlServer(connString));  // Cadena de conexión para SQL Server


var host = builder.Build();
ServiceProviderHelper.ServiceProvider = host.Services;

using (var scope = host.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<NotificationContext>();
    context.Database.EnsureCreated();  // Aplica las migraciones de la base de datos
}
host.Run();
