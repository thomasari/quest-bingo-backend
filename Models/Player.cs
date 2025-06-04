namespace QuestBingo.Models;

public class Player
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Name { get; set; }
    public string? Color { get; set; }
}