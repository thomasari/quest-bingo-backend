using System.Drawing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NanoidDotNet;
using QuestBingo;
using QuestBingo.Models;
using Color = QuestBingo.Models.Color;

[ApiController]
public class RoomController : ControllerBase
{
    private readonly RoomManager _roomManager;
    private readonly QuestManager _questManager;
    private readonly IHubContext<RoomHub> _hub;


    public RoomController(RoomManager roomManager, QuestManager questManager, IHubContext<RoomHub> hub)
    {
        _roomManager = roomManager;
        _questManager = questManager;
        _hub = hub;
    }
    
    [HttpGet("create")]
    public async Task<IActionResult> CreateRoom()
    {
        var roomId = await Nanoid.GenerateAsync("ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890", 5);
        var room = _roomManager.CreateRoom(roomId);
        var board = _questManager.CreateBoard();
        room.Board = board;
        
        return Created($"/{room.Id}", room.Id);
    }
    
    [HttpGet("room/{id}")]
    public IActionResult GetRoom(string id)
    {
        var room = _roomManager.GetRoom(id);
        return room == null ? NotFound() : Ok(room);
    }
    
    [HttpPost("join/{roomId}")]
    public async Task<IActionResult> JoinRoom(string roomId)
    {
        var room = _roomManager.GetRoom(roomId);
        if (room == null) return NotFound();

        var player = new Player { Id = Guid.NewGuid().ToString(), Name = $"Player {room.Players.Count+1}", Color = Color.GetColorHex(room.Players.Count) };
        room.Players.Add(player);

        await _hub.Clients.Group(roomId).SendAsync("PlayerJoined", player); // Notify others
        var systemMessage = new ChatMessage
        {
            Sender = new Player { Name = "System", Color = "#b0b0b0" }, 
            Message = $"{player.Name} joined the game!"
        };

        room.ChatHistory.Add(systemMessage);

        // Broadcast to clients
        await _hub.Clients.Group(roomId)
            .SendAsync("ReceiveChat", "System", "#b0b0b0", systemMessage.Message);

        return Ok(new { player, room });
    }
    
    [HttpPut("room/{roomId}/player/{playerId}")]
    public async Task<IActionResult> UpdateName(string roomId, string playerId, [FromBody] string name)
    {   
        var room = _roomManager.GetRoom(roomId);
        if (room == null) return NotFound($"Room {roomId} not found");

        var player = room.Players.FirstOrDefault(p => p.Id == playerId);
        if (player == null) return NotFound($"Player {playerId} not found");
        
        var systemMessage = new ChatMessage
        {
            Sender = new Player { Name = "System", Color = "#b0b0b0" }, 
            Message = $"{player.Name} changed name to {name}."
        };
        
        player.Name = name;
        
        await _hub.Clients.Group(roomId).SendAsync("PlayerChangedName", player);

        room.ChatHistory.Add(systemMessage);

        // Broadcast to clients
        await _hub.Clients.Group(roomId)
            .SendAsync("ReceiveChat", "System", "#b0b0b0", systemMessage.Message);
        
        return Ok(player);
    }
    [HttpGet("room/{roomId}/start")]
    public async Task<IActionResult> StartGame(string roomId)
    {   
        var room = _roomManager.GetRoom(roomId);
        if (room == null) return NotFound($"Room {roomId} not found");

        room.GameStarted = true;
        
        var systemMessage = new ChatMessage
        {
            Sender = new Player { Name = "System", Color = "#b0b0b0" }, 
            Message = $"Game started!"
        };
        
        await _hub.Clients.Group(roomId).SendAsync("RoomUpdate", room);

        room.ChatHistory.Add(systemMessage);

        // Broadcast to clients
        await _hub.Clients.Group(roomId)
            .SendAsync("ReceiveChat", "System", "#b0b0b0", systemMessage.Message);
        
        return Ok(room);
    }
    
    [HttpGet("room/{roomId}/chat")]
    public IActionResult GetChatHistory(string roomId)
    {
        var room = _roomManager.GetRoom(roomId);
        if (room == null) return NotFound();

        return Ok(room.ChatHistory.Select(msg => new {
            sender = new {
                name = msg.Sender.Name,
                color = msg.Sender.Color
            },
            message = msg.Message
        }));
    }

    [HttpPatch("room/{roomId}/quest/{questId}")]
    public async Task<IActionResult> ToggleQuest(string roomId, string questId, [FromBody] string playerId)
    {
        var room = _roomManager.GetRoom(roomId);
        if (room == null) return NotFound();
        
        foreach (var row in room.Board.Quests)
        {
            var quest = row.FirstOrDefault(q => q.Id == questId);
            if (quest == null) continue;
            
            quest.CompletedByPlayerId = quest.CompletedByPlayerId == playerId ? null : playerId; 

            await _hub.Clients.Group(roomId).SendAsync("RoomUpdate", room);
            return Ok();
        }

        return NotFound("Quest not found");
    }
}