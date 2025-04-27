using System.ComponentModel.DataAnnotations;

namespace Inventory.Domain.Models.Entities
{
    public class Product
    {
        public Product(string name, string description, decimal price, int stock, string category)
        {
            Name = name;
            Description = description;
            Price = price;
            Stock = stock;
            Category = category;
        }
        [Key] 
        public virtual int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        public string Category { get; set; }
    }
}
