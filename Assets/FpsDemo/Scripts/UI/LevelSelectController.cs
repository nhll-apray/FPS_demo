using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FpsDemo.Config;
using FpsDemo.Game;

namespace FpsDemo.UI
{
    public class LevelSelectController : MonoBehaviour
    {
        [SerializeField] private string levelSceneName = "MainScene";
        [SerializeField] private Button level01Button;
        [SerializeField] private Button level02Button;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void OnEnable()
        {
            if (level01Button != null)
            {
                level01Button.onClick.AddListener(SelectLevel01);
            }

            if (level02Button != null)
            {
                level02Button.onClick.AddListener(SelectLevel02);
            }
        }

        private void OnDisable()
        {
            if (level01Button != null)
            {
                level01Button.onClick.RemoveListener(SelectLevel01);
            }

            if (level02Button != null)
            {
                level02Button.onClick.RemoveListener(SelectLevel02);
            }
        }

        public void SelectLevel01()
        {
            LoadSelectedLevel(GameResourcePaths.Config.Level.Level01);
        }

        public void SelectLevel02()
        {
            LoadSelectedLevel(GameResourcePaths.Config.Level.Level02);
        }

        private void LoadSelectedLevel(string levelConfigPath)
        {
            if (string.IsNullOrWhiteSpace(levelSceneName))
            {
                return;
            }

            LevelSelectionContext.SelectLevel(levelConfigPath);
            SceneManager.LoadScene(levelSceneName);
        }
    }
}
