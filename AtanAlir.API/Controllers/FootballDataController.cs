using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace AtanAlir.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FootballDataController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<FootballDataController> _logger;

    public FootballDataController(IConfiguration configuration, ILogger<FootballDataController> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.football-data.org/v4/")
        };
        _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", _configuration["FootballData:ApiKey"]);
    }

    [HttpGet("matches/today")]
    public async Task<IActionResult> GetTodayMatches()
    {
        try
        {
            // Premier League ID'si: 2021
            var response = await _httpClient.GetAsync($"competitions/2021/matches?season=2024");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Football Data API error: {response.StatusCode}");
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error content: {errorContent}");
                return StatusCode((int)response.StatusCode, "Error fetching matches from external API");
            }

            var content = await response.Content.ReadAsStringAsync();
            return Ok(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting today's matches");
            return BadRequest($"Error fetching match data: {ex.Message}");
        }
    }

    [HttpGet("matches/{id}")]
    public async Task<IActionResult> GetMatch(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"matches/{id}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Football Data API error: {response.StatusCode}");
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error content: {errorContent}");
                return StatusCode((int)response.StatusCode, "Error fetching match details from external API");
            }

            var content = await response.Content.ReadAsStringAsync();
            return Ok(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting match details");
            return BadRequest($"Error fetching match data: {ex.Message}");
        }
    }
}