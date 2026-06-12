using System.Collections;
using System.Collections.Generic;
using FpsDemo.Config;
using FpsDemo.Game;
using UnityEngine;

namespace FpsDemo.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        private enum EnemyPrefabType
        {
            Goblin,
            RangedElf
        }

        [SerializeField] private Transform spawnPoint;
        [SerializeField] private EnemyPrefabType enemyPrefabType = EnemyPrefabType.Goblin;
        [SerializeField] private int spawnCount = 2;
        [SerializeField] private float spawnSpacing = 2f;
        [SerializeField] private float respawnDelay = 2.5f;

        [SerializeField] private List<GameObject> currentEnemies = new List<GameObject>();

        private Coroutine _respawnCoroutine;

        private void Start()
        {
            if (spawnPoint == null)
            {
                spawnPoint = transform;
            }

            if (!HasLivingEnemies())
            {
                SpawnEnemies();
            }
        }

        private void Update()
        {
            RemoveMissingEnemies();
            if (!HasLivingEnemies() && _respawnCoroutine == null)
            {
                _respawnCoroutine = StartCoroutine(RespawnRoutine());
            }
        }

        private IEnumerator RespawnRoutine()
        {
            yield return new WaitForSeconds(respawnDelay);

            SpawnEnemies();
            _respawnCoroutine = null;
        }

        private void SpawnEnemies()
        {
            GameObject prefab = GameResources.LoadPrefab(GetEnemyPrefabPath());
            if (prefab == null)
            {
                return;
            }

            currentEnemies.Clear();
            int count = Mathf.Max(1, spawnCount);
            for (int i = 0; i < count; i++)
            {
                GameObject enemy = Instantiate(prefab, GetSpawnPosition(i, count), spawnPoint.rotation);
                currentEnemies.Add(enemy);
            }
        }

        private Vector3 GetSpawnPosition(int index, int count)
        {
            if (count <= 1)
            {
                return spawnPoint.position;
            }

            float centerOffset = (count - 1) * 0.5f;
            float offset = (index - centerOffset) * spawnSpacing;
            return spawnPoint.position + spawnPoint.right * offset;
        }

        private bool HasLivingEnemies()
        {
            RemoveMissingEnemies();
            return currentEnemies.Count > 0;
        }

        private void RemoveMissingEnemies()
        {
            for (int i = currentEnemies.Count - 1; i >= 0; i--)
            {
                if (currentEnemies[i] == null)
                {
                    currentEnemies.RemoveAt(i);
                }
            }
        }

        private string GetEnemyPrefabPath()
        {
            return enemyPrefabType switch
            {
                EnemyPrefabType.RangedElf => GameResourcePaths.Prefabs.Enemy.EnemyRangedElf,
                _ => GameResourcePaths.Prefabs.Enemy.EnemyGoblin
            };
        }
    }
}
