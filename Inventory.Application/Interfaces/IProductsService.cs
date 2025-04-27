using Inventory.Domain.Models.DTOs;

namespace Inventory.Application.Interfaces;

public interface IProductsService
{
    public Task<Product_DTO> Add(Product_DTO dto);
    public Task<bool> Delete(int id);
    public Task<Product_DTO> Update(Product_DTO dto);
    public Task<List<Product_DTO>> GetAll();
    public Task<Product_DTO> GetById(int id);
}