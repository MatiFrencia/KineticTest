using Microsoft.EntityFrameworkCore;
using Notification.Service.Models;


namespace Notification.Service.Data
{
    public class NotificationContext : DbContext
    {
        public NotificationContext()
        {
        }

        public NotificationContext(DbContextOptions<NotificationContext> options)
            : base(options)
        {
        }

        public virtual DbSet<EventLogs> EventLogs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Definir EventId como clave primaria
            modelBuilder.Entity<EventLogs>()
                .HasKey(e => e.EventId);  // Esto hace que EventId sea la clave primaria
        }
#if DEBUG
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connString = Environment.GetEnvironmentVariable("SQLSERVER_CONNECTIONSTRING");
            if (string.IsNullOrEmpty(connString))
            {
                // En caso de que no esté configurado, usar la ruta por defecto
                connString = "Server=sqlserver;Database=NotificationDB;User=sa;Password=MatiFrencia11;TrustServerCertificate=True;";
            }
            optionsBuilder.UseSqlServer(connString);
        }
#else
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Usamos la variable de entorno definida en docker-compose para obtener la ruta
            var connString = Environment.GetEnvironmentVariable("SQLSERVER_CONNECTIONSTRING");
            if (string.IsNullOrEmpty(connString))
            {
                // En caso de que no esté configurado, usar la ruta por defecto
                connString = "Server=sqlserver;Database=NotificationDB;User=sa;Password=MatiFrencia11;TrustServerCertificate=True;";
            }
            optionsBuilder.UseSqlServer(connString);
        }
#endif
    }
}
