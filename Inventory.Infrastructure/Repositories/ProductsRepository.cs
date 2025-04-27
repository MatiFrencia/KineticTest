using System.Linq.Expressions;
using AutoMapper;
using Inventory.Domain.Exceptions.Repository;
using Inventory.Domain.Interfaces;
using Inventory.Domain.Models.Entities;
using Inventory.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

public class ProductsRepository : IProductsRepository
{
    private readonly InventoryContext _context;
    private readonly string _entityName = "product";

    public ProductsRepository(InventoryContext context, IMapper mapper)
    {
        _context = context;

        if (!_context.Database.CanConnect())
        {
            throw new LostDatabaseConnection_Exception(_entityName);
        }
    }

    public async Task<List<Product>> GetAll()
    {
        var response = await _context.Products.AsNoTracking().ToListAsync();
        if (response == null || response.Count == 0)
            throw new NotFound_Exception(_entityName, "all");
        return response;
    }

    public async Task<Product> GetById(int id)
    {
        return await _context.Products.FirstOrDefaultAsync(x => x.Id == id) ?? throw new NotFound_Exception(_entityName, id.ToString());
    }

    public async Task<Product> Add(Product product)
    {
        try
        {
            if (await Any(x => x.Name == product.Name))
                throw new AlreadyExists_Exception(_entityName, product.Name);

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new FailedToAdd_Exception(_entityName, product.Name, ex);
        }
        return product;
    }

    public async Task<Product> Update(Product product)
    {
        var existingProduct = await _context.Products.FindAsync(product.Id) ?? throw new NotFound_Exception(_entityName, product.Id.ToString());
        try
        {
            _context.Entry(existingProduct).CurrentValues.SetValues(product);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new FailedToUpdate_Exception(_entityName, product.Id.ToString(), ex);
        }
        return existingProduct;
    }

    public async Task<bool> Delete(int id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id) ?? throw new NotFound_Exception(_entityName, id.ToString());

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new FailedToDelete_Exception(_entityName, id.ToString(), ex);
        }

        return true;
    }

    public async Task<bool> Any(Expression<Func<Product, bool>> predicate)
    {
        return await _context.Products.AnyAsync(predicate);
    }

    public async Task<int> Count(Expression<Func<Product, bool>> predicate)
    {
        return await _context.Products.CountAsync(predicate);
    }
}
