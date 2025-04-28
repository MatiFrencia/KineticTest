using AutoMapper;
using Inventory.Application.Interfaces;
using Inventory.Domain.Interfaces;
using Inventory.Domain.Models.DTOs;
using Inventory.Domain.Models.Entities;

namespace Inventory.Application.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IProductsRepository _productRepository;
        private readonly IMapper _mapper;
        public ProductsService(IProductsRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }
        public async Task<List<Product_DTO>> GetAll()
        {
            var response = await _productRepository.GetAll();
            return _mapper.Map<List<Product_DTO>>(response);
        }
        public async Task<Product_DTO> GetById(int id)
        {
            var response = await _productRepository.GetById(id);
            return _mapper.Map<Product_DTO>(response);
        }
        public async Task<Product_DTO> Add(Product_DTO dto)
        {
            var product = _mapper.Map<Product>(dto);
            var response = await _productRepository.Add(product);
            return _mapper.Map<Product_DTO>(response);
        }
        public async Task<Product_DTO> Update(Product_DTO dto)
        {
            var product = _mapper.Map<Product>(dto);
            var response = await _productRepository.Update(product);
            return _mapper.Map<Product_DTO>(response);
        }
        public async Task<bool> Delete(int id)
        {
            var deleted = await _productRepository.Delete(id);
            return deleted;
        }
    }
}
