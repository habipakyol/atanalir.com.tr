using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtanAlir.Core.Models
{
    // Models/ChatMessage.cs
    public class ChatMessage
    {
        public string UserId { get; set; } = null!;
        public string Nickname { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string? QuotedMessage { get; set; }
        public DateTime Timestamp { get; set; }
        public int MatchId { get; set; }
    }
}
