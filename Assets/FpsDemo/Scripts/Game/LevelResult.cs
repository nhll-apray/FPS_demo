namespace FpsDemo.Game
{
    public enum LevelResultType
    {
        None,
        Victory,
        Defeat
    }

    public readonly struct LevelResult
    {
        public LevelResult(
            LevelResultType resultType,
            int killedEnemies,
            int totalEnemies,
            float elapsedTime,
            int score,
            string rank)
        {
            ResultType = resultType;
            KilledEnemies = killedEnemies;
            TotalEnemies = totalEnemies;
            ElapsedTime = elapsedTime;
            Score = score;
            Rank = rank;
        }

        public static LevelResult Empty => new LevelResult(LevelResultType.None, 0, 0, 0f, 0, "-");

        public LevelResultType ResultType { get; }
        public int KilledEnemies { get; }
        public int TotalEnemies { get; }
        public float ElapsedTime { get; }
        public int Score { get; }
        public string Rank { get; }
        public bool HasResult => ResultType != LevelResultType.None;
        public bool IsVictory => ResultType == LevelResultType.Victory;
    }
}
