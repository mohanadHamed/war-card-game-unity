namespace Game.DataTypes
{
    public struct GameSettings
    {
        public int ScoreToWin;
        public int BotDelayMs;
        public int DelayToLoadGameOverMs;
        public int DelayToStartNextRoundMs;

        public static GameSettings Default => new GameSettings
        {
            ScoreToWin = 8,
            BotDelayMs = 1000,
            DelayToLoadGameOverMs = 1000,
            DelayToStartNextRoundMs = 1000
        };
    }
}
