namespace QuestBingo.Models;

public class Player
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Name { get; set; }
    public string? Color { get; set; }

    public static Player CreateSystem() => new Player
    {
        Id = "system", // Optional: fixed ID if needed
        Name = "System",
        Color = "#b0b0b0"
    };
}