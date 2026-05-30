using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VShop.Web.Models;
using VShop.Web.Roles;
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
        var tokenJwt = await HttpContext.GetTokenAsync("access_token"); // 1 - Obtém o token JWT do contexto HTTP atual.
        var result = await _productService.GetAllProducts(tokenJwt);

        if (result is null)
            return View("Error");

        return View(result);
    }
    [HttpGet] // Método responsável por exibir a tela de criação de produtos.
    public async Task<ActionResult> CreateProduct()
    {
        var tokenJwt = await HttpContext.GetTokenAsync("access_token");

        ViewBag.CategoryId = new SelectList(await  // 1 - Busca todas as categorias na API. // 2 - Cria um SelectList para popular o dropdown. // 3 - Envia os dados para a View utilizando ViewBag.
            _categoryService.GetAllCategories(tokenJwt), "CategoryId", "Name");

        return View(); // 4 - Retorna a página CreateProduct.
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult> CreateProduct(ProductViewModel productVM)
    {
        var tokenJwt = await HttpContext.GetTokenAsync("access_token");

        if (ModelState.IsValid)
        {
           
            var result = await _productService.CreateProduct(productVM, tokenJwt); // 3 - Envia os dados para a API.

            if (result is not null)
                return RedirectToAction(nameof(Index)); // 4 - Redireciona para a página Index após sucesso.
        }
        else
        {
            ViewBag.categoryId = new SelectList(await 
                _categoryService.GetAllCategories(tokenJwt), "CategoryId", "Name"); // 5 - Caso haja erro, recria o dropdown de categorias.
        }
        return View(productVM);
     }

    [HttpGet] 
    public async Task<ActionResult> UpdateProduct(int id)
    {
        var tokenJwt = await HttpContext.GetTokenAsync("access_token");

        ViewBag.CategoryId = new SelectList(await  // 1 - Busca todas as categorias na API. // 2 - Cria um SelectList para popular o dropdown. // 3 - Envia os dados para a View utilizando ViewBag.
            _categoryService.GetAllCategories(tokenJwt), "CategoryId", "Name");

        var result = await _productService.FindProductById(id, tokenJwt); // 4 - Busca os dados do produto a ser editado.
        
        if (result is null)
            return View("Error");

        return View(result); // 5 - Retorna a View preenchida com os dados do produto.
    }
    [HttpPost]
    [Authorize]
    public async Task<ActionResult> UpdateProduct(ProductViewModel productVM)
    {
        var tokenJwt = await HttpContext.GetTokenAsync("access_token");

        if (ModelState.IsValid)
        {
            var result = await _productService.UpdateProduct(productVM, tokenJwt); // 3 - Envia os dados para a API.

            if (result is not null)
                return RedirectToAction(nameof(Index)); // 4 - Redireciona para a página Index após sucesso.
        }
        return View(productVM);
    }
    [HttpGet]
    [Authorize]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var tokenJwt = await HttpContext.GetTokenAsync("access_token");

        var result = await _productService.FindProductById(id, tokenJwt); // 1 - Busca os dados do produto a ser editado.

        if (result is null)// 2 - Verifica se o produto existe.
            return View("Error");

        return View(result);
    }
    [HttpPost(), ActionName("DeleteProduct")]// ActionName("DeleteProduct") permite que o método POST utilize a mesma rota do método GET.
    [Authorize(Roles = Role.Admin)]
    public async Task<ActionResult> DeleteConfirmed(int id)
    {
        var tokenJwt = await HttpContext.GetTokenAsync("access_token");

        var result = await _productService.DeleteProductById(id, tokenJwt);// 1 - Envia a solicitação DELETE para a API.

        if (!result)
            return View("Error");

        return RedirectToAction("Index");// 2 - Redireciona para Index.
    }
}
