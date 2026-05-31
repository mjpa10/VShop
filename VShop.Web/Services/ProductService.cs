using System.Net.Http.Headers;
using System.Text.Json;
using System.Xml.Linq;
using VShop.Web.Models;
using VShop.Web.Services.Contracts;

namespace VShop.Web.Services;

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

    public async Task<IEnumerable<ProductViewModel>> GetAllProducts(string token)
    {
        var client = _httpClientFactory.CreateClient("ProductApi"); // 1 - Cria um HttpClient configurado.
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

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

    public async Task<ProductViewModel> FindProductById(int id, string token)
    {
        var client = _httpClientFactory.CreateClient("ProductApi");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


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
    public async Task<ProductViewModel> FindProductByName(string name, string token)
    {
        var client = _httpClientFactory.CreateClient("ProductApi");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

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
    public async Task<ProductViewModel> CreateProduct(ProductViewModel productVM, string token)
    {
        var client = _httpClientFactory.CreateClient("ProductApi");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

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
    public async Task<ProductViewModel> UpdateProduct(ProductViewModel productVM, string token)
    {
        var client = _httpClientFactory.CreateClient("ProductApi");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

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
    public async Task<bool> DeleteProductById(int id, string token)
    {
        var client = _httpClientFactory.CreateClient("ProductApi");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

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
