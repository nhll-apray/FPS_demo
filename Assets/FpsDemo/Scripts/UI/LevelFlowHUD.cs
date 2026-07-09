using FpsDemo.Game;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FpsDemo.UI
{
    public class LevelFlowHUD : MonoBehaviour
    {
        [SerializeField] private LevelDirector levelDirector;
        [SerializeField] private GameObject playerHudRoot;
        [SerializeField] private GameObject startPanel;
        [SerializeField] private GameObject gameplayPanel;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject resultPanel;

        [Header("Result")]
        [SerializeField] private TextMeshProUGUI resultTitleText;
        [SerializeField] private TextMeshProUGUI resultStatsText;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button backToLevelSelectButton;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button pauseRetryButton;
        [SerializeField] private Button pauseBackToLevelSelectButton;
        [SerializeField] private string levelSelectSceneName = "LevelSelectScene";

        private void Start()
        {
            ResolveLevelDirector();
            Bind();
            BindButtons();
            Refresh();
        }

        private void OnDestroy()
        {
            UnbindButtons();
            Unbind();
        }

        public void OnStartButtonClicked()
        {
            if (levelDirector != null)
            {
                levelDirector.BeginLevel();
            }
        }

        public void OnRetryButtonClicked()
        {
            Time.timeScale = 1f;
            Scene activeScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(activeScene.name);
        }

        public void OnBackToLevelSelectButtonClicked()
        {
            if (!string.IsNullOrWhiteSpace(levelSelectSceneName))
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(levelSelectSceneName);
            }
        }

        public void OnResumeButtonClicked()
        {
            if (levelDirector != null)
            {
                levelDirector.ResumeLevel();
            }
        }

        private void ResolveLevelDirector()
        {
            if (levelDirector == null)
            {
                levelDirector = FindFirstObjectByType<LevelDirector>();
            }
        }

        private void Bind()
        {
            if (levelDirector != null)
            {
                levelDirector.OnStateChanged += OnLevelStateChanged;
            }
        }

        private void Unbind()
        {
            if (levelDirector != null)
            {
                levelDirector.OnStateChanged -= OnLevelStateChanged;
            }
        }

        private void BindButtons()
        {
            if (retryButton != null)
            {
                retryButton.onClick.AddListener(OnRetryButtonClicked);
            }

            if (backToLevelSelectButton != null)
            {
                backToLevelSelectButton.onClick.AddListener(OnBackToLevelSelectButtonClicked);
            }

            if (resumeButton != null)
            {
                resumeButton.onClick.AddListener(OnResumeButtonClicked);
            }

            if (pauseRetryButton != null)
            {
                pauseRetryButton.onClick.AddListener(OnRetryButtonClicked);
            }

            if (pauseBackToLevelSelectButton != null)
            {
                pauseBackToLevelSelectButton.onClick.AddListener(OnBackToLevelSelectButtonClicked);
            }
        }

        private void UnbindButtons()
        {
            if (retryButton != null)
            {
                retryButton.onClick.RemoveListener(OnRetryButtonClicked);
            }

            if (backToLevelSelectButton != null)
            {
                backToLevelSelectButton.onClick.RemoveListener(OnBackToLevelSelectButtonClicked);
            }

            if (resumeButton != null)
            {
                resumeButton.onClick.RemoveListener(OnResumeButtonClicked);
            }

            if (pauseRetryButton != null)
            {
                pauseRetryButton.onClick.RemoveListener(OnRetryButtonClicked);
            }

            if (pauseBackToLevelSelectButton != null)
            {
                pauseBackToLevelSelectButton.onClick.RemoveListener(OnBackToLevelSelectButtonClicked);
            }
        }

        private void OnLevelStateChanged(LevelDirector.LevelState state)
        {
            Refresh(state);
        }

        private void Refresh()
        {
            if (levelDirector == null)
            {
                SetPanelActive(startPanel, false);
                SetPanelActive(gameplayPanel, false);
                SetPanelActive(pausePanel, false);
                SetPanelActive(resultPanel, false);
                SetPanelActive(playerHudRoot, false);
                return;
            }

            Refresh(levelDirector.State);
        }

        private void Refresh(LevelDirector.LevelState state)
        {
            bool shouldShowStartPanel = state == LevelDirector.LevelState.Ready && !levelDirector.AutoStartOnLoad;
            bool isGameplayVisible = state == LevelDirector.LevelState.Playing || state == LevelDirector.LevelState.Paused;
            SetPanelActive(startPanel, shouldShowStartPanel);
            SetPanelActive(gameplayPanel, isGameplayVisible);
            SetPanelActive(pausePanel, state == LevelDirector.LevelState.Paused);
            SetPanelActive(resultPanel, state == LevelDirector.LevelState.Victory || state == LevelDirector.LevelState.Defeat);
            SetPanelActive(playerHudRoot, isGameplayVisible);

            if (state == LevelDirector.LevelState.Victory || state == LevelDirector.LevelState.Defeat)
            {
                RefreshResult(state);
            }
        }

        private void RefreshResult(LevelDirector.LevelState state)
        {
            LevelResult result = levelDirector != null ? levelDirector.CurrentResult : LevelResult.Empty;

            if (resultTitleText != null)
            {
                bool isVictory = result.HasResult ? result.IsVictory : state == LevelDirector.LevelState.Victory;
                resultTitleText.text = isVictory ? "VICTORY" : "DEFEAT";
            }

            if (resultStatsText != null)
            {
                resultStatsText.text =
                    $"Kills: {result.KilledEnemies}/{result.TotalEnemies}\n" +
                    $"Time: {result.ElapsedTime:0.00}s\n" +
                    $"Score: {result.Score}\n" +
                    $"Rank: {result.Rank}";
            }
        }

        private static void SetPanelActive(GameObject panel, bool isActive)
        {
            if (panel != null)
            {
                panel.SetActive(isActive);
            }
        }
    }
}
