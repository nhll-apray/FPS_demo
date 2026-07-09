using FpsDemo.Config;

namespace FpsDemo.Game
{
    public static class LevelSelectionContext
    {
        public static string SelectedLevelConfigPath { get; private set; }
        public static bool HasSelectedLevel => !string.IsNullOrWhiteSpace(SelectedLevelConfigPath);

        public static void SelectLevel(string levelConfigPath)
        {
            SelectedLevelConfigPath = levelConfigPath;
        }

        public static string GetSelectedOrDefault(string fallbackPath)
        {
            if (!string.IsNullOrWhiteSpace(SelectedLevelConfigPath))
            {
                return SelectedLevelConfigPath;
            }

            return string.IsNullOrWhiteSpace(fallbackPath)
                ? GameResourcePaths.Config.Level.Level01
                : fallbackPath;
        }
    }
}
