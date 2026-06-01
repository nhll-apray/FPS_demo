using System.Collections;
using FpsDemo.Game;
using UnityEngine;

namespace FpsDemo.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private float respawnDelay = 2.5f;
        
        [SerializeField] private GameObject currentEnemy;

        private Coroutine _respawnCoroutine;

        private void Start()
        {
            if (spawnPoint == null)
                spawnPoint = transform;

            if (currentEnemy == null)
            {
                SpawnEnemy();
            }
        }

        private void Update()
        {
            if (currentEnemy == null && _respawnCoroutine == null)
            {
                _respawnCoroutine = StartCoroutine(RespawnRoutine());
            }
        }

        private IEnumerator RespawnRoutine()
        {
            yield return new WaitForSeconds(respawnDelay);

            SpawnEnemy();

            _respawnCoroutine = null;
        }

        private void SpawnEnemy()
        {
            GameObject prefab = GameResources.LoadPrefab(GameResourcePaths.Prefabs.Enemy.EnemyDum);
            if (prefab == null)
                return;

            currentEnemy = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
