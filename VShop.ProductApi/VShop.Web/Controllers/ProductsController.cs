using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VShop.Web.Models;
using VShop.Web.Services.Contracts;

namespace VShop.Web.Controllers;

public class ProductsController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    public ProductsController(IProductService productService, ICategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductViewModel>>> Index()
    {
        var result = await _productService.GetAllProducts();

        if (result is null)
            return View("Error");

        return View(result);
    }
    [HttpGet] // Método responsável por exibir a tela de criação de produtos.
    public async Task<ActionResult> CreateProduct()
    {
        ViewBag.CategoryId = new SelectList(await  // 1 - Busca todas as categorias na API. // 2 - Cria um SelectList para popular o dropdown. // 3 - Envia os dados para a View utilizando ViewBag.
            _categoryService.GetAllCategories(), "CategoryId", "Name");

        return View(); // 4 - Retorna a página CreateProduct.
    }

    [HttpPost]
    public async Task<ActionResult> CreateProduct(ProductViewModel productVM)
    {
        if (ModelState.IsValid)
        {
            var result = await _productService.CreateProduct(productVM); // 3 - Envia os dados para a API.

            if (result is not null)
                return RedirectToAction(nameof(Index)); // 4 - Redireciona para a página Index após sucesso.
        }
        else
        {
            ViewBag.categoryId = new SelectList(await 
                _categoryService.GetAllCategories(), "CategoryId", "Name"); // 5 - Caso haja erro, recria o dropdown de categorias.
        }
        return View(productVM);
     }
}
