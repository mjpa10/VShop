using System.Text.Json;
using System.Xml.Linq;
using VShop.Web.Models;

namespace VShop.Web.Services.Contracts;

public class ProductService : IProductService
{
    private readonly IHttpClientFactory _httpClientFactory;//criar instâncias de HttpClient.
    private const string apiEndpoint = "/api/products/";
    private readonly JsonSerializerOptions _options;// Configuração do serializador JSON.
    private ProductViewModel productVM; // Objeto utilizado para armazenar um único produto.
    private IEnumerable<ProductViewModel> productsVM;// Coleção utilizada para armazenar múltiplos produtos.

    public ProductService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _options = new JsonSerializerOptions{ PropertyNameCaseInsensitive = true};
    }

    public async Task<IEnumerable<ProductViewModel>> GetAllProducts()
    {
        var client = _httpClientFactory.CreateClient("ProductApi"); // 1 - Cria um HttpClient configurado.

        using (var response = await client.GetAsync(apiEndpoint))// 2 - Realiza uma requisição GET.
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();// 3 - Lê o JSON retornado pela API.
                productsVM = await JsonSerializer.
                    DeserializeAsync<IEnumerable<ProductViewModel>>(apiResponse, _options);// 4 - Desserializa o JSON para IEnumerable<ProductViewModel>.
            }
            else
                return null;
        }

        return productsVM;

    }

    public async Task<ProductViewModel> FindProductById(int id)
    {
        var client = _httpClientFactory.CreateClient("ProductApi");

        using (var response = await client.GetAsync(apiEndpoint + "id/" + id))// - Realiza uma requisição GET pelo Id.
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                productVM = await JsonSerializer.
                    DeserializeAsync<ProductViewModel>(apiResponse, _options);// - Desserializa o JSON para <ProductViewModel>.
            }
            else
                return null;
        }

        return productVM;
    }
    public async Task<ProductViewModel> FindProductByName(string name)
    {
        var client = _httpClientFactory.CreateClient("ProductApi");

        using (var response = await client.GetAsync(apiEndpoint + "name/" + name))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                productVM = await JsonSerializer.
                    DeserializeAsync<ProductViewModel>(apiResponse, _options);
            }
            else
                return null;
        }

        return productVM;
    }

    public async Task<ProductViewModel> CreateProduct(ProductViewModel productVM)
    {
        var client = _httpClientFactory.CreateClient("ProductApi");

        StringContent content = new StringContent(
            JsonSerializer.Serialize(productVM), 
            System.Text.Encoding.UTF8, "application/json");// 1 - Serializa o objeto ProductViewModel em JSON.

        using (var response = await client.PostAsync(apiEndpoint, content)) // 2 - Envia um POST para a API.
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                productVM = await JsonSerializer.
                    DeserializeAsync<ProductViewModel>(apiResponse, _options); // 3 - Recebe o produto criado.
            }
            else
                return null;
        }

        return productVM;
    }

    public async Task<ProductViewModel> UpdateProduct(ProductViewModel productVM)
    {
        var client = _httpClientFactory.CreateClient("ProductApi");
        ProductViewModel productUpdated = new ProductViewModel();

        using (var response = await client.PutAsJsonAsync(apiEndpoint, productVM))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                productUpdated = await JsonSerializer.
                    DeserializeAsync<ProductViewModel>(apiResponse, _options);
            }
            else
                return null;
        }

        return productUpdated;
    }

    public async Task<bool> DeleteProductById(int id)
    {
        var client = _httpClientFactory.CreateClient("ProductApi");

        using (var response = await client.DeleteAsync(apiEndpoint + id))
        {
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
        }
        return false;
    }
}
