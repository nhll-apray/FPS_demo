using System;
using FpsDemo.Config.Enemy;
using UnityEngine;

namespace FpsDemo.Config.Level
{
    [Serializable]
    public class LevelSpawnGroupConfig
    {
        [SerializeField] private string spawnPointId = "spawn_a";
        [SerializeField] private EnemyPrefabType enemyType = EnemyPrefabType.Goblin;
        [SerializeField, Min(1)] private int count = 1;
        [SerializeField, Min(0f)] private float spacing = 2f;

        public string SpawnPointId => spawnPointId;
        public EnemyPrefabType EnemyType => enemyType;
        public int Count => Mathf.Max(1, count);
        public float Spacing => Mathf.Max(0f, spacing);
    }
}
