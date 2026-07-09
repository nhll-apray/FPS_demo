using FpsDemo.Config;

namespace FpsDemo.Config.Enemy
{
    public enum EnemyPrefabType
    {
        Goblin,
        RangedElf
    }

    public static class EnemyPrefabTypeExtensions
    {
        public static string GetPrefabPath(this EnemyPrefabType enemyType)
        {
            return enemyType switch
            {
                EnemyPrefabType.RangedElf => GameResourcePaths.Prefabs.Enemy.EnemyRangedElf,
                _ => GameResourcePaths.Prefabs.Enemy.EnemyGoblin
            };
        }
    }
}
