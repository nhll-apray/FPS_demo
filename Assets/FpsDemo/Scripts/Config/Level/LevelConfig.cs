using UnityEngine;

namespace FpsDemo.Config.Level
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "FpsDemo/Level/Level Config")]
    public class LevelConfig : ScriptableObject
    {
        [SerializeField] private string levelId = "level_01";
        [SerializeField] private string displayName = "Level 01";
        [SerializeField] private LevelWaveConfig[] waves;

        public string LevelId => levelId;
        public string DisplayName => displayName;
        public LevelWaveConfig[] Waves => waves;
    }
}
