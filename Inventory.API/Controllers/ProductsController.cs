using AutoMapper;
using Inventory.Application.Interfaces;
using Inventory.Domain.Models.DTOs;
using Inventory.Domain.Models.Requests;
using Inventory.Domain.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IProductsService _productsService;

    public ProductsController(IProductsService productsService, IMapper mapper)
    {
        _productsService = productsService;
        _mapper = mapper;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await _productsService.GetAll();
        return Ok(response);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _productsService.GetById(id);
        return response == null ? NotFound() : Ok(response);
    }
    [HttpPost]
    public async Task<IActionResult> Create(Product_Request request)
    {
        var dto = _mapper.Map<Product_DTO>(request);
        dto = await _productsService.Add(dto);
        var response = _mapper.Map<Product_Response>(dto);
        return CreatedAtAction(nameof(Create), new { id = response.Id }, response);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Product_Request request)
    {
        var dto = _mapper.Map<Product_DTO>(request);
        dto.Id = id;
        dto = await _productsService.Update(dto);
        var response = _mapper.Map<Product_Response>(dto);
        return Ok(response);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _productsService.Delete(id);
        return deleted ? Ok(id) : Problem();
    }
}
