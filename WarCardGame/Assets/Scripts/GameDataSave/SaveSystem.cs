using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace GameDataSave
{
    public static class SaveSystem
    {
        private static string SavedGameDataFilePath => Path.Combine(Application.persistentDataPath, "war_card_game_save.json");

        public static void Save(GameSaveData data)
        {
            var json = JsonUtility.ToJson(data, true);

            File.WriteAllText(SavedGameDataFilePath, json);
        }

        public static GameSaveData Load()
        {
            if (!File.Exists(SavedGameDataFilePath)) return new GameSaveData();
            return JsonUtility.FromJson<GameSaveData>(File.ReadAllText(SavedGameDataFilePath));
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




}
