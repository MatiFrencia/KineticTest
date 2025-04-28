using AutoMapper;
using Inventory.Application.Interfaces;
using Inventory.Domain.Interfaces;
using Inventory.Domain.Models.DTOs;
using Inventory.Domain.Models.Entities;
using inventory_exchange;

namespace Inventory.Application.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IProductsRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IRabbitMQPublisher _rabbitMQPublisher;

        public ProductsService(IProductsRepository productRepository, IMapper mapper, IRabbitMQPublisher rabbitMQPublisher)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _rabbitMQPublisher = rabbitMQPublisher;
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

            // Crear evento de creación
            var createEvent = new ProductCreatedEvent
            {
                ProductId = response.Id,
                Name = response.Name,
                Description = response.Description,
                Price = response.Price,
                Stock = response.Stock,
                Category = response.Category
            };

            // Publicar el evento
            await _rabbitMQPublisher.PublishEvent(createEvent);

            return _mapper.Map<Product_DTO>(response);
        }

        public async Task<Product_DTO> Update(Product_DTO dto)
        {
            var product = _mapper.Map<Product>(dto);
            var response = await _productRepository.Update(product);

            // Crear evento de actualización
            var updateEvent = new ProductUpdatedEvent
            {
                ProductId = response.Id,
                Name = response.Name,
                Description = response.Description,
                Price = response.Price,
                Stock = response.Stock,
                Category = response.Category
            };

            // Publicar el evento
            await _rabbitMQPublisher.PublishEvent(updateEvent);

            return _mapper.Map<Product_DTO>(response);
        }

        public async Task<bool> Delete(int id)
        {
            var deleted = await _productRepository.Delete(id);

            if (deleted)
            {
                // Crear evento de eliminación
                var deleteEvent = new ProductDeletedEvent
                {
                    ProductId = id
                };
                // Publicar el evento
                await _rabbitMQPublisher.PublishEvent(deleteEvent);
            }

            return deleted;
        }
    }
}
