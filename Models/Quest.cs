namespace QuestBingo.Models;

public class Quest
{
    public string Id { get; set; }
    public string Text { get; set; }
    public string? CompletedByPlayerId { get; set; }
}