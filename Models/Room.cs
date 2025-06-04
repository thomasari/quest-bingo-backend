using NanoidDotNet;

namespace QuestBingo.Models;

public class Room(string id)
{
    public string Id { get; set; } = id;
    public List<Player> Players { get; set; } = [];
    public Board Board { get; set; }
    public List<ChatMessage> ChatHistory { get; set; } = new(); // <-- new
    public bool GameStarted { get; set; } = false;

}

public class ChatMessage
{
    public Player Sender { get; set; }
    public string Message { get; set; }
}