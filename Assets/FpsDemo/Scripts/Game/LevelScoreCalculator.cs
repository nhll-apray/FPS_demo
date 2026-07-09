using System;

namespace FpsDemo.Game
{
    public static class LevelScoreCalculator
    {
        private const int EnemyKillScore = 100;
        private const int VictoryBonus = 1000;
        private const int PerfectClearBonus = 200;
        private const int MaxTimeBonus = 600;
        private const int TimePenaltyPerSecond = 10;

        public static LevelResult CreateResult(
            LevelResultType resultType,
            int killedEnemies,
            int totalEnemies,
            float elapsedTime)
        {
            killedEnemies = Math.Max(0, killedEnemies);
            totalEnemies = Math.Max(0, totalEnemies);
            elapsedTime = Math.Max(0f, elapsedTime);

            int score = CalculateScore(resultType, killedEnemies, totalEnemies, elapsedTime);
            string rank = CalculateRank(resultType, score);
            return new LevelResult(resultType, killedEnemies, totalEnemies, elapsedTime, score, rank);
        }

        private static int CalculateScore(
            LevelResultType resultType,
            int killedEnemies,
            int totalEnemies,
            float elapsedTime)
        {
            int score = killedEnemies * EnemyKillScore;

            if (resultType != LevelResultType.Victory)
            {
                return score;
            }

            score += VictoryBonus;

            if (totalEnemies > 0 && killedEnemies >= totalEnemies)
            {
                score += PerfectClearBonus;
            }

            int timeBonus = Math.Max(0, MaxTimeBonus - (int)Math.Round(elapsedTime * TimePenaltyPerSecond));
            score += timeBonus;
            return score;
        }

        private static string CalculateRank(LevelResultType resultType, int score)
        {
            if (resultType != LevelResultType.Victory)
            {
                return "D";
            }

            if (score >= 2000)
            {
                return "S";
            }

            if (score >= 1700)
            {
                return "A";
            }

            if (score >= 1400)
            {
                return "B";
            }

            return "C";
        }
    }
}
