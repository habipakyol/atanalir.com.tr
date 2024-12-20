// Hubs/ChatHub.cs
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using AtanAlir.Core.Models;
using System.Security.Claims;


[Authorize]
public class ChatHub : Hub
{
    private static readonly Dictionary<string, DateTime> _lastMessageTimes = new();
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(ILogger<ChatHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            _lastMessageTimes.Remove(userId);
        }
        return base.OnDisconnectedAsync(exception);
    }

    public async Task JoinMatch(int matchId)
    {
        try
        {
            _logger.LogInformation("Client {ConnectionId} joining match {MatchId}", Context.ConnectionId, matchId);
            await Groups.AddToGroupAsync(Context.ConnectionId, matchId.ToString());
            await Clients.Caller.SendAsync("JoinedMatch", matchId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining match {MatchId}", matchId);
            throw;
        }
    }

    public async Task LeaveMatch(int matchId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, matchId.ToString());
    }

    public async Task SendMessage(int matchId, string message, string? quotedMessage = null)
    {
        try
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var nickname = Context.User?.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(nickname))
            {
                throw new HubException("Kullanıcı bilgileri bulunamadı");
            }

            // Son mesaj zamanını kontrol et
            if (_lastMessageTimes.TryGetValue(userId, out DateTime lastMessageTime))
            {
                var timeSinceLastMessage = DateTime.UtcNow - lastMessageTime;
                if (timeSinceLastMessage.TotalSeconds < 5)
                {
                    throw new HubException($"Çok hızlı mesaj gönderiyorsunuz. Lütfen {5 - Math.Ceiling(timeSinceLastMessage.TotalSeconds)} saniye bekleyin.");
                }
            }

            var chatMessage = new ChatMessage
            {
                UserId = userId,
                Nickname = nickname,
                Message = message,
                QuotedMessage = quotedMessage,
                Timestamp = DateTime.UtcNow,
                MatchId = matchId
            };

            // Son mesaj zamanını güncelle
            _lastMessageTimes[userId] = DateTime.UtcNow;

            await Clients.Group(matchId.ToString()).SendAsync("ReceiveMessage", chatMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message");
            throw;
        }
    }
}