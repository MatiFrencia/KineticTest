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
            var connString = "Data Source=/data/inventory.db"; // Ruta de la base de datos SQLite
            optionsBuilder.UseSqlite(connString);
        }
#else
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Usamos la variable de entorno definida en docker-compose para obtener la ruta
            var connString = Environment.GetEnvironmentVariable("SQLITE_DB_PATH");
            if (string.IsNullOrEmpty(connString))
            {
                // En caso de que no esté configurado, usar la ruta por defecto
                connString = "Data Source=/data/inventory.db";
            }
            optionsBuilder.UseSqlite(connString);
        }
#endif
    }
}
