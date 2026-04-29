using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace PacMan.App.Services;

/// handles saving and loading high scores to/from a JSON file
public class ScoreManager
{
    private readonly string _scorePath;

    /// constructor accepts a custom path, defaults to scores.json
    
    public ScoreManager(string path = "scores.json")
    {
        _scorePath = path;
    }
    public void SaveScores(Dictionary<string, int> scores)
    {
        try
        {
            string json = JsonSerializer.Serialize(scores);
            File.WriteAllText(_scorePath, json);
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Could not save scores: {ex.Message}");
        }
    }

    /// loads scores from JSON file + returns empty dictionary if file doesn't exist

    public Dictionary<string, int> LoadScores()
    {
        try
        {
            string json = File.ReadAllText(_scorePath);
            return JsonSerializer.Deserialize<Dictionary<string, int>>(json)
                   ?? new Dictionary<string, int>();
        }
        catch
        {
            return new Dictionary<string, int>();
        }
    }
}