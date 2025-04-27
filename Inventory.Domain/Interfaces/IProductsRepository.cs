using Inventory.Domain.Models.Entities;
using System.Linq.Expressions;

namespace Inventory.Domain.Interfaces;

public interface IProductsRepository
{
    public Task<Product> Add(Product dto);
    public Task<bool> Delete(int id);
    public Task<Product> Update(Product dto);
    public Task<List<Product>> GetAll();
    public Task<Product> GetById(int id);
    public Task<bool> Any(Expression<Func<Product, bool>> predicate);
    public Task<int> Count(Expression<Func<Product, bool>> predicate);

}