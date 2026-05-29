using System.Text.Json;
using VShop.Web.Models;

namespace VShop.Web.Services.Contracts;

public class CategoryService : ICategoryService
{
    private readonly IHttpClientFactory _httpClientFactory;//criar instâncias de HttpClient.
    private const string apiEndpoint = "/api/categories/";
    private readonly JsonSerializerOptions _options;// Configuração do serializador JSON.
    public CategoryService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _options = new JsonSerializerOptions{ PropertyNameCaseInsensitive = true};
    }
    public async Task<IEnumerable<CategoryViewModel>> GetAllCategories()
    {
        var client = _httpClientFactory.CreateClient("ProductApi"); // 1 - Cria um HttpClient configurado.

        IEnumerable<CategoryViewModel> categories;

        using (var response = await client.GetAsync(apiEndpoint))// 2 - Realiza uma requisição GET.
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();// 3 - Lê o JSON retornado pela API.
                categories = await JsonSerializer.
                    DeserializeAsync<IEnumerable<CategoryViewModel>>(apiResponse, _options);// 4 - Desserializa o JSON para IEnumerable<ProductViewModel>.
            }
            else
                return null;
        }
        return categories;
    }
}
