using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace RaceDayMVC.Services;

public class RaceDayApiClient
{
    private readonly IHttpClientFactory _factory;
    private readonly IHttpContextAccessor _accessor;

    private readonly JsonSerializerOptions _options =
        new()
        {
            PropertyNameCaseInsensitive = true
        };

    public RaceDayApiClient(
        IHttpClientFactory factory,
        IHttpContextAccessor accessor)
    {
        _factory = factory;
        _accessor = accessor;
    }

    private HttpClient CreateClient()
    {
        var client = _factory.CreateClient("RaceDayAPI");

        var token = _accessor.HttpContext?
            .Session.GetString("Token");

        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer", token);
        }

        return client;
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        var response = await CreateClient().GetAsync(endpoint);

        if (!response.IsSuccessStatusCode)
            return default;

        var json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<T>(
            json, _options);
    }

    public async Task<bool> PostAsync<T>(
        string endpoint, T data)
    {
        var json = JsonSerializer.Serialize(data);

        var content = new StringContent(
            json, Encoding.UTF8, "application/json");

        var response = await CreateClient()
            .PostAsync(endpoint, content);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> PutAsync<T>(
        string endpoint, T data)
    {
        var json = JsonSerializer.Serialize(data);

        var content = new StringContent(
            json, Encoding.UTF8, "application/json");

        var response = await CreateClient()
            .PutAsync(endpoint, content);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(string endpoint)
    {
        var response = await CreateClient()
            .DeleteAsync(endpoint);

        return response.IsSuccessStatusCode;
    }

    public async Task<JsonDocument?> PostJsonAsync<T>(
        string endpoint, T data)
    {
        var json = JsonSerializer.Serialize(data);

        var content = new StringContent(
            json, Encoding.UTF8, "application/json");

        var response = await CreateClient()
            .PostAsync(endpoint, content);

        if (!response.IsSuccessStatusCode)
            return null;

        var result = await response.Content.ReadAsStringAsync();

        return JsonDocument.Parse(result);
    }
}