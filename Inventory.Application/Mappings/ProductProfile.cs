using AutoMapper;
using Inventory.Domain.Models.DTOs;
using Inventory.Domain.Models.Entities;
using Inventory.Domain.Models.Requests;
using Inventory.Domain.Models.Responses;

namespace Inventory.Application.Mappings
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product_Response, Product_DTO>().ReverseMap();
            CreateMap<Product, Product_DTO>().ReverseMap();
            CreateMap<Product, Product_Response>().ReverseMap();
            CreateMap<Product, Product_Request>().ReverseMap();
            CreateMap<Product_Request, Product_DTO>().ReverseMap();
            CreateMap<Product_Request, Product_Response>().ReverseMap();
        }
    }
}