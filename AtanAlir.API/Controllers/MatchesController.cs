using Microsoft.AspNetCore.Mvc;
using AtanAlir.Core.Models;

namespace AtanAlir.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchesController : ControllerBase
{
    private readonly List<Match> _matches = new()
    {
        new Match
        {
            Id = 1,
            HomeTeam = "Galatasaray",
            AwayTeam = "Fenerbahçe",
            Score = new MatchScore { Home = 2, Away = 1 },
            Status = "LIVE",
            StartTime = DateTime.Now.AddMinutes(-45)
        },
        new Match
        {
            Id = 2,
            HomeTeam = "Beşiktaş",
            AwayTeam = "Trabzonspor",
            Score = new MatchScore { Home = 1, Away = 1 },
            Status = "LIVE",
            StartTime = DateTime.Now.AddMinutes(-30)
        },
        new Match
        {
            Id = 3,
            HomeTeam = "Başakşehir",
            AwayTeam = "Adana Demirspor",
            Status = "SCHEDULED",
            StartTime = DateTime.Now.AddHours(2)
        },
        new Match
        {
            Id = 4,
            HomeTeam = "Konyaspor",
            AwayTeam = "Antalyaspor",
            Status = "SCHEDULED",
            StartTime = DateTime.Now.AddHours(3)
        }
    };

    [HttpGet]
    public ActionResult<MatchesResponse> GetMatches()
    {
        var response = new MatchesResponse
        {
            LiveMatches = _matches.Where(m => m.Status == "LIVE").ToList(),
            UpcomingMatches = _matches.Where(m => m.Status == "SCHEDULED").ToList()
        };

        return Ok(response);
    }

    [HttpGet("{matchId}")]
    public ActionResult<Match> GetMatch(int matchId)
    {
        var match = _matches.FirstOrDefault(m => m.Id == matchId);

        if (match == null)
            return NotFound();

        return Ok(match);
    }
}