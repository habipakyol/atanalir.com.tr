using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace AtanAlir.Infrastructure.External;

public class FootballDataApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public FootballDataApiClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["FootballData:ApiKey"];

        _httpClient.BaseAddress = new Uri("http://api.football-data.org/v4/");
        _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", _apiKey);
    }

    public async Task<string> GetTodayMatchesAsync()
    {
        var response = await _httpClient.GetAsync($"competitions/TR1/matches?dateFrom={DateTime.Today:yyyy-MM-dd}&dateTo={DateTime.Today:yyyy-MM-dd}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}