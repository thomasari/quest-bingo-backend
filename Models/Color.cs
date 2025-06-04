namespace QuestBingo.Models;

public static class Color
{
    public static string[] Hexes = ["#277da1","#f94144", "#90be6d", "#f9c74f", "#f8961e", "#4d908e"];

    public static string GetColorHex(int i)
    {
        return Hexes[Math.Min(i, Hexes.Length-1)];
    }
}