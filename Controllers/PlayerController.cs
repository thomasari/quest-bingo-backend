using Microsoft.AspNetCore.Mvc;
using NanoidDotNet;
using QuestBingo.Models;

[ApiController]
[Route("[controller]")]
public class PlayerController : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> CreatePlayer()
    {
        var player = new Player();
        return Created($"/player/{player.Id}", player);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlayer([FromQuery] string id)
    {
        var player = new Player();
        return Ok(player);
    }

}