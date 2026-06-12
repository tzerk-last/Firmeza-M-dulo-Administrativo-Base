using Blazored.LocalStorage;
using Firmeza.Client.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Firmeza.Client.Services;

public class VentaService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;

    public VentaService(HttpClient http, ILocalStorageService localStorage)
    {
        _http = http;
        _localStorage = localStorage;
    }

    public async Task<VentaResponse?> CrearVentaAsync(CrearVentaRequest request)
    {
        var token = await _localStorage.GetItemAsync<string>("token");
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _http.PostAsJsonAsync("api/Ventas", request);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<VentaResponse>();
    }
}
