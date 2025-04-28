using Inventory.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Data
{
    public class InventoryContext : DbContext
    {
        public InventoryContext()
        {
        }

        public InventoryContext(DbContextOptions<InventoryContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Product> Products { get; set; }

#if DEBUG
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //var connString = "Server=localhost,1434;Database=InventoryDB;User=sa;Password=MatiFrencia11;TrustServerCertificate=True;";
            var connString = Environment.GetEnvironmentVariable("SQLSERVER_CONNECTIONSTRING");
            if (string.IsNullOrEmpty(connString))
            {
                // En caso de que no esté configurado, usar la ruta por defecto
                connString = "Server=sqlserver;Database=InventoryDB;User=sa;Password=MatiFrencia11;TrustServerCertificate=True;";
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
                connString = "Server=sqlserver;Database=InventoryDB;User=sa;Password=MatiFrencia11;TrustServerCertificate=True;";
            }
            optionsBuilder.UseSqlServer(connString);
        }
#endif
    }
}
