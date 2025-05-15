using UnityEngine;

[System.Serializable]
public class LeaderboardEntry
{
    public string PlayerName;
    public int Score;
}

[System.Serializable]
public class GameSaveData
{
    public bool SoundMusicEnabled = true;
    public bool SoundEffectsEnabled = true;
    
    public LeaderboardEntry[] LeaderboardEntries = new LeaderboardEntry[10];
}
