using Blazored.LocalStorage;
using Firmeza.Client.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Firmeza.Client.Services;

public class ProductoService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;

    public ProductoService(HttpClient http, ILocalStorageService localStorage)
    {
        _http = http;
        _localStorage = localStorage;
    }

    private async Task AgregarTokenAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("token");
        if (!string.IsNullOrEmpty(token))
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<List<ProductoModel>> GetProductosAsync()
    {
        await AgregarTokenAsync();
        return await _http.GetFromJsonAsync<List<ProductoModel>>("api/Productos") ?? new();
    }
}
