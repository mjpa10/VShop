using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VShop.ProductApi.DTOs;
using VShop.ProductApi.Roles;
using VShop.ProductApi.Services;

namespace VShop.ProductApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> Get()
    {
        var productsDto = await _productService.GetProducts();

        if (productsDto == null)
            return NotFound("Products Not Found");

        return Ok(productsDto);
    }

    [HttpGet("id/{id:int}", Name = "GetProductId")]
    public async Task<ActionResult<ProductDTO>> Get(int id)
    {
        var productsDto = await _productService.GetProductById(id);

        if (productsDto == null)
            return NotFound("Product Not Found");

        return Ok(productsDto);
    }

    [HttpGet("name/{name}", Name = "GetProductName")]
    public async Task<ActionResult<ProductDTO>> Get(string name)
    {
        var productsDto = await _productService.GetProductByName(name);

        if (productsDto == null)
            return NotFound("Product Not Found");

        return Ok(productsDto);
    }
    [HttpPost]
    [Authorize(Roles = Role.Admin)]
    public async Task<ActionResult> Post([FromBody] ProductDTO productDto)
    {
        if (productDto == null)
            return BadRequest("Invalid Data");

        await _productService.AddProduct(productDto);

        return new CreatedAtRouteResult(
            "GetProductId", 
            new { id = productDto.Id },
            productDto);
    }
    [HttpPut]
    [Authorize(Roles = Role.Admin)]
    public async Task<ActionResult> Put([FromBody] ProductDTO productDto)
    {
        if (productDto == null)
            return BadRequest("Data invalid");

        await _productService.UpdateProduct(productDto);

        return Ok(productDto);
    }
    [HttpDelete("{id:int}")]
    [Authorize(Roles = Role.Admin)]
    public async Task<ActionResult> Delete(int id)
    {
        var product = await _productService.GetProductById(id);

        if (product == null)
            return NotFound("Product Not Found");

        await _productService.RemoveProduct(id);
        return NoContent();
    }
}
