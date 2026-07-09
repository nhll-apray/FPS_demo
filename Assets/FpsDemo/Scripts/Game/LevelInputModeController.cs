using FpsDemo.Player;
using UnityEngine;

namespace FpsDemo.Game
{
    public class LevelInputModeController : MonoBehaviour
    {
        [SerializeField] private LevelDirector levelDirector;
        [SerializeField] private PlayerInputReader playerInputReader;
        [SerializeField] private bool unlockCursorOnDestroy = true;

        private void Awake()
        {
            ResolveReferences();
        }

        private void OnEnable()
        {
            ResolveReferences();

            if (levelDirector != null)
            {
                levelDirector.OnStateChanged += HandleLevelStateChanged;
                ApplyInputMode(levelDirector.State);
            }
            else
            {
                ApplyInputMode(LevelDirector.LevelState.Ready);
            }
        }

        private void OnDisable()
        {
            if (levelDirector != null)
            {
                levelDirector.OnStateChanged -= HandleLevelStateChanged;
            }

            if (unlockCursorOnDestroy)
            {
                SetCursorForGameplay(false);
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus && levelDirector != null)
            {
                ApplyInputMode(levelDirector.State);
            }
        }

        private void HandleLevelStateChanged(LevelDirector.LevelState state)
        {
            ApplyInputMode(state);
        }

        private void ApplyInputMode(LevelDirector.LevelState state)
        {
            bool isGameplay = state == LevelDirector.LevelState.Playing;

            if (playerInputReader != null)
            {
                playerInputReader.SetGameplayInputEnabled(isGameplay);
            }

            SetCursorForGameplay(isGameplay);
        }

        private void SetCursorForGameplay(bool isGameplay)
        {
            Cursor.lockState = isGameplay ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !isGameplay;
        }

        private void ResolveReferences()
        {
            if (levelDirector == null)
            {
                levelDirector = GetComponent<LevelDirector>();
            }

            if (levelDirector == null)
            {
                levelDirector = FindFirstObjectByType<LevelDirector>();
            }

            if (playerInputReader == null)
            {
                PlayerEntity player = GameManager.Instance != null
                    ? GameManager.Instance.CurrentPlayer
                    : null;

                if (player != null)
                {
                    playerInputReader = player.GetComponent<PlayerInputReader>();
                }
            }

            if (playerInputReader == null)
            {
                playerInputReader = FindFirstObjectByType<PlayerInputReader>();
            }
        }
    }
}
