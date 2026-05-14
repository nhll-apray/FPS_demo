using System.Collections;
using UnityEngine;

namespace FpsDemo.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private float respawnDelay = 2.5f;
        
        [SerializeField] private GameObject currentEnemy;
        [SerializeField] private bool spawnOnStart = true;

        private Coroutine _respawnCoroutine;

        private void Start()
        {
            if (spawnPoint == null)
                spawnPoint = transform;

            if (currentEnemy == null && spawnOnStart)
            {
                SpawnEnemy();
            }
        }

        private void Update()
        {
            if (currentEnemy != null)
                return;

            if (_respawnCoroutine != null)
                return;

            _respawnCoroutine = StartCoroutine(RespawnRoutine());
        }

        private IEnumerator RespawnRoutine()
        {
            yield return new WaitForSeconds(respawnDelay);

            SpawnEnemy();

            _respawnCoroutine = null;
        }

        private void SpawnEnemy()
        {
            if (enemyPrefab == null)
            {
                return;
            }

            currentEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}