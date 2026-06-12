using Blazored.LocalStorage;
using Firmeza.Client.Models;
using System.Net.Http.Json;

namespace Firmeza.Client.Services;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;

    public AuthService(HttpClient http, ILocalStorageService localStorage)
    {
        _http = http;
        _localStorage = localStorage;
    }

    public async Task<bool> LoginAsync(LoginRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/Auth/login", request);
        if (!response.IsSuccessStatusCode) return false;

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        if (auth == null) return false;

        await _localStorage.SetItemAsync("token", auth.Token);
        await _localStorage.SetItemAsync("nombre", auth.NombreCompleto);
        await _localStorage.SetItemAsync("email", auth.Email);
        await _localStorage.SetItemAsync("clienteId", auth.ClienteId);
        return true;
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/Auth/register", request);
        if (!response.IsSuccessStatusCode) return false;

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        if (auth == null) return false;

        await _localStorage.SetItemAsync("token", auth.Token);
        await _localStorage.SetItemAsync("nombre", auth.NombreCompleto);
        await _localStorage.SetItemAsync("email", auth.Email);
        await _localStorage.SetItemAsync("clienteId", auth.ClienteId);
        return true;
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync("token");
        await _localStorage.RemoveItemAsync("nombre");
        await _localStorage.RemoveItemAsync("email");
        await _localStorage.RemoveItemAsync("clienteId");
    }

    public async Task<string?> GetTokenAsync()
        => await _localStorage.GetItemAsync<string>("token");

    public async Task<string?> GetNombreAsync()
        => await _localStorage.GetItemAsync<string>("nombre");

    public async Task<int> GetClienteIdAsync()
        => await _localStorage.GetItemAsync<int>("clienteId");

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }
}
