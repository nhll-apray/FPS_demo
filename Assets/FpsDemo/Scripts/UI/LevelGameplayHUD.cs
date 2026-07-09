using FpsDemo.Game;
using TMPro;
using UnityEngine;

namespace FpsDemo.UI
{
    public class LevelGameplayHUD : MonoBehaviour
    {
        [SerializeField] private LevelDirector levelDirector;
        [SerializeField] private TextMeshProUGUI objectiveText;
        [SerializeField] private TextMeshProUGUI enemyProgressText;
        [SerializeField] private TextMeshProUGUI timerText;

        private void Start()
        {
            ResolveLevelDirector();
            Refresh();
        }

        private void Update()
        {
            if (levelDirector != null && levelDirector.State == LevelDirector.LevelState.Playing)
            {
                Refresh();
            }
        }

        private void ResolveLevelDirector()
        {
            if (levelDirector == null)
            {
                levelDirector = FindFirstObjectByType<LevelDirector>();
            }
        }

        private void Refresh()
        {
            if (objectiveText != null)
            {
                objectiveText.text = GetObjectiveText();
            }

            if (levelDirector == null)
            {
                SetWaitingText();
                return;
            }

            if (enemyProgressText != null)
            {
                enemyProgressText.text = GetEnemyProgressText();
            }

            if (timerText != null)
            {
                timerText.text = $"Time: {FormatTime(levelDirector.ElapsedTime)}";
            }
        }

        private void SetWaitingText()
        {
            if (enemyProgressText != null)
            {
                enemyProgressText.text = "Enemies: 0/0";
            }

            if (timerText != null)
            {
                timerText.text = "Time: 00:00";
            }
        }

        private string GetObjectiveText()
        {
            if (levelDirector == null || !levelDirector.IsUsingWaves)
            {
                return "CLEAR ALL ENEMIES";
            }

            if (levelDirector.IsWaitingForNextWave)
            {
                return $"NEXT WAVE {levelDirector.CurrentWaveNumber}/{levelDirector.TotalWaveCount}";
            }

            string waveName = levelDirector.CurrentWaveName;
            if (!string.IsNullOrWhiteSpace(waveName))
            {
                return $"{waveName.ToUpperInvariant()} {levelDirector.CurrentWaveNumber}/{levelDirector.TotalWaveCount}";
            }

            return $"WAVE {levelDirector.CurrentWaveNumber}/{levelDirector.TotalWaveCount}";
        }

        private string GetEnemyProgressText()
        {
            if (levelDirector == null)
            {
                return "Enemies: 0/0";
            }

            if (!levelDirector.IsUsingWaves)
            {
                return $"Enemies: {levelDirector.KilledEnemyCount}/{levelDirector.TotalEnemyCount}";
            }

            if (levelDirector.CurrentWaveTotalEnemyCount <= 0)
            {
                return $"Total Enemies: {levelDirector.KilledEnemyCount}/{levelDirector.TotalEnemyCount}";
            }

            return $"Wave Enemies: {levelDirector.CurrentWaveKilledEnemyCount}/{levelDirector.CurrentWaveTotalEnemyCount}";
        }

        private static string FormatTime(float seconds)
        {
            seconds = Mathf.Max(0f, seconds);
            int minutes = Mathf.FloorToInt(seconds / 60f);
            int wholeSeconds = Mathf.FloorToInt(seconds % 60f);
            return $"{minutes:00}:{wholeSeconds:00}";
        }
    }
}
