using System.Text.Json;
using QuestBingo.Models;

namespace QuestBingo;

public class QuestManager
{
    private readonly List<string> _allQuestTexts;

    public QuestManager(IWebHostEnvironment env)
    {
        var filePath = Path.Combine(env.ContentRootPath, "Assets", "Quests.json");
        var json = File.ReadAllText(filePath);
        _allQuestTexts = JsonSerializer.Deserialize<List<string>>(json) ?? [];
    }

    public Board CreateBoard()
    {
        var random = new Random();
        var selectedTexts = _allQuestTexts
            .OrderBy(_ => random.Next())
            .Take(25)
            .Select((text, index) => new Quest
            {
                Id = index,
                Text = text
            })
            .ToArray();

        var grid = new Quest[5][];
        for (var i = 0; i < 5; i++)
        {
            grid[i] = selectedTexts.Skip(i * 5).Take(5).ToArray();
        }

        return new Board { Quests = grid };
    }
}