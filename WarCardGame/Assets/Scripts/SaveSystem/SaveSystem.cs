using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class SaveSystem
{
    static string path => Path.Combine(Application.persistentDataPath, "war_card_game_save.json");

    public static void Save(GameSaveData data)
    {
        var json = JsonUtility.ToJson(data, true);

        File.WriteAllText(path, json);
    }

    public static GameSaveData Load()
    {
        if (!File.Exists(path)) return new GameSaveData();
        return JsonUtility.FromJson<GameSaveData>(File.ReadAllText(path));
    }

    public static void SaveLeaderboardEntry(LeaderboardEntry entry)
    {
        var data  = Load();
        var entries = data.LeaderboardEntries.ToList();
        var found = false;

        for (int i = 0; i < entries.Count; i++)
        {
            if (entries[i] != null && entries[i].PlayerName == entry.PlayerName)
            {
                entries[i].Score = Mathf.Max(entries[i].Score, entry.Score);
                found = true;
                break;
            }
        }

        if (!found)
        {
            entries.Add(entry);
        }

        entries = entries.OrderByDescending(e => e.Score).ToList();
        if (entries.Count > 10)
        {
            entries.RemoveAt(10);
        }

        data.LeaderboardEntries = entries.ToArray();
        Save(data);
    }

    public static List<LeaderboardEntry> LoadLeaderboardEntries()
    {
        var data = Load();
        return data.LeaderboardEntries.ToList();
    }
}




