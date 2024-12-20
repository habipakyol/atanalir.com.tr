using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtanAlir.Core.Models
{
    public class Match
    {
        public int Id { get; set; }
        public string HomeTeam { get; set; } = null!;
        public string AwayTeam { get; set; } = null!;
        public MatchScore? Score { get; set; }
        public string Status { get; set; } = null!;
        public DateTime StartTime { get; set; }
    }

    public class MatchScore
    {
        public int Home { get; set; }
        public int Away { get; set; }
    }

    public class MatchesResponse
    {
        public List<Match> LiveMatches { get; set; } = new();
        public List<Match> UpcomingMatches { get; set; } = new();
    }
}
