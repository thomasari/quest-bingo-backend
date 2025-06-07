using NanoidDotNet;

namespace QuestBingo.Models;

public class Room(string id)
{
    public string Id { get; set; } = id;
    public List<Player> Players { get; set; } = [];
    public Board Board { get; set; }
    public List<ChatMessage> ChatHistory { get; set; } = new(); // <-- new
    public DateTimeOffset? GameStarted { get; set; }
    public bool GameEnded { get; set; } = false;
    public bool IsBingoMode { get; set; } = false;

    public Player? LeadingPlayer()
    {
        var playerCounts = Board.Quests
            .SelectMany(row => row)
            .Where(q => q.CompletedByPlayerId != null)
            .GroupBy(q => q.CompletedByPlayerId)
            .ToDictionary(g => g.Key!, g => g.Count());

        if (playerCounts.Count == 0)
            return null;

        var topCount = playerCounts.Values.Max();
        var topPlayers = playerCounts
            .Where(kvp => kvp.Value == topCount)
            .Select(kvp => kvp.Key)
            .ToList();

        return topPlayers.Count == 1
            ? Players.FirstOrDefault(p => p.Id == topPlayers[0])
            : null;
    }
}

public class ChatMessage
{
    public Player Sender { get; set; }
    public string Message { get; set; }
    public bool IsSystemMessage { get; set; }
}